using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Contracts;
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
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly AdminUserService service;

		public AdminUserServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new AdminUserService(userManagerMock.Object);

			userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());
			userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser>());
			userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
				.ReturnsAsync(false);
			userManagerMock.Setup(m => m.SetLockoutEndDateAsync(It.IsAny<ApplicationUser>(), It.IsAny<DateTimeOffset?>()))
				.ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()))
				.ReturnsAsync(IdentityResult.Success);
		}

		private static ApplicationUser MakeUser(
			string id = "u1",
			string userName = "testuser",
			string email = "test@test.com",
			bool isBanned = false,
			bool isDeactivated = false,
			List<Cipher>? uploadedCiphers = null,
			List<UserSolution>? cipherAnswers = null) => new()
			{
				Id = id,
				UserName = userName,
				Email = email,
				IsBanned = isBanned,
				IsDeactivated = isDeactivated,
				UploadedCiphers = uploadedCiphers ?? new List<Cipher>(),
				CipherAnswers = cipherAnswers ?? new List<UserSolution>(),
				HintsRequested = new List<HintRequest>(),
			};

		private static UserFilter NoFilter() => new UserFilter
		{
			Username = null,
			IsBanned = null,
			IsDeactivated = null,
		};

		private void SetupUsers(params ApplicationUser[] users)
		{
			userManagerMock.Setup(m => m.Users)
				.Returns(users.AsQueryable().BuildMock());
		}

		#region GetAllUsers

		[Fact]
		public async Task GetAllUsers_ReturnsEmptyList_WhenNoUsersExist()
		{
			SetupUsers();

			var result = await service.GetAllUsers(NoFilter());

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllUsers_ReturnsAllUsers_WhenNoFilterApplied()
		{
			SetupUsers(MakeUser("u1", "alice"), MakeUser("u2", "bob"));

			var result = await service.GetAllUsers(NoFilter());

			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetAllUsers_FiltersCorrectly_ByUsername()
		{
			SetupUsers(MakeUser("u1", "alice"), MakeUser("u2", "bob"));

			var result = await service.GetAllUsers(new UserFilter { Username = "ali" });

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task GetAllUsers_UsernameFilter_IsCaseInsensitive()
		{
			SetupUsers(MakeUser("u1", "Alice"), MakeUser("u2", "bob"));

			var result = await service.GetAllUsers(new UserFilter { Username = "ALICE" });

			Assert.Single(result);
		}

		[Fact]
		public async Task GetAllUsers_FiltersCorrectly_ByIsBanned_True()
		{
			SetupUsers(
				MakeUser("u1", isBanned: true),
				MakeUser("u2", isBanned: false));

			var result = await service.GetAllUsers(new UserFilter { IsBanned = true });

			Assert.Single(result);
			Assert.Equal("u1", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_FiltersCorrectly_ByIsBanned_False()
		{
			SetupUsers(
				MakeUser("u1", isBanned: true),
				MakeUser("u2", isBanned: false));

			var result = await service.GetAllUsers(new UserFilter { IsBanned = false });

			Assert.Single(result);
			Assert.Equal("u2", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_NoIsBannedFilter_WhenNull()
		{
			SetupUsers(
				MakeUser("u1", isBanned: true),
				MakeUser("u2", isBanned: false));

			var result = await service.GetAllUsers(new UserFilter { IsBanned = null });

			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetAllUsers_FiltersCorrectly_ByIsDeactivated_True()
		{
			SetupUsers(
				MakeUser("u1", isDeactivated: true),
				MakeUser("u2", isDeactivated: false));

			var result = await service.GetAllUsers(new UserFilter { IsDeactivated = true });

			Assert.Single(result);
			Assert.Equal("u1", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_FiltersCorrectly_ByIsDeactivated_False()
		{
			SetupUsers(
				MakeUser("u1", isDeactivated: true),
				MakeUser("u2", isDeactivated: false));

			var result = await service.GetAllUsers(new UserFilter { IsDeactivated = false });

			Assert.Single(result);
			Assert.Equal("u2", result[0].Id);
		}

		[Fact]
		public async Task GetAllUsers_NoIsDeactivatedFilter_WhenNull()
		{
			SetupUsers(
				MakeUser("u1", isDeactivated: true),
				MakeUser("u2", isDeactivated: false));

			var result = await service.GetAllUsers(new UserFilter { IsDeactivated = null });

			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetAllUsers_MarksAdminCorrectly()
		{
			var adminUser = MakeUser("u1", "adminuser");
			SetupUsers(adminUser);
			userManagerMock.Setup(m => m.GetUsersInRoleAsync("Admin"))
				.ReturnsAsync(new List<ApplicationUser> { adminUser });
			userManagerMock.Setup(m => m.GetRolesAsync(adminUser))
				.ReturnsAsync(new List<string> { "Admin" });

			var result = await service.GetAllUsers(NoFilter());

			Assert.True(result[0].IsAdmin);
			Assert.Equal("Admin", result[0].Role);
		}

		[Fact]
		public async Task GetAllUsers_CountsPendingCiphersCorrectly()
		{
			var ciphers = new List<Cipher>
			{
				new TextCipher { Id = 1, Status = ApprovalStatus.Pending, CipherTags = new List<CipherTag>(), AnswerSuggestions = new List<AnswerSuggestion>(), MLPrediction = "" },
				new TextCipher { Id = 2, Status = ApprovalStatus.Approved, CipherTags = new List<CipherTag>(), AnswerSuggestions = new List<AnswerSuggestion>(), MLPrediction = "" },
			};
			SetupUsers(MakeUser("u1", uploadedCiphers: ciphers));

			var result = await service.GetAllUsers(NoFilter());

			Assert.Equal(1, result[0].PendingCiphers);
		}

		#endregion

		#region GetUser

		[Fact]
		public async Task GetUser_Throws_WhenUserNotFound()
		{
			SetupUsers();

			await Assert.ThrowsAsync<NotFoundException>(() => service.GetUser("ghost"));
		}

		[Fact]
		public async Task GetUser_ReturnsCorrectBasicInfo()
		{
			var user = MakeUser("u1", "alice", "alice@test.com");
			SetupUsers(user);

			var result = await service.GetUser("u1");

			Assert.Equal("u1", result.Id);
			Assert.Equal("alice", result.Username);
			Assert.Equal("alice@test.com", result.Email);
		}

		[Fact]
		public async Task GetUser_ReturnsCorrectSubmittedCiphersCount()
		{
			var ciphers = new List<Cipher>
			{
				new TextCipher { Id = 1, Title = "C1", Status = ApprovalStatus.Approved, CipherTags = new List<CipherTag>(), AnswerSuggestions = new List<AnswerSuggestion>(), MLPrediction = "" },
				new TextCipher { Id = 2, Title = "C2", Status = ApprovalStatus.Pending, CipherTags = new List<CipherTag>(), AnswerSuggestions = new List<AnswerSuggestion>(), MLPrediction = "" },
			};
			SetupUsers(MakeUser("u1", uploadedCiphers: ciphers));

			var result = await service.GetUser("u1");

			Assert.Equal(2, result.CiphersSubmitted);
			Assert.Equal(2, result.SubmittedCiphers.Count);
		}

		[Fact]
		public async Task GetUser_ReturnsCorrectSolvedCiphers()
		{
			var cipher = new TextCipher
			{
				Id = 1,
				Title = "Solved",
				Status = ApprovalStatus.Approved,
				CipherTags = new List<CipherTag>(),
				AnswerSuggestions = new List<AnswerSuggestion>(),
				MLPrediction = ""
			};
			var solution = new UserSolution { Cipher = cipher };
			SetupUsers(MakeUser("u1", cipherAnswers: new List<UserSolution> { solution }));

			var result = await service.GetUser("u1");

			Assert.Single(result.SolvedCiphers);
			Assert.Equal("Solved", result.SolvedCiphers[0].Title);
		}

		[Fact]
		public async Task GetUser_MarksAdminCorrectly()
		{
			var user = MakeUser("u1");
			SetupUsers(user);
			userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin" });

			var result = await service.GetUser("u1");

			Assert.True(result.IsAdmin);
			Assert.Equal("Admin", result.Role);
		}

		[Fact]
		public async Task GetUser_ReturnsBanInfo_WhenUserIsBanned()
		{
			var bannedAt = new DateTime(2025, 1, 1);
			var user = MakeUser("u1", isBanned: true);
			user.BanReason = "cheating";
			user.BannedAt = bannedAt;
			SetupUsers(user);

			var result = await service.GetUser("u1");

			Assert.True(result.IsBanned);
			Assert.Equal("cheating", result.BanReason);
			Assert.Equal(bannedAt, result.BannedAt);
		}

		#endregion

		#region MakeAdmin

		[Fact]
		public async Task MakeAdmin_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => service.MakeAdmin("ghost"));
		}

		[Fact]
		public async Task MakeAdmin_Throws_WhenUserIsAlreadyAdmin()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(() => service.MakeAdmin("u1"));
		}

		[Fact]
		public async Task MakeAdmin_AddsAdminRole_WhenUserIsNotAdmin()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
			userManagerMock.Setup(m => m.AddToRoleAsync(user, "Admin"))
				.ReturnsAsync(IdentityResult.Success);

			await service.MakeAdmin("u1");

			userManagerMock.Verify(m => m.AddToRoleAsync(user, "Admin"), Times.Once);
		}

		#endregion

		#region BanUserAsync

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => service.BanUserAsync("ghost", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsDeactivated()
		{
			var user = MakeUser("u1", isDeactivated: true);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsAlreadyBanned()
		{
			var user = MakeUser("u1", isBanned: true);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_Throws_WhenUserIsAdmin()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			await Assert.ThrowsAsync<ConflictException>(() => service.BanUserAsync("u1", "reason"));
		}

		[Fact]
		public async Task BanUserAsync_SetsIsBannedAndReason()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.BanUserAsync("u1", "rule violation");

			Assert.True(user.IsBanned);
			Assert.Equal("rule violation", user.BanReason);
			Assert.NotNull(user.BannedAt);
		}

		[Fact]
		public async Task BanUserAsync_EnablesLockout()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.BanUserAsync("u1", "reason");

			userManagerMock.Verify(m => m.SetLockoutEnabledAsync(user, true), Times.Once);
			userManagerMock.Verify(m => m.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue), Times.Once);
		}

		#endregion

		#region UnbanUserAsync

		[Fact]
		public async Task UnbanUserAsync_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => service.UnbanUserAsync("ghost"));
		}

		[Fact]
		public async Task UnbanUserAsync_Throws_WhenUserIsNotBanned()
		{
			var user = MakeUser("u1", isBanned: false);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await Assert.ThrowsAsync<ConflictException>(() => service.UnbanUserAsync("u1"));
		}

		[Fact]
		public async Task UnbanUserAsync_ClearsIsBannedAndReason()
		{
			var user = MakeUser("u1", isBanned: true);
			user.BanReason = "rule violation";
			user.BannedAt = DateTime.Now.AddDays(-1);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.UnbanUserAsync("u1");

			Assert.False(user.IsBanned);
			Assert.Null(user.BanReason);
			Assert.Null(user.BannedAt);
		}

		[Fact]
		public async Task UnbanUserAsync_RemovesLockout()
		{
			var user = MakeUser("u1", isBanned: true);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.UnbanUserAsync("u1");

			userManagerMock.Verify(m =>
				m.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>()), Times.Once);
		}

		#endregion
	}
}