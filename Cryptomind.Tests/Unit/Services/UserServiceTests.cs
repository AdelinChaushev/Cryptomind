using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
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
	public class UserServiceTests
	{
		private readonly Mock<IRepository<ApplicationUser, string>> _userRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly UserService _service;

		public UserServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_service = new UserService(
				_userRepoMock.Object,
				_userManagerMock.Object);
		}

		private static ApplicationUser MakeUser(
			string id,
			string userName = "testuser",
			string email = "test@test.com",
			int score = 100,
			int solvedCount = 5,
			List<UserBadge>? badges = null) => new()
			{
				Id = id,
				UserName = userName,
				Email = email,
				Score = score,
				SolvedCount = solvedCount,
				RegisteredAt = DateTime.UtcNow,
				Badges = badges ?? new List<UserBadge>(),
			};

		private static Badge MakeBadge(int id, string title, string description = "Badge description") => new()
		{
			Id = id,
			Title = title,
			Description = description,
			EarnedBy = 10,
		};

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			_userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region GetRolesUsers

		[Fact]
		public async Task GetRolesUsers_Throws_WhenUserNotFound()
		{
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser)null);

			await Assert.ThrowsAsync<NullReferenceException>(
				() => _service.GetRolesUsers("ghost"));
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsRoles_WhenUserExists()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User", "Admin" });

			var result = await _service.GetRolesUsers("u1");

			Assert.Equal(2, result.Count());
			Assert.Contains("User", result);
			Assert.Contains("Admin", result);
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsEmpty_WhenUserHasNoRoles()
		{
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetRolesUsers("u1");

			Assert.Empty(result);
		}

		#endregion

		#region GetUserAccountInfo

		[Fact]
		public async Task GetUserAccountInfo_Throws_WhenUserNotFound()
		{
			SetupAttachedUsers();

			await Assert.ThrowsAsync<NullReferenceException>(
				() => _service.GetUserAccountInfo("ghost"));
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsAccountInfo_WithBasicFields()
		{
			var user = MakeUser("u1", userName: "alice", email: "alice@test.com", score: 500, solvedCount: 10);
			SetupAttachedUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			var result = await _service.GetUserAccountInfo("u1");

			Assert.NotNull(result);
			Assert.Equal("alice", result.Username);
			Assert.Equal("alice@test.com", result.Email);
			Assert.Equal(500, result.Score);
			Assert.Equal(10, result.SolvedCount);
		}

		[Fact]
		public async Task GetUserAccountInfo_PopulatesRoles()
		{
			var user = MakeUser("u1");
			SetupAttachedUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User", "Admin" });

			var result = await _service.GetUserAccountInfo("u1");

			Assert.Equal(2, result.Roles.Length);
			Assert.Contains("User", result.Roles);
			Assert.Contains("Admin", result.Roles);
		}

		[Fact]
		public async Task GetUserAccountInfo_PopulatesBadges_WhenUserHasBadges()
		{
			var badge1 = MakeBadge(1, "First Solve");
			var badge2 = MakeBadge(2, "Master Solver");
			var userBadges = new List<UserBadge>
			{
				new() { Badge = badge1 },
				new() { Badge = badge2 }
			};
			var user = MakeUser("u1", badges: userBadges);
			SetupAttachedUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetUserAccountInfo("u1");

			Assert.Equal(2, result.Badges.Count);
			Assert.Contains(result.Badges, b => b.Title == "First Solve");
			Assert.Contains(result.Badges, b => b.Title == "Master Solver");
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsEmptyBadges_WhenUserHasNoBadges()
		{
			var user = MakeUser("u1");
			SetupAttachedUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetUserAccountInfo("u1");

			Assert.Empty(result.Badges);
		}

		[Fact]
		public async Task GetUserAccountInfo_IncludesBadgeEarnedByCount()
		{
			var badge = MakeBadge(1, "Popular Badge");
			badge.EarnedBy = 50;
			var userBadges = new List<UserBadge>
			{
				new() { Badge = badge }
			};
			var user = MakeUser("u1", badges: userBadges);
			SetupAttachedUsers(user);
			_userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await _service.GetUserAccountInfo("u1");

			Assert.Single(result.Badges);
			Assert.Equal(50, result.Badges.First().EarnedBy);
		}

		#endregion
	}
}