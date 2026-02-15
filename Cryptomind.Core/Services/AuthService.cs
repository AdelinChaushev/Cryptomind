using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class AuthService(
		UserManager<ApplicationUser> userManager,
		IConfiguration configuration) : IAuthService
	{
		public async Task<ApplicationUser> Authenticate(string email, string password)
		{
			var user = await userManager.FindByEmailAsync(email);

			if (user != null && await userManager.CheckPasswordAsync(user, password))
				return user;

			return null;
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
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

			string issuedToken = new JwtSecurityTokenHandler().WriteToken(token);
			return issuedToken;
		}
		public async Task<ApplicationUser> CreateUser(string userName, string email, string password)
		{
			ApplicationUser user = new ApplicationUser()
			{
				UserName = userName,
				Email = email,
				EmailConfirmed = true,
				RegisteredAt = DateTime.Now,
			};

			var userExist = await userManager.FindByEmailAsync(email);
			if (userExist != null)
			{
				throw new ArgumentException("User with this email already exists");
			}

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
				throw new InvalidOperationException("User not found.");

			await userManager.DeleteAsync(user);
		}
	}
}
