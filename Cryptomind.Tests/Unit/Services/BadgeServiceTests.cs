using Cryptomind.Common.Exceptions;
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

namespace Cryptomind.Tests.Unit.Services
{
	public class BadgeServiceTests
	{
		private readonly Mock<IRepository<UserBadge, int>> userBadgeRepoMock = new();
		private readonly Mock<IRepository<ApplicationUser, string>> userRepoMock = new();
		private readonly Mock<IRepository<Badge, int>> badgeRepoMock = new();
		private readonly Mock<IBadgeStatisticsService> statsServiceMock = new();
		private readonly Mock<INotificationService> notificationMock = new();
		private readonly BadgeService service;

		public BadgeServiceTests()
		{
			service = new BadgeService(
				userBadgeRepoMock.Object,
				userRepoMock.Object,
				badgeRepoMock.Object,
				statsServiceMock.Object,
				notificationMock.Object);

			SetupUserBadgeIds("u1");
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(
				MakeBadge(1), MakeBadge(2), MakeBadge(3), MakeBadge(4),
				MakeBadge(5), MakeBadge(6), MakeBadge(7), MakeBadge(8),
				MakeBadge(9), MakeBadge(10), MakeBadge(11), MakeBadge(12),
				MakeBadge(13), MakeBadge(14), MakeBadge(15));

			statsServiceMock.Setup(s => s.GetSolvedCount(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetDistinctCipherTypesSolved(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetApprovedCount(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetApprovedAnswersCount(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetSolvedWithoutHintCount(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetSolvedOnFirstAttemptCount(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetUsedHints(It.IsAny<string>())).ReturnsAsync(0);
			statsServiceMock.Setup(s => s.GetRareSolves(It.IsAny<string>())).ReturnsAsync(0);

			userBadgeRepoMock.Setup(r => r.AddAsync(It.IsAny<UserBadge>()))
				.Returns(Task.CompletedTask);
			badgeRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Badge>()))
				.ReturnsAsync(true);
			notificationMock.Setup(n => n.CreateAndSendNotification(
				It.IsAny<string>(), It.IsAny<NotificationType>(),
				It.IsAny<string>(), It.IsAny<string>()))
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
			userBadgeRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedBadges(params Badge[] badges)
		{
			var mock = new List<Badge>(badges).AsQueryable().BuildMock();
			badgeRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region CheckBadgesByCategory - OnSolve

		[Fact]
		public async Task CheckBadgesByCategory_SkipsBadge_WhenUserAlreadyHasAllOnSolveBadges()
		{
			SetupUserBadgeIds("u1", 1, 2, 3, 4, 5, 6, 12, 13, 14, 15);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			statsServiceMock.Verify(s => s.GetSolvedCount(It.IsAny<string>()), Times.Never);
			statsServiceMock.Verify(s => s.GetDistinctCipherTypesSolved(It.IsAny<string>()), Times.Never);
			statsServiceMock.Verify(s => s.GetSolvedWithoutHintCount(It.IsAny<string>()), Times.Never);
			statsServiceMock.Verify(s => s.GetSolvedOnFirstAttemptCount(It.IsAny<string>()), Times.Never);
			statsServiceMock.Verify(s => s.GetUsedHints(It.IsAny<string>()), Times.Never);
			statsServiceMock.Verify(s => s.GetRareSolves(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge1_WhenSolvedCountIs1()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge1_WhenSolvedCountIs0()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(0);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserBadge>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge1And2_WhenSolvedCountIs25()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsOnlyBadge1_WhenSolvedCountIs1()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge6_WhenDistinctTypesSolvedIs10()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetDistinctCipherTypesSolved("u1")).ReturnsAsync(10);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(5), MakeBadge(6));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 6)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge3_WhenSolvedCountIs49()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(49);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 3)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge4_WhenSolvedCountIs100()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(100);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2), MakeBadge(3), MakeBadge(4));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 4)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge4_WhenSolvedCountIs99()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(99);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 4)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge5_WhenDistinctTypesSolvedIs5()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetDistinctCipherTypesSolved("u1")).ReturnsAsync(5);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(5));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 5)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge5_WhenDistinctTypesSolvedIs4()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetDistinctCipherTypesSolved("u1")).ReturnsAsync(4);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 5)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge3_WhenSolvedCountIs50()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(50);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1), MakeBadge(2), MakeBadge(3));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 3)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge6_WhenDistinctTypesSolvedIs9()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetDistinctCipherTypesSolved("u1")).ReturnsAsync(9);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 6)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_SkipsAlreadyEarnedBadge_ButAwardsNewOne()
		{
			SetupUserBadgeIds("u1", 1);
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(2));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 1)), Times.Never);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 2)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_SendsNotification_WhenBadgeIsAwarded()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(1, "First Solve"));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.BadgeEarned,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_IncrementsBadgeEarnedByCount_WhenAwarded()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			var badge = MakeBadge(1);
			SetupAttachedBadges(badge);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			Assert.Equal(1, badge.EarnedBy);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge12_WhenNoHintSolvesCriteriaMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedWithoutHintCount("u1")).ReturnsAsync(10);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(12));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 12)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge12_WhenNoHintSolvesCriteriaNotMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedWithoutHintCount("u1")).ReturnsAsync(9);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 12)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge13_WhenPerfectSolveCriteriaMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedOnFirstAttemptCount("u1")).ReturnsAsync(10);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(13));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 13)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge13_WhenPerfectSolveCriteriaNotMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedOnFirstAttemptCount("u1")).ReturnsAsync(9);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 13)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge14_WhenHintUsageCriteriaMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetUsedHints("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(14));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 14)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge14_WhenHintUsageCriteriaNotMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetUsedHints("u1")).ReturnsAsync(24);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 14)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge15_WhenRareSolveCriteriaMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetRareSolves("u1")).ReturnsAsync(25);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(15));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 15)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardBadge15_WhenRareSolveCriteriaNotMet()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetRareSolves("u1")).ReturnsAsync(24);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 15)), Times.Never);
		}

		#endregion

		#region CheckBadgesByCategory - OnUpload

		[Fact]
		public async Task CheckBadgesByCategory_SkipsBadge_WhenUserAlreadyHasAllOnCipherApproveBadges()
		{
			SetupUserBadgeIds("u1", 7, 8, 9);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnUpload);

			statsServiceMock.Verify(s => s.GetApprovedCount(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge7_WhenFirstCipherApproved()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(7), MakeBadge(8), MakeBadge(9));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnUpload);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 7)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 8)), Times.Never);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 9)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsAllOnCipherApproveBadges_WhenAllCriteriaAreSatisfied()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedCount("u1")).ReturnsAsync(15);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(7), MakeBadge(8), MakeBadge(9));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnUpload);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 7)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 8)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 9)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardOnCipherApproveBadge_WhenNoApprovedCiphers()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedCount("u1")).ReturnsAsync(0);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnUpload);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserBadge>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_SkipsAlreadyEarnedCipherBadge_ButAwardsNext()
		{
			SetupUserBadgeIds("u1", 7);
			statsServiceMock.Setup(s => s.GetApprovedCount("u1")).ReturnsAsync(5);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(8));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnUpload);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 7)), Times.Never);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 8)), Times.Once);
		}

		#endregion

		#region CheckBadgesByCategory - OnSuggesting

		[Fact]
		public async Task CheckBadgesByCategory_SkipsBadge_WhenUserAlreadyHasAllOnAnswerApproveBadges()
		{
			SetupUserBadgeIds("u1", 10, 11);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSuggesting);

			statsServiceMock.Verify(s => s.GetApprovedAnswersCount(It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBadge10_WhenFirstAnswerApproved()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedAnswersCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(10), MakeBadge(11));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSuggesting);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 10)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 11)), Times.Never);
		}

		[Fact]
		public async Task CheckBadgesByCategory_AwardsBothOnAnswerApproveBadges_WhenBothCriteriaAreSatisfied()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedAnswersCount("u1")).ReturnsAsync(10);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges(MakeBadge(10), MakeBadge(11));

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSuggesting);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 10)), Times.Once);
			userBadgeRepoMock.Verify(r => r.AddAsync(It.Is<UserBadge>(b => b.BadgeId == 11)), Times.Once);
		}

		[Fact]
		public async Task CheckBadgesByCategory_DoesNotAwardOnAnswerApproveBadge_WhenNoApprovedAnswers()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetApprovedAnswersCount("u1")).ReturnsAsync(0);

			await service.CheckBadgesByCategory("u1", BadgeCategory.OnSuggesting);

			userBadgeRepoMock.Verify(r => r.AddAsync(It.IsAny<UserBadge>()), Times.Never);
		}

		#endregion

		#region AwardBadge internal guards

		[Fact]
		public async Task CheckBadgesByCategory_Throws_WhenUserNotFoundDuringAwarding()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers();

			await Assert.ThrowsAsync<Exception>(
				() => service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve));
		}

		[Fact]
		public async Task CheckBadgesByCategory_Throws_WhenUserAlreadyHasBadgeInBadgesCollection()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);

			var user = MakeUser("u1");
			user.Badges = new List<UserBadge> { new UserBadge { BadgeId = 1, UserId = "u1" } };
			SetupAttachedUsers(user);
			SetupAttachedBadges(MakeBadge(1));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve));
		}

		[Fact]
		public async Task CheckBadgesByCategory_Throws_WhenBadgeNotFoundDuringAwarding()
		{
			SetupUserBadgeIds("u1");
			statsServiceMock.Setup(s => s.GetSolvedCount("u1")).ReturnsAsync(1);
			SetupAttachedUsers(MakeUser("u1"));
			SetupAttachedBadges();

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.CheckBadgesByCategory("u1", BadgeCategory.OnSolve));
		}

		#endregion
	}

	public class BadgeStatisticsServiceTests
	{
		private readonly Mock<IRepository<ApplicationUser, string>> userRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> solutionRepoMock = new();
		private readonly Mock<IRepository<HintRequest, int>> hintRepoMock = new();
		private readonly BadgeStatisticsService service;

		public BadgeStatisticsServiceTests()
		{
			service = new BadgeStatisticsService(
				userRepoMock.Object,
				solutionRepoMock.Object,
				hintRepoMock.Object);
		}

		private void SetupAttachedUsers(params ApplicationUser[] users)
		{
			var mock = new List<ApplicationUser>(users).AsQueryable().BuildMock();
			userRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedSolutions(params UserSolution[] solutions)
		{
			var mock = new List<UserSolution>(solutions).AsQueryable().BuildMock();
			solutionRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedHints(params HintRequest[] hints)
		{
			var mock = new List<HintRequest>(hints).AsQueryable().BuildMock();
			hintRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
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

			var result = await service.GetApprovedCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetApprovedCount_ReturnsZero_WhenUserHasNoApprovedCiphers()
		{
			var user = new ApplicationUser { Id = "u1", UploadedCiphers = new List<Cipher>() };
			SetupAttachedUsers(user);

			var result = await service.GetApprovedCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetDistinctCipherTypesSolved

		[Fact]
		public async Task GetDistinctCipherTypesSolved_ReturnsDistinctTypeCount()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", IsCorrect = true, Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar, Status = ApprovalStatus.Approved } },
				new UserSolution { UserId = "u1", IsCorrect = true, Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar, Status = ApprovalStatus.Approved } },
				new UserSolution { UserId = "u1", IsCorrect = true, Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Vigenere, Status = ApprovalStatus.Approved } },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetDistinctCipherTypesSolved("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetDistinctCipherTypesSolved_ReturnsZero_WhenUserHasNoSolutions()
		{
			SetupAttachedSolutions();

			var result = await service.GetDistinctCipherTypesSolved("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetDistinctCipherTypesSolved_OnlyCountsForGivenUser()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", IsCorrect = true, Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Caesar, Status = ApprovalStatus.Approved } },
				new UserSolution { UserId = "u2", IsCorrect = true, Cipher = new ConcreteCipher { TypeOfCipher = CipherType.Vigenere, Status = ApprovalStatus.Approved } }
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetDistinctCipherTypesSolved("u1");

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

			var result = await service.GetApprovedAnswersCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetApprovedAnswersCount_ReturnsZero_WhenUserHasNoApprovedAnswers()
		{
			var user = new ApplicationUser { Id = "u1", SuggestedAnswers = new List<AnswerSuggestion>() };
			SetupAttachedUsers(user);

			var result = await service.GetApprovedAnswersCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetSolvedCount

		[Fact]
		public async Task GetSolvedCount_ReturnsUserSolvedCount()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", IsCorrect = true },
				new UserSolution { UserId = "u1", IsCorrect = true },
				new UserSolution { UserId = "u1", IsCorrect = true },
				new UserSolution { UserId = "u2" },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedCount("u1");

			Assert.Equal(3, result);
		}

		[Fact]
		public async Task GetSolvedCount_ReturnsZero_WhenUserHasNoSolutions()
		{
			SetupAttachedSolutions();

			var result = await service.GetSolvedCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetSolvedWithoutHintCount

		[Fact]
		public async Task GetSolvedWithoutHintCount_ReturnsOnlySolutionsWithNoHintsUsed()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", IsCorrect = true,  UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = false },
				new UserSolution { UserId = "u1", IsCorrect = true,  UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = false },
				new UserSolution { UserId = "u1", IsCorrect = true,  UsedFullSolution = true,  UsedTypeHint = false, UsedSolutionHint = false },
				new UserSolution { UserId = "u1", IsCorrect = true,  UsedFullSolution = false, UsedTypeHint = true,  UsedSolutionHint = false },
				new UserSolution { UserId = "u1", IsCorrect = true,  UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = true  },
				new UserSolution { UserId = "u1", IsCorrect = false, UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = false },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedWithoutHintCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetSolvedWithoutHintCount_ReturnsZero_WhenAllSolutionsUsedHints()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", UsedFullSolution = true, UsedTypeHint = false, UsedSolutionHint = false },
				new UserSolution { UserId = "u1", UsedFullSolution = false, UsedTypeHint = true, UsedSolutionHint = false },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedWithoutHintCount("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetSolvedWithoutHintCount_OnlyCountsForGivenUser()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", IsCorrect = true, UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = false },
				new UserSolution { UserId = "u2", IsCorrect = true, UsedFullSolution = false, UsedTypeHint = false, UsedSolutionHint = false },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedWithoutHintCount("u1");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetSolvedWithoutHintCount_ReturnsZero_WhenUserHasNoSolutions()
		{
			SetupAttachedSolutions();

			var result = await service.GetSolvedWithoutHintCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetSolvedOnFirstAttemptCount

		[Fact]
		public async Task GetSolvedOnFirstAttemptCount_ReturnsCiphersWithExactlyOneCorrectAttempt()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u1", CipherId = 2, IsCorrect = false },
				new UserSolution { UserId = "u1", CipherId = 2, IsCorrect = true },
				new UserSolution { UserId = "u1", CipherId = 3, IsCorrect = false },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedOnFirstAttemptCount("u1");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetSolvedOnFirstAttemptCount_ReturnsZero_WhenNoFirstAttemptSolves()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = false },
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedOnFirstAttemptCount("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetSolvedOnFirstAttemptCount_OnlyCountsForGivenUser()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u2", CipherId = 2, IsCorrect = true },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetSolvedOnFirstAttemptCount("u1");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetSolvedOnFirstAttemptCount_ReturnsZero_WhenUserHasNoSolutions()
		{
			SetupAttachedSolutions();

			var result = await service.GetSolvedOnFirstAttemptCount("u1");

			Assert.Equal(0, result);
		}

		#endregion

		#region GetUsedHints

		[Fact]
		public async Task GetUsedHints_ReturnsDistinctCipherCount_WithHintRequests()
		{
			var hints = new[]
			{
				new HintRequest { UserId = "u1", CipherId = 1 },
				new HintRequest { UserId = "u1", CipherId = 1 },
				new HintRequest { UserId = "u1", CipherId = 2 },
				new HintRequest { UserId = "u1", CipherId = 3 },
			};
			SetupAttachedHints(hints);

			var result = await service.GetUsedHints("u1");

			Assert.Equal(3, result);
		}

		[Fact]
		public async Task GetUsedHints_ReturnsZero_WhenUserHasNoHintRequests()
		{
			SetupAttachedHints();

			var result = await service.GetUsedHints("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetUsedHints_OnlyCountsForGivenUser()
		{
			var hints = new[]
			{
				new HintRequest { UserId = "u1", CipherId = 1 },
				new HintRequest { UserId = "u2", CipherId = 2 },
				new HintRequest { UserId = "u2", CipherId = 3 },
			};
			SetupAttachedHints(hints);

			var result = await service.GetUsedHints("u1");

			Assert.Equal(1, result);
		}

		#endregion

		#region GetRareSolves

		[Fact]
		public async Task GetRareSolves_CountsCiphers_WithThreeOrFewerCorrectSolversTotal()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u1", CipherId = 2, IsCorrect = true },
				new UserSolution { UserId = "u2", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u2", CipherId = 2, IsCorrect = true },
				new UserSolution { UserId = "u3", CipherId = 2, IsCorrect = true },
				new UserSolution { UserId = "u4", CipherId = 2, IsCorrect = true },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetRareSolves("u1");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetRareSolves_CountsCipher_WhenExactlyThreeCorrectSolversTotal()
		{
			var cipher = new ConcreteCipher
			{
				Id = 1,
				UserSolutions = new List<UserSolution>
				{
					new() { IsCorrect = true },
					new() { IsCorrect = true },
					new() { IsCorrect = true },
				}
			};
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true, Cipher = cipher },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetRareSolves("u1");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetRareSolves_DoesNotCountCipher_WhenFourOrMoreCorrectSolversTotal()
		{
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u2", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u3", CipherId = 1, IsCorrect = true },
				new UserSolution { UserId = "u4", CipherId = 1, IsCorrect = true },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetRareSolves("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetRareSolves_OnlyCountsCorrectSolutions_ForGivenUser()
		{
			var cipher = new ConcreteCipher
			{
				Id = 1,
				UserSolutions = new List<UserSolution> { new() { IsCorrect = true } }
			};
			var solutions = new[]
			{
				new UserSolution { UserId = "u1", CipherId = 1, IsCorrect = false, Cipher = cipher },
				new UserSolution { UserId = "u2", CipherId = 1, IsCorrect = true, Cipher = cipher },
			};
			SetupAttachedSolutions(solutions);

			var result = await service.GetRareSolves("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetRareSolves_ReturnsZero_WhenUserHasNoCorrectSolutions()
		{
			SetupAttachedSolutions();

			var result = await service.GetRareSolves("u1");

			Assert.Equal(0, result);
		}

		#endregion
	}
}