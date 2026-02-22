using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class AdminUserServiceTests
	{
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly AdminUserService _service;

		public AdminUserServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_service = new AdminUserService(_userManagerMock.Object);

			_userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(IdentityResult.Success);
			_userManagerMock.Setup(m => m.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.IsAny<DateTimeOffset?>()))
				.ReturnsAsync(IdentityResult.Success);
			_userManagerMock.Setup(m => m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()))
				.ReturnsAsync(IdentityResult.Success);
			_userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(IdentityResult.Success);
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());
			_userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(false);
		}

		private static ApplicationUser MakeUser(string id, string userName = "testuser",
			string email = "test@test.com", bool isBanned = false, bool isDeactivated = false,
			List<Cipher>? uploadedCiphers = null, List<UserSolution>? cipherAnswers = null,
			List<HintRequest>? hintsRequested = null) => new()
			{
				Id = id,
				UserName = userName,
				Email = email,
				IsBanned = isBanned,
				IsDeactivated = isDeactivated,
				UploadedCiphers = uploadedCiphers ?? new List<Cipher>(),
				CipherAnswers = cipherAnswers ?? new List<UserSolution>(),
				HintsRequested = hintsRequested ?? new List<HintRequest>(),
			};

		private static ConcreteCipher MakeCipher(int id, ApprovalStatus status,
			CipherType? type = null, int points = 100) => new()
			{
				Id = id,
				Status = status,
				TypeOfCipher = type,
				Points = points,
				Title = $"Cipher {id}",
				CreatedAt = DateTime.UtcNow,
				ChallengeType = ChallengeType.Standard,
			};

		private void SetupUsers(params ApplicationUser[] users)
		{
			var mock = users.AsQueryable().BuildMock();
			_userManagerMock.Setup(m => m.Users).Returns(mock);
		}

		#region GetAllUsers

		[Fact]
		public async Task GetAllUsers_ReturnsEmptyList_WhenNoUsersExist()
		{
			SetupUsers();
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());

			var result = await _service.GetAllUsers(new UserFilter());

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllUsers_ReturnsAllUsers_WhenNoFilterApplied()
		{
			var users = new[] { MakeUser("u1", "alice"), MakeUser("u2", "bob") };
			SetupUsers(users);
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter());

			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetAllUsers_MarksAdminUsers_Correctly()
		{
			var adminUser = MakeUser("u1", "alice");
			var regularUser = MakeUser("u2", "bob");
			SetupUsers(adminUser, regularUser);

			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser> { adminUser });
			_userManagerMock.Setup(m => m.GetRolesAsync(adminUser))
				.ReturnsAsync(new List<string> { "Admin" });
			_userManagerMock.Setup(m => m.GetRolesAsync(regularUser))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter());

			Assert.True(result.First(x => x.Id == "u1").IsAdmin);
			Assert.False(result.First(x => x.Id == "u2").IsAdmin);
		}

		[Fact]
		public async Task GetAllUsers_FiltersByBanned_WhenIsBannedIsTrue()
		{
			var banned = MakeUser("u1", isBanned: true);
			var notBanned = MakeUser("u2", isBanned: false);
			SetupUsers(banned, notBanned);
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter { IsBanned = true });

			Assert.Single(result);
			Assert.Equal("u1", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_FiltersByNotBanned_WhenIsBannedIsFalse()
		{
			var banned = MakeUser("u1", isBanned: true);
			var notBanned = MakeUser("u2", isBanned: false);
			SetupUsers(banned, notBanned);
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter { IsBanned = false });

			Assert.Single(result);
			Assert.Equal("u2", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_FiltersByDeactivated_WhenIsDeactivatedIsTrue()
		{
			var deactivated = MakeUser("u1", isDeactivated: true);
			var active = MakeUser("u2", isDeactivated: false);
			SetupUsers(deactivated, active);
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter { IsDeactivated = true });

			Assert.Single(result);
			Assert.Equal("u1", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_CountsPendingCiphers_Correctly()
		{
			var ciphers = new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Pending),
				MakeCipher(2, ApprovalStatus.Pending),
				MakeCipher(3, ApprovalStatus.Approved),
			};
			var user = MakeUser("u1", uploadedCiphers: ciphers);
			SetupUsers(user);
			_userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetAllUsers(new UserFilter());

			Assert.Equal(2, result[0].PendingCiphers);
		}

		#endregion

		#region GetUser

		[Fact]
		public async Task GetUser_Throws_WhenUserNotFound()
		{
			SetupUsers();

			await Assert.ThrowsAsync<NotFoundException>(() => _service.GetUser("ghost"));
		}

		[Fact]
		public async Task GetUser_ReturnsCorrectBasicFields()
		{
			var user = MakeUser("u1", "alice", "alice@test.com");
			user.RegisteredAt = DateTime.UtcNow.AddDays(-10);
			user.Score = 500;
			user.SolvedCount = 3;
			SetupUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
						.ReturnsAsync(new List<string>());

			var result = await _service.GetUser("u1");

			Assert.Equal("u1", result.Id);
			Assert.Equal("alice", result.Username);
			Assert.Equal("alice@test.com", result.Email);
			Assert.Equal(500, result.TotalScore);
			Assert.Equal(3, result.CiphersSolved);
			Assert.False(result.IsAdmin);
		}

		[Fact]
		public async Task GetUser_ReturnsIsAdminTrue_WhenUserIsAdmin()
		{
			var user = MakeUser("u1");
			SetupUsers(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			var result = await _service.GetUser("u1");

			Assert.True(result.IsAdmin);
		}

		[Fact]
		public async Task GetUser_CountsSubmittedAndApprovedCiphers_Correctly()
		{
			var ciphers = new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
				MakeCipher(2, ApprovalStatus.Pending),
				MakeCipher(3, ApprovalStatus.Pending),
			};
			var user = MakeUser("u1", uploadedCiphers: ciphers);
			SetupUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
						.ReturnsAsync(new List<string>());

			var result = await _service.GetUser("u1");

			Assert.Equal(3, result.CiphersSubmitted);
			Assert.Equal(1, result.ApprovedCiphers);
			Assert.Equal(2, result.PendingCiphers);
		}

		[Fact]
		public async Task GetUser_PopulatesSolvedCiphers_FromUserSolutions()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			var solutions = new List<UserSolution>
			{
				new() { Cipher = cipher },
			};
			var user = MakeUser("u1", cipherAnswers: solutions);
			SetupUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
						.ReturnsAsync(new List<string>());

			var result = await _service.GetUser("u1");

			Assert.Single(result.SolvedCiphers);
			Assert.Equal(1, result.SolvedCiphers[0].Id);
		}

		[Fact]
		public async Task GetUser_ReturnsBanDetails_WhenUserIsBanned()
		{
			var bannedAt = DateTime.UtcNow.AddDays(-1);
			var user = MakeUser("u1", isBanned: true);
			user.BanReason = "cheating";
			user.BannedAt = bannedAt;
			SetupUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
						.ReturnsAsync(new List<string>());

			var result = await _service.GetUser("u1");

			Assert.True(result.IsBanned);
			Assert.Equal("cheating", result.BanReason);
			Assert.Equal(bannedAt, result.BannedAt);
		}

		#endregion

		#region MakeAdmin

		[Fact]
		public async Task MakeAdmin_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.MakeAdmin("ghost"));
		}

		[Fact]
		public async Task MakeAdmin_Throws_WhenUserIsAlreadyAdmin()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(() => _service.MakeAdmin("u1"));
		}

		[Fact]
		public async Task MakeAdmin_AddsAdminRole_WhenUserIsNotAdmin()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

			await _service.MakeAdmin("u1");

			_userManagerMock.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
		}

		#endregion

		#region BanUserAsync

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.BanUserAsync("ghost", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsDeactivated()
		{
			var user = MakeUser("u1", isDeactivated: true);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => _service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsAlreadyBanned()
		{
			var user = MakeUser("u1", isBanned: true);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => _service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsAdmin()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(() => _service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_SetsBanFields_OnUser()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

			await _service.BanUserAsync("u1", "rule violation");

			Assert.True(user.IsBanned);
			Assert.Equal("rule violation", user.BanReason);
			Assert.NotNull(user.BannedAt);
		}

		[Fact]
		public async Task BanUserAsync_SetsLockout_OnUser()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

			await _service.BanUserAsync("u1", "reason");

			_userManagerMock.Verify(m => m.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue), Times.Once);
			_userManagerMock.Verify(m => m.SetLockoutEnabledAsync(user, true), Times.Once);
		}

		#endregion

		#region UnbanUserAsync

		[Fact]
		public async Task UnbanUserAsync_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _service.UnbanUserAsync("ghost"));
		}

		[Fact]
		public async Task UnbanUserAsync_Throws_WhenUserIsNotBanned()
		{
			var user = MakeUser("u1", isBanned: false);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => _service.UnbanUserAsync("u1"));
		}

		[Fact]
		public async Task UnbanUserAsync_ClearsBanFields_OnUser()
		{
			var user = MakeUser("u1", isBanned: true);
			user.BanReason = "old reason";
			user.BannedAt = DateTime.UtcNow.AddDays(-5);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.UnbanUserAsync("u1");

			Assert.False(user.IsBanned);
			Assert.Null(user.BanReason);
			Assert.Null(user.BannedAt);
		}

		[Fact]
		public async Task UnbanUserAsync_RemovesLockout_OnUser()
		{
			var user = MakeUser("u1", isBanned: true);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.UnbanUserAsync("u1");

			_userManagerMock.Verify(m => m.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>()), Times.Once);
		}

		#endregion
	}
}