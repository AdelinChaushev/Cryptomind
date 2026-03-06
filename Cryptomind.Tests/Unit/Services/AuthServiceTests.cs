using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class AuthServiceTests
	{
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly AuthService service;

		public AuthServiceTests()
		{
			Environment.SetEnvironmentVariable("JWT_SECRET", "super_secret_test_key_that_is_long_enough_32chars");
			Environment.SetEnvironmentVariable("JWT_ISSUER", "TestIssuer");
			Environment.SetEnvironmentVariable("JWT_AUDIENCE", "TestAudience");

			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new AuthService(userManagerMock.Object);

			userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());
			userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(false);
		}

		private static ApplicationUser MakeUser(
			string id = "u1",
			string userName = "testuser",
			string email = "test@test.com",
			bool isDeactivated = false) => new()
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
			userManagerMock.Setup(m => m.FindByEmailAsync("ghost@test.com"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => service.Authenticate("ghost@test.com", "password"));
		}

		[Fact]
		public async Task Authenticate_Throws_WhenPasswordIsInvalid()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			userManagerMock.Setup(m => m.CheckPasswordAsync(user, "wrongpassword"))
				.ReturnsAsync(false);

			await Assert.ThrowsAsync<UnauthorizedException>(
				() => service.Authenticate(user.Email, "wrongpassword"));
		}

		[Fact]
		public async Task Authenticate_Throws_WhenUserIsDeactivated()
		{
			var user = MakeUser(isDeactivated: true);
			userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password123"))
				.ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.Authenticate(user.Email, "password123"));
		}

		[Fact]
		public async Task Authenticate_ReturnsUser_WhenCredentialsAreValid()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
			userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password123"))
				.ReturnsAsync(true);

			var result = await service.Authenticate(user.Email, "password123");

			Assert.Equal(user.Id, result.Id);
		}

		#endregion

		#region GenerateJSONWebToken

		[Fact]
		public async Task GenerateJSONWebToken_ReturnsNonEmptyString()
		{
			var user = MakeUser();

			var token = await service.GenerateJSONWebToken(user);

			Assert.False(string.IsNullOrWhiteSpace(token));
		}

		[Fact]
		public async Task GenerateJSONWebToken_ContainsNameIdentifierClaim()
		{
			var user = MakeUser(id: "u1");

			var token = await service.GenerateJSONWebToken(user);

			var handler = new JwtSecurityTokenHandler();
			var parsed = handler.ReadJwtToken(token);
			var claim = parsed.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

			Assert.NotNull(claim);
			Assert.Equal("u1", claim.Value);
		}

		[Fact]
		public async Task GenerateJSONWebToken_ContainsUsernameClaim()
		{
			var user = MakeUser(userName: "alice");

			var token = await service.GenerateJSONWebToken(user);

			var handler = new JwtSecurityTokenHandler();
			var parsed = handler.ReadJwtToken(token);
			var claim = parsed.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

			Assert.NotNull(claim);
			Assert.Equal("alice", claim.Value);
		}

		[Fact]
		public async Task GenerateJSONWebToken_ContainsRoleClaim_WhenUserHasRole()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin" });

			var token = await service.GenerateJSONWebToken(user);

			var handler = new JwtSecurityTokenHandler();
			var parsed = handler.ReadJwtToken(token);
			var roleClaim = parsed.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

			Assert.NotNull(roleClaim);
			Assert.Equal("Admin", roleClaim.Value);
		}

		[Fact]
		public async Task GenerateJSONWebToken_TokenExpires_InFiveHours()
		{
			var user = MakeUser();

			var token = await service.GenerateJSONWebToken(user);

			var handler = new JwtSecurityTokenHandler();
			var parsed = handler.ReadJwtToken(token);

			Assert.True(parsed.ValidTo > DateTime.UtcNow.AddHours(4.9));
			Assert.True(parsed.ValidTo < DateTime.UtcNow.AddHours(5.1));
		}

		#endregion

		#region CreateUser

		[Fact]
		public async Task CreateUser_Throws_WhenEmailAlreadyExists()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync("taken@test.com"))
				.ReturnsAsync(MakeUser());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.CreateUser("newuser", "taken@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_Throws_WhenUsernameIsAnonymous()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.CreateUser("anonymous", "new@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_Throws_WhenUsernameAlreadyExists()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync("new@test.com"))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync("takenuser"))
				.ReturnsAsync(MakeUser());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.CreateUser("takenuser", "new@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_Throws_WhenUsernameTooShort()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.CreateUser("ab", "new@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_Throws_WhenUsernameTooLong()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.CreateUser("thisusernameiswaytoolong", "new@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_Throws_WhenPasswordTooShort()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.CreateUser("validuser", "new@test.com", "short"));
		}

		[Fact]
		public async Task CreateUser_EmailDuplicateCheck_TakesPriority_OverUsernameLength()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync("taken@test.com"))
				.ReturnsAsync(MakeUser());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.CreateUser("ab", "taken@test.com", "password123"));
		}

		[Fact]
		public async Task CreateUser_ReturnsNull_WhenIdentityCreationFails()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Failed());

			var result = await service.CreateUser("validuser", "new@test.com", "password123");

			Assert.Null(result);
		}

		[Fact]
		public async Task CreateUser_AssignsUserRole_WhenCreationSucceeds()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
				.ReturnsAsync(IdentityResult.Success);

			var result = await service.CreateUser("validuser", "new@test.com", "password123");

			Assert.NotNull(result);
			userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
		}

		[Fact]
		public async Task CreateUser_SetsEmailConfirmedToTrue()
		{
			userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>()))
				.ReturnsAsync((ApplicationUser?)null);
			ApplicationUser? captured = null;
			userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.Callback<ApplicationUser, string>((u, _) => captured = u)
				.ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
				.ReturnsAsync(IdentityResult.Success);

			await service.CreateUser("validuser", "new@test.com", "password123");

			Assert.NotNull(captured);
			Assert.True(captured.EmailConfirmed);
		}

		#endregion

		#region DeactivateAccount

		[Fact]
		public async Task DeactivateAccount_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.DeactivateAccount("ghost"));
		}

		[Fact]
		public async Task DeactivateAccount_Throws_WhenAlreadyDeactivated()
		{
			var user = MakeUser(isDeactivated: true);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.DeactivateAccount("u1"));
		}

		[Fact]
		public async Task DeactivateAccount_Throws_WhenUserIsAdmin()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.DeactivateAccount("u1"));
		}

		[Fact]
		public async Task DeactivateAccount_SetsIsDeactivatedAndDeactivatedAt()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.DeactivateAccount("u1");

			Assert.True(user.IsDeactivated);
			Assert.NotNull(user.DeactivatedAt);
		}

		[Fact]
		public async Task DeactivateAccount_CallsUpdateAsync()
		{
			var user = MakeUser();
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.DeactivateAccount("u1");

			userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
		}

		#endregion
	}
}