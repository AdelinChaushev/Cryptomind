using Cryptomind.Common.Exceptions;
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
		private readonly Mock<IRepository<ApplicationUser, string>> userRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly UserService service;

		public UserServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new UserService(
				userRepoMock.Object,
				userManagerMock.Object);
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

		private static Badge MakeBadge(int id, string title, string description = "Badge description", int earnedBy = 10) => new()
		{
			Id = id,
			Title = title,
			Description = description,
			EarnedBy = earnedBy,
		};

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = users.AsQueryable().BuildMock();
			userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region GetRolesUsers

		[Fact]
		public async Task GetRolesUsers_Throws_WhenUserNotFound()
		{
			userManagerMock.Setup(m => m.FindByIdAsync("ghost"))
				.ReturnsAsync((ApplicationUser)null!);

			var ex = await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetRolesUsers("ghost"));

			Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsRoles_WhenUserExists()
		{
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User", "Admin" });

			var result = await service.GetRolesUsers("u1");

			Assert.Equal(2, result.Count());
			Assert.Contains("User", result);
			Assert.Contains("Admin", result);
		}

		[Fact]
		public async Task GetRolesUsers_ReturnsEmpty_WhenUserHasNoRoles()
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
			SetupAttachedUsers(); // empty

			var ex = await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetUserAccountInfo("ghost"));

			Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsAccountInfo_WithBasicFields()
		{
			var user = MakeUser("u1", "alice", "alice@test.com", 500, 10);
			SetupAttachedUsers(user);

			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User" });

			var result = await service.GetUserAccountInfo("u1");

			Assert.NotNull(result);
			Assert.Equal("alice", result.Username);
			Assert.Equal("alice@test.com", result.Email);
			Assert.Equal(500, result.Score);
			Assert.Equal(10, result.SolvedCount);
			Assert.Single(result.Roles);
			Assert.Equal("User", result.Roles[0]);
		}

		[Fact]
		public async Task GetUserAccountInfo_PopulatesBadges_WhenUserHasBadges()
		{
			var badge1 = MakeBadge(1, "First Solve");
			var badge2 = MakeBadge(2, "Master Solver");

			var user = MakeUser("u1", badges: new List<UserBadge>
			{
				new() { Badge = badge1 },
				new() { Badge = badge2 }
			});

			SetupAttachedUsers(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(2, result.Badges.Count);
			Assert.Contains(result.Badges, b => b.Title == "First Solve");
			Assert.Contains(result.Badges, b => b.Title == "Master Solver");
		}
		[Fact]
		public async Task GetUserAccountInfo_PopulatesRoles()
		{
			var user = MakeUser("u1");
			SetupAttachedUsers(user);

			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string> { "User", "Admin" });

			var result = await service.GetUserAccountInfo("u1");

			Assert.Equal(2, result.Roles.Length);
			Assert.Contains("User", result.Roles);
			Assert.Contains("Admin", result.Roles);
		}

		[Fact]
		public async Task GetUserAccountInfo_ReturnsEmptyBadges_WhenUserHasNoBadges()
		{
			var user = MakeUser("u1");
			SetupAttachedUsers(user);

			userManagerMock.Setup(m => m.GetRolesAsync(user))
				.ReturnsAsync(new List<string>());

			var result = await service.GetUserAccountInfo("u1");

			Assert.Empty(result.Badges);
		}

		[Fact]
		public async Task GetUserAccountInfo_IncludesBadgeEarnedByCount()
		{
			var badge = MakeBadge(1, "Popular Badge", earnedBy: 50);
			var user = MakeUser("u1", badges: new List<UserBadge>
			{
				new() { Badge = badge }
			});

			SetupAttachedUsers(user);
			userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(Array.Empty<string>());

			var result = await service.GetUserAccountInfo("u1");

			Assert.Single(result.Badges);
			Assert.Equal(50, result.Badges.First().EarnedBy);
		}

		#endregion
	}
}