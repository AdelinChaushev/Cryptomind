using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
				.OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

			builder.Entity<UserSolution>()
				.HasOne(us => us.Cipher)
				.WithMany(c => c.UserSolutions)
				.HasForeignKey(us => us.CipherId)
				.OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths

			builder.Entity<Cipher>()
				.OwnsOne(c => c.LLMData);

			builder.Entity<Badge>().HasData(
				new Badge
				{
					Id = 1,
					Title = "First Blood",
					Description = "Solve your first cipher",
					Category = BadgeCategory.OnSolve
				},
				new Badge
				{
					Id = 2,
					Title = "Apprentice Cryptanalyst",
					Description = "Solve 25 ciphers",
					Category = BadgeCategory.OnSolve
				},
				new Badge
				{
					Id = 3,
					Title = "Cipher Creator",
					Description = "Have your first cipher approved",
					Category = BadgeCategory.OnUpload
				},
				new Badge
				{
					Id = 4,
					Title = "Community Contributor",
					Description = "Have 5 ciphers approved",
					Category = BadgeCategory.OnUpload
				},
				new Badge
				{
					Id = 5,
					Title = "Diverse Solver",
					Description = "Solve at least one cipher from 5 different types",
					Category = BadgeCategory.Periodic
				},
				new Badge
				{
					Id = 6,
					Title = "Outstanding Cryptographer",
					Description = "Solve your first experimental cipher",
					Category = BadgeCategory.OnSuggesting
				});

			base.OnModelCreating(builder);
		}

	}
}
