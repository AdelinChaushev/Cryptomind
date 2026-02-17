using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Services
{
	public class BadgeServiceTests
	{
		private readonly Mock<IRepository<UserBadge, int>> _userBadgeRepoMock = new();
		private readonly Mock<IRepository<ApplicationUser, string>> _userRepoMock = new();
		private readonly Mock<IRepository<Badge, int>> _badgeRepoMock = new();
		private readonly Mock<IBadgeStatisticsService> _statsServiceMock = new();
		private readonly Mock<INotificationService> _notificationMock = new();
		private readonly BadgeService _service;

		public BadgeServiceTests()
		{
			_service = new BadgeService(
				_userBadgeRepoMock.Object,
				_userRepoMock.Object,
				_badgeRepoMock.Object,
				_statsServiceMock.Object,
				_notificationMock.Object);

			_userBadgeRepoMock.Setup(r => r.AddAsync(It.IsAny<UserBadge>()))
				.Returns(Task.CompletedTask);
			_notificationMock.Setup(n => n.CreateAndSendNotification(
				It.IsAny<string>(), It.IsAny<NotificationType>(),
				It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);
		}

		private static ApplicationUser MakeUser(string id) => new()
		{
			Id = id,
			Badges = new List<UserBadge>(),
		};

		private static Badge MakeBadge(int id, string title = "Test Badge") => new()
		{
			Id = id,
			Title = title,
			EarnedBy = 0,
			UserBadges = new List<UserBadge>(),
		};

		private void SetupUserBadgeIds(string userId, params int[] badgeIds)
		{
			var userBadges = badgeIds
				.Select(id => new UserBadge { UserId = userId, BadgeId = id })
				.ToList();
			var mock = userBadges.AsQueryable().BuildMock();
			_userBadgeRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			_userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedBadges(params Badge[] badges)
		{
			var mock = new List<Badge>(badges).AsQueryable().BuildMock();
			_badgeRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region CheckBadgesByCategory

		[Fact]
		public async Task CheckBadgesByCategory_SkipsBadge_WhenUserAlreadyHasIt()
		{
			SetupUserBadgeIds("u1", 1, 2);

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_statsServiceMock.Verify(s => s.GetSolvedCount(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge_WhenCriteriaIsSatisfied()
		{
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge_WhenCriteriaIsNotSatisfied()
		{
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(0);

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_userBadgeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserBadge>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBothOnSolveBadges_WhenBothCriteriaAreSatisfied()
		{
			// Badge 1 requires 1 solve, badge 2 requires 25 solves
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsOnlyFirstBadge_WhenOnlyFirstCriteriaIsSatisfied()
		{
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_SkipsAlreadyEarnedBadge_ButAwardsNewOne()
		{
			// User already has badge 1, qualifies for badge 2
			SetupUserBadgeIds("u1", 1);
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(2));

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Never);
			_userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_SendsNotification_WhenBadgeIsAwarded()
		{
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1, "First Solve"));

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.BadgeEarned,
				It.IsAny<string>(),
				1,
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_IncrementsBadgeEarnedByCount_WhenAwarded()
		{
			SetupUserBadgeIds("u1");
			_statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			var badge = MakeBadge(1);
			SetupAttachedBadges(badge);

			await _service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			Assert.Equal(1, badge.EarnedBy);
		}

		#endregion
	}

	public class BadgeStatisticsServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IRepository<ApplicationUser, string>> _userRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> _solutionRepoMock = new();
		private readonly BadgeStatisticsService _service;

		public BadgeStatisticsServiceTests()
		{
			_service = new BadgeStatisticsService(
				_cipherRepoMock.Object,
				_userRepoMock.Object,
				_solutionRepoMock.Object);
		}

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			_userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedSolutions(params UserSolution[] solutions)
		{
			var mock = new List<UserSolution>(solutions).AsQueryable().BuildMock();
			_solutionRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region GetApprovedCount

		[Fact]
		public async Task GetApprovedCount_ReturnsOnlyApprovedCiphers_ForGivenUser()
		{
			var user = new ApplicationUser
			{
				Id = "u1",
				UploadedCiphers = new List<Cipher>
				{
					new ConcreteCipher { Id = 1, Status = ApprovalStatus.Approved },
					new ConcreteCipher { Id = 2, Status = ApprovalStatus.Approved },
					new ConcreteCipher { Id = 3, Status = ApprovalStatus.Pending },
				}
			};
			SetupAttachedUsers(user);

			var result = await _service.GetApprovedCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetApprovedCount_ReturnsZero_WhenUserHasNoApprovedCiphers()
		{
			var user = new ApplicationUser { Id = "u1", UploadedCiphers = new List<Cipher>() };
			SetupAttachedUsers(user);

			var result = await _service.GetApprovedCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetDistinctCipherTypesSolved

		[Fact]
		public async Task GetDistinctCipherTypesSolved_ReturnsDistinctTypeCount()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar } },
				new UserSolution { UserId = "u1", Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar } },
				new UserSolution { UserId = "u1", Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Vigenere } },
			};
			SetupAttachedSolutions(solutions);

			var result = await _service.GetDistinctCipherTypesSolved("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetDistinctCipherTypesSolved_ReturnsZero_WhenUserHasNoSolutions()
		{
			SetupAttachedSolutions();

			var result = await _service.GetDistinctCipherTypesSolved("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetDistinctCipherTypesSolved_OnlyCountsForGivenUser()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar } },
				new UserSolution { UserId = "u2", Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Vigenere } },
			};
			SetupAttachedSolutions(solutions);

			var result = await _service.GetDistinctCipherTypesSolved("u1");

			Assert.Equal(1, result);
		}

		#endregion

		#region GetApprovedAnswersCount

		[Fact]
		public async Task GetApprovedAnswersCount_ReturnsOnlyApprovedAnswers_ForGivenUser()
		{
			var user = new ApplicationUser
			{
				Id = "u1",
				SuggestedAnswers = new List<AnswerSuggestion>
				{
					new() { Status = ApprovalStatus.Approved },
					new() { Status = ApprovalStatus.Approved },
					new() { Status = ApprovalStatus.Pending },
					new() { Status = ApprovalStatus.Rejected },
				}
			};
			SetupAttachedUsers(user);

			var result = await _service.GetApprovedAnswersCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetApprovedAnswersCount_ReturnsZero_WhenUserHasNoApprovedAnswers()
		{
			var user = new ApplicationUser { Id = "u1", SuggestedAnswers = new List<AnswerSuggestion>() };
			SetupAttachedUsers(user);

			var result = await _service.GetApprovedAnswersCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetSolvedCount

		[Fact]
		public async Task GetSolvedCount_Throws_WhenUserNotFound()
		{
			_userRepoMock.Setup(r => r.GetByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetSolvedCount("ghost"));
		}

		[Fact]
		public async Task GetSolvedCount_ReturnsUserSolvedCount()
		{
			var user = new ApplicationUser { Id = "u1", SolvedCount = 7 };
			_userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

			var result = await _service.GetSolvedCount("u1");

			Assert.Equal(7, result);
		}

		#endregion
	}
}