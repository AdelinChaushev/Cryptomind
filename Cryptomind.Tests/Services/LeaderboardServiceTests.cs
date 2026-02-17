using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Services
{
	public class LeaderboardServiceTests
	{
		private readonly Mock<IRepository<ApplicationUser, string>> _userRepoMock = new();
		private readonly LeaderboardService _service;

		public LeaderboardServiceTests()
		{
			_service = new LeaderboardService(_userRepoMock.Object);
		}

		private static ApplicationUser MakeUser(string id, string userName, int score) => new()
		{
			Id = id,
			UserName = userName,
			Score = score,
		};

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			_userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsEmptyList_WhenNoUsersExist()
		{
			SetupAttachedUsers();

			var result = await _service.GetLeaderboard();

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsSingleUser_WithPlaceOne()
		{
			SetupAttachedUsers(MakeUser("u1", "alice", 100));

			var result = await _service.GetLeaderboard();

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

			var result = await _service.GetLeaderboard();

			Assert.Equal(3, result.Count);
			Assert.Equal("bob", result[0].Username);
			Assert.Equal(150, result[0].Points);
			Assert.Equal("charlie", result[1].Username);
			Assert.Equal(100, result[1].Points);
			Assert.Equal("alice", result[2].Username);
			Assert.Equal(50, result[2].Points);
		}

		[Fact]
		public async Task GetLeaderboard_AssignsConsecutivePlaces()
		{
			SetupAttachedUsers(
				MakeUser("u1", "first", 300),
				MakeUser("u2", "second", 200),
				MakeUser("u3", "third", 100));

			var result = await _service.GetLeaderboard();

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

			var result = await _service.GetLeaderboard();

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

			var result = await _service.GetLeaderboard();

			Assert.Equal(2, result.Count);
			Assert.Equal(0, result[0].Points);
			Assert.Equal(0, result[1].Points);
		}
	}
}