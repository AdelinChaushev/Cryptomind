using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cryptomind.Core.Services
{
	public class AuthService(
		UserManager<ApplicationUser> userManager,
		IConfiguration configuration) : IAuthService
	{
		public async Task<ApplicationUser> Authenticate(string email, string password)
		{
			var user = await userManager.FindByEmailAsync(email);
			bool passwordValid = user != null && await userManager.CheckPasswordAsync(user, password);

			if (!passwordValid)
				throw new UnauthorizedException("Невалидни данни");
			if (user.IsDeactivated)
				throw new ConflictException("Този акаунт е деактивиран");
			return user;
		}
		public async Task<string> GenerateJSONWebToken(ApplicationUser user)
		{
			var userRoles = await userManager.GetRolesAsync(user);

			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			string jwtSecret = configuration["JWT:Secret"];
			byte[] jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSecret);
			var authSigningKey = new SymmetricSecurityKey(jwtSecretBytes);

			var token = new JwtSecurityToken(
				issuer: configuration["JWT:ValidIssuer"],
				audience: configuration["JWT:ValidAudience"],
				expires: DateTime.UtcNow.AddHours(5),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

			string issuedToken = new JwtSecurityTokenHandler().WriteToken(token);
			return issuedToken;
		}
		public async Task<ApplicationUser> CreateUser(string userName, string email, string password)
		{
			var userExist = await userManager.FindByEmailAsync(email);
			if (userExist != null)
			{
				throw new ConflictException("Потребител с този имейл вече съществува");
			}

			if (string.Equals(userName, CipherErrorConstants.AnonymousUser, StringComparison.OrdinalIgnoreCase))
			{
				throw new ConflictException($"Не може да създадете потребител с име: {userName}");
			}

			if (await userManager.FindByNameAsync(userName) != null)
			{
				throw new ConflictException("Вече съществува потребител с това име");
			}

			if (userName.Length < 3 || userName.Length > 16)
				throw new CustomValidationException("Спазвайте ограниченията за името");

			if (password.Length < 8)
				throw new CustomValidationException("Спазвайте ограниченията на паролата");

			ApplicationUser user = new ApplicationUser()
			{
				UserName = userName,
				Email = email,
				EmailConfirmed = true,
				RegisteredAt = DateTime.UtcNow.AddHours(2),
			};

			var result = await userManager.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				return null;
			}
			await userManager.AddToRoleAsync(user, "User");
			return user;
		}
		public async Task DeactivateAccount(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			if (user == null)
				throw new NotFoundException("Потребителят не е намерен");

			if (user.IsDeactivated)
				throw new ConflictException("Този акаунт е деактивиран");

			if (await userManager.IsInRoleAsync(user, "Admin"))
				throw new ConflictException("Администраторите не могат да деактивират собствения си акаунт");

			user.IsDeactivated = true;
			user.DeactivatedAt = DateTime.UtcNow.AddHours(2);
			await userManager.UpdateAsync(user);
		}
	}
}
