using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cryptomind.Common.ViewModels.UserViewModels;

namespace Cryptomind.Core.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IConfiguration configuration;
		private readonly IRepository<ApplicationUser, string> userRepo;
		public UserService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IRepository<ApplicationUser, string> userRepo)
		{

			this.configuration = configuration;
			this.userManager = userManager;
			this.userRepo = userRepo;
		}
		public async Task<ApplicationUser> Authenticate(string email, string password)
		{
			var user = await userManager.FindByEmailAsync(email);

			if (user != null && await this.userManager.CheckPasswordAsync(user, password))
				return user;

			return null;
		}
		public async Task<string> GenerateJSONWebToken(ApplicationUser user)
		{
			// Get user roles
			var userRoles = await userManager.GetRolesAsync(user);

			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			// Add role claims to the token
			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			string jwtSecret = this.configuration["JWT:Secret"];
			byte[] jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSecret);
			var authSigningKey = new SymmetricSecurityKey(jwtSecretBytes);

			var token = new JwtSecurityToken(
				issuer: this.configuration["JWT:ValidIssuer"],
				audience: this.configuration["JWT:ValidAudience"],
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
				throw new ArgumentException("User with this password already exists");
			}

			var result = await userManager.CreateAsync(user, password);
			if (!result.Succeeded)
			{
				return null;
			}
			await userManager.AddToRoleAsync(user, "User");
			return user;
		}
		public async Task<IEnumerable<string>> GetRolesUsers(string id)
			=> await userManager.GetRolesAsync(await userManager.FindByIdAsync(id));
		public async Task RemoveUserFromRole(string userId, string role)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (!await userManager.IsInRoleAsync(user, role))
			{
				return;
			}

			await userManager.RemoveFromRoleAsync(user, role);
		}
		public async Task AddUserToRole(string userId, string role)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (await userManager.IsInRoleAsync(user, role))
			{
				return;
			}
			await userManager.AddToRoleAsync(user, role);
		}
		public async Task DeactivateAccount(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			await userManager.DeleteAsync(user);
		}
		public async Task<AccountViewModel?> GetUserAccountInfo(string id)
		{
			var user = userRepo.GetAllAttached()
				.Include(x => x.Badges)
				.ThenInclude(x => x.Badge)
				.FirstOrDefault(x => x.Id == id);

			ICollection<BadgeViewModel> badges = user.Badges.Select(x => new BadgeViewModel
			{
				Title = x.Badge.Title,
				Description = x.Badge.Description,
				EarnedBy = x.Badge.EarnedBy,
			}).ToList();

			AccountViewModel result = new AccountViewModel()
			{
				Username = user.UserName,
				Email = user.Email,
				Roles = (await userManager.GetRolesAsync(user)).ToArray(),
				RegisteredAt = user.RegisteredAt,
				//BannedAt = user.BannedAt,
				Points = user.Score,
				SolvedCount = user.SolvedCount,
				Score = user.Score,
				AttemptedCiphers = user.AttemptedCiphers,
				LeaderBoardPlace = user.LeaderBoardPlace,
				SuccessRate = user.SuccessRate,
				Badges = badges,
			};
			return result;
		}
	}
}
