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
	public class LeaderboardServiceTests
	{
		private readonly Mock<IRepository<ApplicationUser, string>> userRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly LeaderboardService service;

		public LeaderboardServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(new List<string>());

			service = new LeaderboardService(userRepoMock.Object, userManagerMock.Object);
		}

		private static ApplicationUser MakeUser(
			string id,
			string userName,
			int score,
			bool isBanned = false,
			bool isDeactivated = false) => new()
			{
				Id = id,
				UserName = userName,
				Score = score,
				IsBanned = isBanned,
				IsDeactivated = isDeactivated,
			};

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			userRepoMock.Setup(r => r.GetAllAttached())
				.Returns(users.AsQueryable().BuildMock());
		}

		private void SetupAdminUser(ApplicationUser user)
		{
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "Admin" });
		}

		#region GetLeaderboard

		[Fact]
		public async Task GetLeaderboard_ReturnsEmptyList_WhenNoUsersExist()
		{
			SetupAttachedUsers();

			var result = await service.GetLeaderboard();

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsSingleUser_WithPlaceOne()
		{
			SetupAttachedUsers(MakeUser("u1", "alice", 100));

			var result = await service.GetLeaderboard();

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
			Assert.Equal(100, result[0].Points);
			Assert.Equal(1, result[0].Place);
		}

		[Fact]
		public async Task GetLeaderboard_OrdersUsersByScore_Descending()
		{
			SetupAttachedUsers(
				MakeUser("u1", "alice", 50),
				MakeUser("u2", "bob", 150),
				MakeUser("u3", "charlie", 100));

			var result = await service.GetLeaderboard();

			Assert.Equal("bob", result[0].Username);
			Assert.Equal("charlie", result[1].Username);
			Assert.Equal("alice", result[2].Username);
		}

		[Fact]
		public async Task GetLeaderboard_AssignsConsecutivePlaces()
		{
			SetupAttachedUsers(
				MakeUser("u1", "first", 300),
				MakeUser("u2", "second", 200),
				MakeUser("u3", "third", 100));

			var result = await service.GetLeaderboard();

			Assert.Equal(1, result[0].Place);
			Assert.Equal(2, result[1].Place);
			Assert.Equal(3, result[2].Place);
		}

		[Fact]
		public async Task GetLeaderboard_AssignsDifferentPlaces_WhenScoresAreTied()
		{
			SetupAttachedUsers(
				MakeUser("u1", "alice", 100),
				MakeUser("u2", "bob", 100),
				MakeUser("u3", "charlie", 50));

			var result = await service.GetLeaderboard();

			Assert.Equal(1, result[0].Place);
			Assert.Equal(2, result[1].Place);
			Assert.Equal(3, result[2].Place);
		}

		[Fact]
		public async Task GetLeaderboard_HandlesZeroScores()
		{
			SetupAttachedUsers(
				MakeUser("u1", "alice", 0),
				MakeUser("u2", "bob", 0));

			var result = await service.GetLeaderboard();

			Assert.Equal(2, result.Count);
			Assert.Equal(0, result[0].Points);
			Assert.Equal(0, result[1].Points);
		}

		[Fact]
		public async Task GetLeaderboard_ExcludesBannedUsers()
		{
			SetupAttachedUsers(
				MakeUser("u1", "alice", 100),
				MakeUser("u2", "banned", 500, isBanned: true));

			var result = await service.GetLeaderboard();

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task GetLeaderboard_ExcludesDeactivatedUsers()
		{
			SetupAttachedUsers(
				MakeUser("u1", "alice", 100),
				MakeUser("u2", "deactivated", 500, isDeactivated: true));

			var result = await service.GetLeaderboard();

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task GetLeaderboard_ExcludesAdmins()
		{
			var admin = MakeUser("u1", "admin", 1000);
			var user = MakeUser("u2", "alice", 100);
			SetupAttachedUsers(admin, user);
			SetupAdminUser(admin);

			var result = await service.GetLeaderboard();

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task GetLeaderboard_LimitsResultsTo20()
		{
			var users = Enumerable.Range(1, 25)
				.Select(i => MakeUser($"u{i}", $"user{i}", 100 - i))
				.ToArray();
			SetupAttachedUsers(users);

			var result = await service.GetLeaderboard();

			Assert.Equal(20, result.Count);
		}

		[Fact]
		public async Task GetLeaderboard_PlacesReflectPositionAfterFiltering_WhenAdminIsRemoved()
		{
			var admin = MakeUser("u1", "admin", 1000);
			var first = MakeUser("u2", "first", 300);
			var second = MakeUser("u3", "second", 200);
			SetupAttachedUsers(admin, first, second);
			SetupAdminUser(admin);

			var result = await service.GetLeaderboard();

			Assert.Equal(2, result.Count);
			Assert.Equal(1, result[0].Place);
			Assert.Equal(2, result[1].Place);
		}

		#endregion
	}
}