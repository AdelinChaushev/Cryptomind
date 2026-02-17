using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Services
{
	public class AuthServiceTests
	{
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly AuthService _service;

		public AuthServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_configurationMock = new Mock<IConfiguration>();
			_configurationMock.Setup(c => c["JWT:Secret"])
				.Returns("super-secret-key-that-is-long-enough-for-hmac");
			_configurationMock.Setup(c => c["JWT:ValidIssuer"]).Returns("TestIssuer");
			_configurationMock.Setup(c => c["JWT:ValidAudience"]).Returns("TestAudience");

			_service = new AuthService(_userManagerMock.Object, _configurationMock.Object);

			_userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(IdentityResult.Success);
			_userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);
		}

		private static ApplicationUser MakeUser(string id, string userName = "testuser",
			string email = "test@test.com", bool isDeactivated = false) => new()
			{
				Id = id,
				UserName = userName,
				Email = email,
				IsDeactivated = isDeactivated,
			};

		#region Authenticate

		[Fact]
		public async Task Authenticate_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("ghost@test.com"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.Authenticate("ghost@test.com", "password"));
		}

		[Fact]
		public async Task Authenticate_Throws_WhenPasswordIsIncorrect()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrongpassword")).ReturnsAsync(false);

			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.Authenticate(user.Email, "wrongpassword"));
		}

		[Fact]
		public async Task Authenticate_Throws_WhenAccountIsDeactivated()
		{
			var user = MakeUser("u1", isDeactivated: true);
			_userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.Authenticate(user.Email, "password"));
		}

		[Fact]
		public async Task Authenticate_ReturnsUser_WhenCredentialsAreValid()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.CheckPasswordAsync(user, "correctpassword")).ReturnsAsync(true);

			var result = await _service.Authenticate(user.Email, "correctpassword");

			Assert.Equal("u1", result.Id);
		}

		#endregion

		#region GenerateJSONWebToken

		[Fact]
		public async Task GenerateJSONWebToken_ReturnsNonEmptyToken()
		{
			var user = MakeUser("u1", "alice");
			_userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

			var token = await _service.GenerateJSONWebToken(user);

			Assert.False(string.IsNullOrWhiteSpace(token));
		}

		[Fact]
		public async Task GenerateJSONWebToken_ContainsUserIdAndUsernameClaims()
		{
			var user = MakeUser("u1", "alice");
			_userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

			var tokenString = await _service.GenerateJSONWebToken(user);
			var decoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

			Assert.Contains(decoded.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "u1");
			Assert.Contains(decoded.Claims, c => c.Type == ClaimTypes.Name && c.Value == "alice");
		}

		[Fact]
		public async Task GenerateJSONWebToken_ContainsRoleClaims_WhenUserHasRoles()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin" });

			var tokenString = await _service.GenerateJSONWebToken(user);
			var decoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

			Assert.Contains(decoded.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
		}

		[Fact]
		public async Task GenerateJSONWebToken_TokenExpires_InThreeHours()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

			var before = DateTime.UtcNow.AddHours(3).AddSeconds(-5);
			var tokenString = await _service.GenerateJSONWebToken(user);
			var after = DateTime.UtcNow.AddHours(3).AddSeconds(5);

			var decoded = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

			Assert.InRange(decoded.ValidTo, before, after);
		}

		#endregion

		#region CreateUser

		[Fact]
		public async Task CreateUser_Throws_WhenEmailAlreadyExists()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("taken@test.com"))
				.ReturnsAsync(MakeUser("u1"));

			await Assert.ThrowsAsync<ArgumentException>(
				() => _service.CreateUser("alice", "taken@test.com", "password"));
		}

		[Fact]
		public async Task CreateUser_ReturnsNull_WhenCreationFails()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("new@test.com"))
				.ReturnsAsync((ApplicationUser?)null);
			_userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed());

			var result = await _service.CreateUser("alice", "new@test.com", "password");

			Assert.Null(result);
		}

		[Fact]
		public async Task CreateUser_ReturnsUser_WithCorrectFields_WhenSuccessful()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("new@test.com"))
				.ReturnsAsync((ApplicationUser?)null);
			_userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

			var result = await _service.CreateUser("alice", "new@test.com", "password");

			Assert.NotNull(result);
			Assert.Equal("alice", result.UserName);
			Assert.Equal("new@test.com", result.Email);
			Assert.True(result.EmailConfirmed);
		}

		[Fact]
		public async Task CreateUser_AssignsUserRole_WhenCreationSucceeds()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("new@test.com"))
				.ReturnsAsync((ApplicationUser?)null);
			_userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);

			await _service.CreateUser("alice", "new@test.com", "password");

			_userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
		}

		[Fact]
		public async Task CreateUser_DoesNotAssignRole_WhenCreationFails()
		{
			_userManagerMock.Setup(m => m.FindByEmailAsync("new@test.com"))
				.ReturnsAsync((ApplicationUser?)null);
			_userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed());

			await _service.CreateUser("alice", "new@test.com", "password");

			_userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
		}

		#endregion

		#region DeactivateAccount

		[Fact]
		public async Task DeactivateAccount_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.DeactivateAccount("ghost"));
		}

		[Fact]
		public async Task DeactivateAccount_Throws_WhenUserIsAlreadyDeactivated()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(MakeUser("u1", isDeactivated: true));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.DeactivateAccount("u1"));
		}

		[Fact]
		public async Task DeactivateAccount_SetsIsDeactivatedAndDeactivatedAt()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.DeactivateAccount("u1");

			Assert.True(user.IsDeactivated);
			Assert.NotNull(user.DeactivatedAt);
		}

		#endregion
	}
}