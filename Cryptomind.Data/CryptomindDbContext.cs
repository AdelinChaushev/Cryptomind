using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Data
{
	public class CryptomindDbContext : IdentityDbContext<ApplicationUser>
	{

		public CryptomindDbContext(DbContextOptions<CryptomindDbContext> options)
		: base(options) { }

		public DbSet<ApplicationUser> Users { get; set; }
		public DbSet<ImageCipher> ImageCiphers { get; set; }
		public DbSet<TextCipher> TextCiphers { get; set; }
		public DbSet<CipherTag> CipherTags { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Badge> Badges { get; set; }
		public DbSet<HintRequest> HintRequests { get; set; }
		public DbSet<AnswerSuggestion> AnswerSuggestions { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
            base.OnModelCreating(builder);
            builder.Entity<CipherTag>()
				.HasKey(c => new { c.CipherId, c.TagId });

			builder.Entity<HintRequest>().HasOne(c => c.Cipher)
			   .WithMany(c => c.HintsRequested).OnDelete(DeleteBehavior.NoAction);
			builder.Entity<HintRequest>().HasOne(c => c.ApplicationUser)
			   .WithMany(c => c.HintsRequested).OnDelete(DeleteBehavior.NoAction);

			builder.Entity<Cipher>()
				.HasDiscriminator<string>("EntityType")
				.HasValue<TextCipher>("TextCipher")
				.HasValue<ImageCipher>("ImageCipher");

			builder.Entity<Cipher>()
				.HasOne(c => c.CreatedByUser)
				.WithMany(u => u.UploadedCiphers)
				.HasForeignKey(c => c.CreatedByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Cipher>()
				.HasIndex(c => c.Title)
				.IsUnique();

			builder.Entity<Cipher>()
				.HasIndex(c => c.EncryptedText)
				.IsUnique();

			builder.Entity<UserSolution>()
				.HasOne(us => us.Cipher)
				.WithMany(c => c.UserSolutions)
				.HasForeignKey(us => us.CipherId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserSolution>()
				.HasIndex(us => new { us.UserId, us.CipherId })
				.HasFilter("[IsCorrect] = 1")
				.IsUnique();

			builder.Entity<Cipher>()
				.OwnsOne(c => c.LLMData);

			builder.Entity<UserBadge>()
				.HasIndex(ub => new { ub.UserId, ub.BadgeId })
				.IsUnique();

			builder.Entity<AnswerSuggestion>()
				.HasIndex(a => new { a.UserId, a.CipherId, a.DecryptedText })
				.IsUnique();

			builder.Entity<Badge>().HasData(
				new Badge
				{
					Id = 1,
					Title = "First Blood",
					Description = "Solve your first cipher",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_1.png"
				},
				new Badge
				{
					Id = 2,
					Title = "Apprentice Cryptanalyst",
					Description = "Solve 25 ciphers",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_2.png"
				},
				new Badge
				{
					Id = 3,
					Title = "Seasoned Decoder",
					Description = "Solve 50 ciphers",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_3.png"
				},
				new Badge
				{
					Id = 4,
					Title = "Master Cryptanalyst",
					Description = "Solve 100 ciphers",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_4.png"
				},
				new Badge
				{
					Id = 5,
					Title = "Diverse Solver",
					Description = "Solve ciphers from 5 different types",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_5.png"
				},
				new Badge
				{
					Id = 6,
					Title = "Polyglot Decoder",
					Description = "Solve ciphers from 10 different types",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_6.png"
				},
				new Badge
				{
					Id = 7,
					Title = "Cipher Creator",
					Description = "Have your first cipher approved",
					Category = BadgeCategory.OnUpload,
					ImagePath = "../Images/Badges/Badge_7.png"
				},
				new Badge
				{
					Id = 8,
					Title = "Community Contributor",
					Description = "Have 5 ciphers approved",
					Category = BadgeCategory.OnUpload,
					ImagePath = "../Images/Badges/Badge_8.png"
				},
				new Badge
				{
					Id = 9,
					Title = "Architect of Ciphers",
					Description = "Have 15 ciphers approved",
					Category = BadgeCategory.OnUpload,
					ImagePath = "../Images/Badges/Badge_9.png"
				},
				new Badge
				{
					Id = 10,
					Title = "Helpful Mind",
					Description = "First approved suggested answer",
					Category = BadgeCategory.OnSuggesting,
					ImagePath = "../Images/Badges/Badge_10.png"
				},
				new Badge
				{
					Id = 11,
					Title = "Trusted Contributor",
					Description = "10 approved suggested answers",
					Category = BadgeCategory.OnSuggesting,
					ImagePath = "../Images/Badges/Badge_11.png"
				},
				new Badge
				{
					Id = 12,
					Title = "No Mercy",
					Description = "Solve 10 ciphers without using hints",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_12.png"
				},
				new Badge
				{
					Id = 13,
					Title = "Flawless Solver",
					Description = "Solve 10 ciphers correctly on the first attempt",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_13.png"
				},
				new Badge
				{
					Id = 14,
					Title = "Curious Mind",
					Description = "Use hints on 25 different ciphers",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_14.png"
				},
				new Badge
				{
					Id = 15,
					Title = "Against the Odds",
					Description = "Solve a cipher solved by fewer than 3 users",
					Category = BadgeCategory.OnSolve,
					ImagePath = "../Images/Badges/Badge_15.png"
				});

			
		}
	}
}
