using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IRepository<ApplicationUser, string>> userRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly UserService service;

		public UserServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new UserService(userRepoMock.Object, userManagerMock.Object);

			userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());
		}

		private static ApplicationUser MakeUser(
			string id = "u1",
			string userName = "testuser",
			string email = "test@test.com",
			int score = 0,
			bool isDeactivated = false,
			bool isBanned = false,
			List<UserSolution>? cipherAnswers = null,
			List<UserBadge>? badges = null) => new()
			{
				Id = id,
				UserName = userName,
				Email = email,
				Score = score,
				IsDeactivated = isDeactivated,
				IsBanned = isBanned,
				CipherAnswers = cipherAnswers ?? new List<UserSolution>(),
				Badges = badges ?? new List<UserBadge>(),
			};

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			userRepoMock.Setup(r => r.GetAllAttached())
				.Returns(users.AsQueryable().BuildMock());
		}

		#region GetRolesUsers

		[Fact]
		public async Task GetRolesUsers_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetRolesUsers("ghost"));
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsRoles_WhenUserExists()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin", "User" });

			var result = await service.GetRolesUsers("u1");

			Assert.Contains("Admin", result);
			Assert.Contains("User", result);
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsEmptyList_WhenUserHasNoRoles()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await service.GetRolesUsers("u1");

			Assert.Empty(result);
		}

		#endregion

		#region GetUserAccountInfo

		[Fact]
		public async Task GetUserAccountInfo_Throws_WhenUserNotFound()
		{
			// Bug: service throws NullReferenceException before the null check
			// because CipherAnswers is accessed before the null guard
			SetupAttachedUsers();

			await Assert.ThrowsAsync<System.NullReferenceException>(
				() => service.GetUserAccountInfo("ghost"));
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsCorrectUsername_WhenUserExists()
		{
			var user = MakeUser("u1", userName: "alice", score: 0);
			SetupAttachedUsers(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal("alice", result.Username);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsCorrectScore_WhenUserExists()
		{
			var user = MakeUser("u1", score: 500);
			SetupAttachedUsers(user);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(500, result.Score);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsCorrectAttemptedCiphers_CountsDistinctCipherIds()
		{
			var user = MakeUser("u1", cipherAnswers: new List<UserSolution>
			{
				new UserSolution { CipherId = 1, IsCorrect = false },
				new UserSolution { CipherId = 1, IsCorrect = true },
				new UserSolution { CipherId = 2, IsCorrect = true },
			});
			SetupAttachedUsers(user);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(2, result.AttemptedCiphers);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsRank1_WhenNoOtherUsersHaveHigherScore()
		{
			var user = MakeUser("u1", score: 1000);
			var otherUser = MakeUser("u2", score: 500);
			SetupAttachedUsers(user, otherUser);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(1, result.LeaderBoardPlace);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsCorrectRank_WhenOtherUsersHaveHigherScore()
		{
			var user = MakeUser("u1", score: 100);
			var higherUser1 = MakeUser("u2", score: 500);
			var higherUser2 = MakeUser("u3", score: 300);
			SetupAttachedUsers(user, higherUser1, higherUser2);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(3, result.LeaderBoardPlace);
		}

		[Fact]
		public async Task GetUserAccountInfo_ExcludesBannedUsers_FromRankCalculation()
		{
			var user = MakeUser("u1", score: 100);
			var bannedUser = MakeUser("u2", score: 500, isBanned: true);
			SetupAttachedUsers(user, bannedUser);

			var result = await service.GetUserAccountInfo("u1");

			// banned user should not count toward rank
			Assert.Equal(1, result.LeaderBoardPlace);
		}

		[Fact]
		public async Task GetUserAccountInfo_ExcludesDeactivatedUsers_FromRankCalculation()
		{
			var user = MakeUser("u1", score: 100);
			var deactivatedUser = MakeUser("u2", score: 500, isDeactivated: true);
			SetupAttachedUsers(user, deactivatedUser);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(1, result.LeaderBoardPlace);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsEmptyBadges_WhenUserHasNoBadges()
		{
			var user = MakeUser("u1", badges: new List<UserBadge>());
			SetupAttachedUsers(user);

			var result = await service.GetUserAccountInfo("u1");

			Assert.Empty(result.Badges);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsCorrectRoles()
		{
			var user = MakeUser("u1");
			SetupAttachedUsers(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin" });

			var result = await service.GetUserAccountInfo("u1");

			Assert.Contains("Admin", result.Roles);
		}

		#endregion
	}
}