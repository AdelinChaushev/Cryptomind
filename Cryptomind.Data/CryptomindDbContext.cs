using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
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
				.HasFilter("IsDeleted = 0")
				.IsUnique();

			builder.Entity<Cipher>()
			.HasIndex(c => c.EncryptedText)
			.IsUnique();

			builder.Entity<Cipher>()
				.HasIndex(c => c.Title)
				.HasFilter("IsDeleted = 0")
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

			builder.Entity<UserBadge>()
				.HasIndex(ub => new { ub.UserId, ub.BadgeId })
				.IsUnique();

			builder.Entity<AnswerSuggestion>()
				.HasIndex(a => new { a.UserId, a.CipherId, a.DecryptedText })
				.IsUnique();

			var adminRoleId = "c3d4e5f6-a7b8-9012-cdef-123456789012";
			var userRoleId = "d4e5f6a7-b8c9-0123-defa-234567890123";

			builder.Entity<IdentityRole>().HasData(
				new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
				new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
			);

			var adminId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
			var userId = "b2c3d4e5-f6a7-8901-bcde-f12345678901";
			var hasher = new PasswordHasher<ApplicationUser>();

			var admin = new ApplicationUser
			{
				Id = adminId,
				UserName = "Admin",
				NormalizedUserName = "ADMIN@CRYPTOMIND.COM",
				Email = "admin@cryptomind.com",
				NormalizedEmail = "ADMIN@CRYPTOMIND.COM",
				EmailConfirmed = true,
				SecurityStamp = "a1a1a1a1-b2b2-c3c3-d4d4-e5e5e5e5e5e5"
			};
			admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

			var regularUser = new ApplicationUser
			{
				Id = userId,
				UserName = "User",
				NormalizedUserName = "USER@CRYPTOMIND.COM",
				Email = "user@cryptomind.com",
				NormalizedEmail = "USER@CRYPTOMIND.COM",
				EmailConfirmed = true,
				SecurityStamp = "f6f6f6f6-e5e5-d4d4-c3c3-b2b2b2b2b2b2"
			};
			regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

			builder.Entity<ApplicationUser>().HasData(admin, regularUser);

			builder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string> { UserId = adminId, RoleId = adminRoleId },
				new IdentityUserRole<string> { UserId = userId, RoleId = userRoleId }
			);

			var seededAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			builder.Entity<Cipher>()
				.OwnsOne(c => c.LLMData, llm =>
				{
					llm.HasData(
						// Base64 (1-3)
						new { CipherId = 1, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Base64", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 2, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Base64", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 3, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Base64", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Hex (4-6)
						new { CipherId = 4, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Hex", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 5, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Hex", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 6, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Hex", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Binary (7-9)
						new { CipherId = 7, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Binary", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 8, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Binary", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 9, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Binary", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Morse (10-12)
						new { CipherId = 10, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Morse", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 11, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Morse", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 12, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Morse", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// ROT13 (13-15)
						new { CipherId = 13, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "ROT13", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 14, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "ROT13", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 15, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "ROT13", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Caesar (16-20)
						new { CipherId = 16, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 17, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 18, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 19, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 20, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Atbash (21-23)
						new { CipherId = 21, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Atbash", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 22, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Atbash", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 23, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Atbash", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// SimpleSubstitution (24-27)
						new { CipherId = 24, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "SimpleSubstitution", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 25, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "SimpleSubstitution", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 26, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "SimpleSubstitution", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 27, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "SimpleSubstitution", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// RailFence (28-31)
						new { CipherId = 28, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "RailFence", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 29, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "RailFence", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 30, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "RailFence", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 31, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "RailFence", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Trithemius (32-34)
						new { CipherId = 32, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Trithemius", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 33, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Trithemius", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 34, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Trithemius", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Vigenere (35-39)
						new { CipherId = 35, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 36, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 37, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 38, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 39, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Columnar (40-43)
						new { CipherId = 40, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Columnar", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 41, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Columnar", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 42, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Columnar", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 43, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Columnar", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Route (44-46)
						new { CipherId = 44, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Route", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 45, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Route", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 46, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Route", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						// Autokey (47-50)
						new { CipherId = 47, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Autokey", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 48, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Autokey", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 49, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Autokey", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 50, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Autokey", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" }
					);
				});

			builder.Entity<TextCipher>(entity =>
			{
				entity.HasData(
					new TextCipher
					{
						Id = 1,
						Title = "The Merchant's Hidden Ledger",
						DecryptedText = "The ancient art of cryptography has protected secrets for thousands of years from the earliest Egyptian scribes to modern digital encryption systems",
						EncryptedText = "VGhlIGFuY2llbnQgYXJ0IG9mIGNyeXB0b2dyYXBoeSBoYXMgcHJvdGVjdGVkIHNlY3JldHMgZm9yIHRob3VzYW5kcyBvZiB5ZWFycyBmcm9tIHRoZSBlYXJsaWVzdCBFZ3lwdGlhbiBzY3JpYmVzIHRvIG1vZGVybiBkaWdpdGFsIGVuY3J5cHRpb24gc3lzdGVtcw==",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9978, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9978}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.2e-07}]}",
						TypeOfCipher = CipherType.Base64,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 2,
						Title = "Whispers from the Ivory Tower",
						DecryptedText = "Julius Caesar famously used a simple letter shift cipher to communicate with his generals during military campaigns across the vast Roman Empire",
						EncryptedText = "SnVsaXVzIENhZXNhciBmYW1vdXNseSB1c2VkIGEgc2ltcGxlIGxldHRlciBzaGlmdCBjaXBoZXIgdG8gY29tbXVuaWNhdGUgd2l0aCBoaXMgZ2VuZXJhbHMgZHVyaW5nIG1pbGl0YXJ5IGNhbXBhaWducyBhY3Jvc3MgdGhlIHZhc3QgUm9tYW4gRW1waXJl",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9965, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9965}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.5e-07}]}",
						TypeOfCipher = CipherType.Base64,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 3,
						Title = "The Sealed Dispatch of Bletchley",
						DecryptedText = "The Enigma machine used by Germany in the Second World War was considered unbreakable until brilliant mathematicians at Bletchley Park finally cracked it",
						EncryptedText = "VGhlIEVuaWdtYSBtYWNoaW5lIHVzZWQgYnkgR2VybWFueSBpbiB0aGUgU2Vjb25kIFdvcmxkIFdhciB3YXMgY29uc2lkZXJlZCB1bmJyZWFrYWJsZSB1bnRpbCBicmlsbGlhbnQgbWF0aGVtYXRpY2lhbnMgYXQgQmxldGNobGV5IFBhcmsgZmluYWxseSBjcmFja2VkIGl0",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9982, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9982}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 1.8e-07}]}",
						TypeOfCipher = CipherType.Base64,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 4,
						Title = "The Archivist's Numbered Scroll",
						DecryptedText = "Cryptography is the science of hiding information from unintended recipients using mathematical transformations and carefully guarded secret keys",
						EncryptedText = "43727970746f6772617068792069732074686520736369656e6365206f6620686964696e6720696e666f726d6174696f6e2066726f6d20756e696e74656e64656420726563697069656e7473207573696e67206d617468656d61746963616c207472616e73666f726d6174696f6e7320616e64206361726566756c6c79206775617264656420736563726574206b657973",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9971, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9971}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 2.9e-07}]}",
						TypeOfCipher = CipherType.Hex,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 5,
						Title = "Manuscript of the Indecipherable",
						DecryptedText = "The Vigenere cipher was considered unbreakable for three centuries earning it the famous title of the indecipherable cipher among cryptographers",
						EncryptedText = "54686520566967656e657265206369706865722077617320636f6e7369646572656420756e627265616b61626c6520666f722074687265652063656e747572696573206561726e696e67206974207468652066616d6f7573207469746c65206f662074686520696e646563697068657261626c652063697068657220616d6f6e672063727970746f6772617068657273",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9968, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9968}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 3.2e-07}]}",
						TypeOfCipher = CipherType.Hex,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 6,
						Title = "The Digital Vault Inscription",
						DecryptedText = "Modern encryption algorithms protect billions of digital transactions every day keeping financial data and private communications secure from hackers",
						EncryptedText = "4d6f6465726e20656e6372797074696f6e20616c676f726974686d732070726f746563742062696c6c696f6e73206f66206469676974616c207472616e73616374696f6e7320657665727920646179206b656570696e672066696e616e6369616c206461746120616e64207072697661746520636f6d6d756e69636174696f6e73207365637572652066726f6d206861636b657273",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9975, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9975}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 2.5e-07}]}",
						TypeOfCipher = CipherType.Hex,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 7,
						Title = "The Alchemist's Binary Rune",
						DecryptedText = "HIDE THE GOLD IN THE CAVE",
						EncryptedText = "01001000 01001001 01000100 01000101 00100000 01010100 01001000 01000101 00100000 01000111 01001111 01001100 01000100 00100000 01001001 01001110 00100000 01010100 01001000 01000101 00100000 01000011 01000001 01010110 01000101",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9969, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9969}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.1e-07}]}",
						TypeOfCipher = CipherType.Binary,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 8,
						Title = "Signals from the Iron Watchtower",
						DecryptedText = "ATTACK AT DAWN BRING FORCES",
						EncryptedText = "01000001 01010100 01010100 01000001 01000011 01001011 00100000 01000001 01010100 00100000 01000100 01000001 01010111 01001110 00100000 01000010 01010010 01001001 01001110 01000111 00100000 01000110 01001111 01010010 01000011 01000101 01010011",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9963, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9963}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.7e-07}]}",
						TypeOfCipher = CipherType.Binary,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 9,
						Title = "The Clockmaker's Pulse Sequence",
						DecryptedText = "MEET AT THE BRIDGE TONIGHT",
						EncryptedText = "01001101 01000101 01000101 01010100 00100000 01000001 01010100 00100000 01010100 01001000 01000101 00100000 01000010 01010010 01001001 01000100 01000111 01000101 00100000 01010100 01001111 01001110 01001001 01000111 01001000 01010100",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9971, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9971}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.9e-07}]}",
						TypeOfCipher = CipherType.Binary,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 10,
						Title = "Dots and Dashes from the Frontier",
						DecryptedText = "THE EAGLE HAS LANDED PROCEED TO EXTRACTION",
						EncryptedText = "- .... . / . .- --. .-.. . / .... .- ... / .-.. .- -. -.. . -.. / .--. .-. --- -.-. . . -.. / - --- / . -..- - .-. .- -.-. - .. --- -.",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9974, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9974}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.6e-07}]}",
						TypeOfCipher = CipherType.Morse,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 11,
						Title = "The Operator's Last Transmission",
						DecryptedText = "SEND REINFORCEMENTS TO THE NORTHERN BORDER",
						EncryptedText = "... . -. -.. / .-. . .. -. ..-. --- .-. -.-. . -- . -. - ... / - --- / - .... . / -. --- .-. - .... . .-. -. / -... --- .-. -.. . .-.",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9961, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9961}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.9e-07}]}",
						TypeOfCipher = CipherType.Morse,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 12,
						Title = "Echoes Through the Telegraph Wire",
						DecryptedText = "CIPHER DECODED ADVANCE TO SECONDARY POSITION",
						EncryptedText = "-.-. .. .--. .... . .-. / -.. . -.-. --- -.. . -.. / .- -.. ...- .- -. -.-. . / - --- / ... . -.-. --- -. -.. .- .-. -.-- / .--. --- ... .. - .. --- -.",
						MLPrediction = "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9978, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9978}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.2e-07}]}",
						TypeOfCipher = CipherType.Morse,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 50,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 13,
						Title = "The Mirrored Philosopher's Note",
						DecryptedText = "THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG WHILE THE HOUND WATCHED FROM BEHIND THE OLD OAK TREE IN THE SUNNY MEADOW",
						EncryptedText = "GUR DHVPX OEBJA SBK WHZCRQ BIRE GUR YNML QBT JUVYR GUR UBHAQ JNGPURQ SEBZ ORUVAQ GUR BYQ BNX GERR VA GUR FHAAL ZRNQBJ",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9891, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9891}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.09e-06}]}",
						TypeOfCipher = CipherType.ROT13,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 75,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 14,
						Title = "A Letter Left in the Looking Glass",
						DecryptedText = "CRYPTOGRAPHY IS THE PRACTICE OF SECURING COMMUNICATIONS FROM ADVERSARIES WHO MIGHT INTERCEPT AND READ PRIVATE MESSAGES BETWEEN TRUSTED PARTIES",
						EncryptedText = "PELCGBTENCUL VF GUR CENPGVPR BS FRPHEVAT PBZZHAVPNGVBAF SEBZ NQIREFNEVRF JUB ZVTUG VAGREPRCG NAQ ERNQ CEVINGR ZRFFNTRF ORGJRRA GEHFGRQ CNEGVRF",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9847, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9847}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.53e-06}]}",
						TypeOfCipher = CipherType.ROT13,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 75,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 15,
						Title = "The Reversed Scholar's Testimony",
						DecryptedText = "THE CAESAR CIPHER IS ONE OF THE OLDEST AND MOST WIDELY KNOWN ENCRYPTION TECHNIQUES IN THE ENTIRE HISTORY OF SECRET COMMUNICATION AND CRYPTANALYSIS",
						EncryptedText = "GUR PNRFNE PVCURE VF BAR BS GUR BYQRFG NAQ ZBFG JVQRYL XABJA RAPELCGVBA GRPUAVDHRF VA GUR RAGVER UVFGBEL BS FRPERG PBZZHAVPNGVBA NAQ PELCGNANYLFVF",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9863, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9863}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.37e-06}]}",
						TypeOfCipher = CipherType.ROT13,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 75,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 16,
						Title = "The Emperor's Forgotten Orders",
						DecryptedText = "THE ANCIENT ROMANS DEVELOPED MANY SYSTEMS FOR SENDING SECRET MESSAGES ACROSS THEIR VAST EMPIRE WITHOUT ENEMIES BEING ABLE TO READ THE CONTENTS",
						EncryptedText = "WKH DQFLHQW URPDQV GHYHORSHG PDQB VBVWHPV IRU VHQGLQJ VHFUHW PHVVDJHV DFURVV WKHLU YDVW HPSLUH ZLWKRXW HQHPLHV EHLQJ DEOH WR UHDG WKH FRQWHQWV",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9913, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9913}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 8.7e-07}]}",
						TypeOfCipher = CipherType.Caesar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 100,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 17,
						Title = "A Soldier's Note from the Dark Forest",
						DecryptedText = "SOLDIERS MARCHING THROUGH THE DARK FOREST MUST REMAIN COMPLETELY SILENT AND COMMUNICATE ONLY THROUGH PREARRANGED CODED SIGNALS AND SECRET MESSAGES",
						EncryptedText = "ZVSKPLYZ THYJOPUN AOYVBNO AOL KHYR MVYLZA TBZA YLTHPU JVTWSLALSF ZPSLUA HUK JVTTBUPJHAL VUSF AOYVBNO WYLHYYHUNLK JVKLK ZPNUHSZ HUK ZLJYLA TLZZHNLZ",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9887, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9887}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.13e-06}]}",
						TypeOfCipher = CipherType.Caesar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 100,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 18,
						Title = "Parchment from the Royal Library",
						DecryptedText = "IN THE DEPTHS OF THE ROYAL LIBRARY THERE EXISTS AN ANCIENT MANUSCRIPT DESCRIBING THE METHODS USED BY SPIES TO CONCEAL THEIR SECRET COMMUNICATIONS",
						EncryptedText = "TY ESP OPAESD ZQ ESP CZJLW WTMCLCJ ESPCP PITDED LY LYNTPYE XLYFDNCTAE OPDNCTMTYR ESP XPESZOD FDPO MJ DATPD EZ NZYNPLW ESPTC DPNCPE NZXXFYTNLETZYD",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9901, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9901}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 9.9e-07}]}",
						TypeOfCipher = CipherType.Caesar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 100,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 19,
						Title = "The Spymaster's Market Errand",
						DecryptedText = "THE MASTER OF SECRETS WALKED SLOWLY THROUGH THE MARKETPLACE PASSING CODED MESSAGES HIDDEN INSIDE ORDINARY LOOKING OBJECTS TO HIS TRUSTED NETWORK",
						EncryptedText = "KYV DRJKVI FW JVTIVKJ NRCBVU JCFNCP KYIFLXY KYV DRIBVKGCRTV GRJJZEX TFUVU DVJJRXVJ YZUUVE ZEJZUV FIUZERIP CFFBZEX FSAVTKJ KF YZJ KILJKVU EVKNFIB",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9878, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9878}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.22e-06}]}",
						TypeOfCipher = CipherType.Caesar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 100,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 20,
						Title = "Beyond the Northern Mountain Pass",
						DecryptedText = "BEYOND THE MOUNTAIN RANGE LIES A HIDDEN VALLEY WHERE THE SECRET ORDER OF CRYPTOGRAPHERS HAS MAINTAINED ITS ANCIENT TRADITION OF SECRET KEEPING",
						EncryptedText = "WZTJIY OCZ HJPIOVDI MVIBZ GDZN V CDYYZI QVGGZT RCZMZ OCZ NZXMZO JMYZM JA XMTKOJBMVKCZMN CVN HVDIOVDIZY DON VIXDZIO OMVYDODJI JA NZXMZO FZZKDIB",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9924, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9924}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 7.6e-07}]}",
						TypeOfCipher = CipherType.Caesar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 100,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 21,
						Title = "The Hebrew Scribe's Sacred Text",
						DecryptedText = "THE ANCIENT HEBREW SCHOLARS USED THIS ELEGANT CIPHER TO CONCEAL SACRED TEXTS FROM THOSE WHO WERE NOT INITIATED INTO THEIR SECRET TRADITIONS",
						EncryptedText = "GSV ZMXRVMG SVYIVD HXSLOZIH FHVW GSRH VOVTZMG XRKSVI GL XLMXVZO HZXIVW GVCGH UILN GSLHV DSL DVIV MLG RMRGRZGVW RMGL GSVRI HVXIVG GIZWRGRLMH",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9756, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9756}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.44e-06}]}",
						TypeOfCipher = CipherType.Atbash,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 125,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 22,
						Title = "Intercepted Letter from the Southern Court",
						DecryptedText = "WHEN THE SPYMASTER RECEIVED THE ENCODED LETTER HE QUICKLY REVERSED EACH CHARACTER AND DECODED THE URGENT MESSAGE ABOUT THE ENEMY TROOP MOVEMENTS",
						EncryptedText = "DSVM GSV HKBNZHGVI IVXVREVW GSV VMXLWVW OVGGVI SV JFRXPOB IVEVIHVW VZXS XSZIZXGVI ZMW WVXLWVW GSV FITVMG NVHHZTV ZYLFG GSV VMVNB GILLK NLEVNVMGH",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9712, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9712}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.88e-06}]}",
						TypeOfCipher = CipherType.Atbash,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 125,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 23,
						Title = "The Mirror Inscription of Alexandria",
						DecryptedText = "THE SIMPLEST FORM OF SUBSTITUTION MAPS EACH LETTER TO ITS MIRROR IMAGE WHERE A BECOMES Z AND B BECOMES Y CONTINUING THROUGHOUT THE ENTIRE ALPHABET",
						EncryptedText = "GSV HRNKOVHG ULIN LU HFYHGRGFGRLM NZKH VZXS OVGGVI GL RGH NRIILI RNZTV DSVIV Z YVXLNVH A ZMW Y YVXLNVH B XLMGRMFRMT GSILFTSLFG GSV VMGRIV ZOKSZYVG",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9788, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9788}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.12e-06}]}",
						TypeOfCipher = CipherType.Atbash,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 125,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 24,
						Title = "The Midnight Letter",
						DecryptedText = "THE MYSTERIOUS LETTER ARRIVED AT MIDNIGHT CONTAINING A SECRET MESSAGE THAT ONLY THOSE WHO POSSESSED THE CIPHER KEY COULD POSSIBLY HOPE TO DECIPHER",
						EncryptedText = "ZIT DNLZTKOGXL STZZTK QKKOCTR QZ DORFOUIZ EGFZQOFOFU Q LTEKTZ DTLLQUT ZIQZ GFSN ZIGLT VIG HGLLTLLTR ZIT EOHITK ATN EGXSR HGLLOWSN IGHT ZG RTEOHITK",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9634, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9634}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 3.66e-06}]}",
						TypeOfCipher = CipherType.SimpleSubstitution,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 250,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 25,
						Title = "Beneath the Old Tavern Floor",
						DecryptedText = "HIDDEN BENEATH THE FLOORBOARDS OF THE OLD TAVERN WAS A SEALED BOX CONTAINING THE CIPHER KEY AND SEVERAL ENCODED MESSAGES FROM THE UNDERGROUND NETWORK",
						EncryptedText = "LKVVCD NCDCMUL ULC XGSSONSMOVI SX ULC SGV UMTCOD RMI M ICMGCV NSE BSDUMKDKDZ ULC BKALCO HCW MDV ICTCOMG CDBSVCV FCIIMZCI XOSF ULC YDVCOZOSYDV DCURSOH",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9591, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9591}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 4.09e-06}]}",
						TypeOfCipher = CipherType.SimpleSubstitution,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 250,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 26,
						Title = "The Southern Front Dispatch",
						DecryptedText = "THE ENCODED DISPATCH FROM THE SOUTHERN FRONT DESCRIBED THE POSITIONS OF ENEMY FORTIFICATIONS AND THE ROUTES AVAILABLE FOR THE ADVANCING CAVALRY COLUMN",
						EncryptedText = "CEI IFQDGIG GARXPCQE UKDO CEI RDVCEIKF UKDFC GIRQKAHIG CEI XDRACADFR DU IFIOW UDKCAUAQPCADFR PFG CEI KDVCIR PSPANPHNI UDK CEI PGSPFQAFM QPSPNKW QDNVOF",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9647, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9647}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 3.53e-06}]}",
						TypeOfCipher = CipherType.SimpleSubstitution,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 250,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 27,
						Title = "Dust and Silence in the Royal Archive",
						DecryptedText = "DEEP IN THE ARCHIVES OF THE ROYAL CIPHER OFFICE THERE ARE THOUSANDS OF ENCODED MESSAGES WAITING TO BE CRACKED BY SKILLED CRYPTANALYSTS WITH SUFFICIENT TIME",
						EncryptedText = "QFFT WX UJF DYVJWLFA HI UJF YHZDS VWTJFY HIIWVF UJFYF DYF UJHOADXQA HI FXVHQFQ CFAADBFA RDWUWXB UH KF VYDVEFQ KZ AEWSSFQ VYZTUDXDSZAUA RWUJ AOIIWVWFXU UWCF",
						MLPrediction = "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9598, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9598}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 4.02e-06}]}",
						TypeOfCipher = CipherType.SimpleSubstitution,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 250,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 28,
						Title = "The Zigzagging Courier's Note",
						DecryptedText = "THESECRETMESSAGEHASBEENHIDDENINSIDETHISPATTERNANDYOUWILLNEEDTOFIGUREOUTHOWMANYROWSOFTHEFENCEWERETOUSEDINORDERTODECIPHERIT",
						EncryptedText = "TETSHEINIHARDWNTGOONWTEWTEOREHTHSCEMSAEABEHDEISDTIPTENNYUILEDOIUEUHWAYOSFHFNEEEOSDNRETDCPEIEREGSNDNESTAOLEFRTMROECRUIDOIR",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9234, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9234}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 7.66e-06}]}",
						TypeOfCipher = CipherType.RailFence,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 200,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 29,
						Title = "A Winding Path Through the Fortress",
						DecryptedText = "CRYPTOGRAPHYISTHESCIENCEOFSECRETWRITINGANDITHASBEENUSEDTHROUGHOUTHISTORYTOPROTECTSENSITIVEINFORMATIONFROMFALLINGINTOTHEHANDSOFADVERSARIES",
						EncryptedText = "CGICOEIIEDGITESIARLTAAARORYSSIEFRTTNDTBEETUHHSYOTCNIENMTFOLINOHNFDSRYTAHTEECSCWIGNHSNSHOOTTRPOTETVFRINMANITEDOVRISPPHNERAAURUORSIOOFGHSEE",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9187, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9187}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 8.13e-06}]}",
						TypeOfCipher = CipherType.RailFence,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 200,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 30,
						Title = "The Six-Rail Garrison Communique",
						DecryptedText = "THEMASTERCRYPTOGRAPHERENCRYPTEDTHEMESSAGEUSINGAFENCEOFSIXRAILSANDONLYTHOSEWHOKNEWTHECORRECTPATTERNCOULDHOPETORECOVERTHEORIGINALTEXT",
						EncryptedText = "TREDECLHWTUERTHCYHRETGUNEISTOETCPOLRCOIXERPPETHASEOAAYSNHEACDOOEGEMETANPESIFFRNLEKERTNHTVHITATORCYMSNASXDNWOCRTROEETNLSGREGIOHOEPRA",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9156, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9156}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 8.44e-06}]}",
						TypeOfCipher = CipherType.RailFence,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 200,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 31,
						Title = "Two Rows Across the Battlefield",
						DecryptedText = "WHENTHEGENERALSSENTTHEIRORDERSTOTHEFRONTLINEOFFICERSTHEYUSEDATRANSPOSITIONCIPHERTOSCRAMBLETHELETTERSANDPREVENTTHEENEMYFROMREADINGTHEPLANS",
						EncryptedText = "WETEEEASETHIODRTTERNLNOFCRTEUEARNPSTOCPETSRMLTEETRADRVNTENMFORAIGHPASHNHGNRLSNTERRESOHFOTIEFIESHYSDTASOIINIHROCABEHLTESNPEETHEEYRMEDNTELN",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9301, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9301}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 6.99e-06}]}",
						TypeOfCipher = CipherType.RailFence,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = false,
						Points = 200,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 32,
						Title = "The Abbot's Progressive Manuscript",
						DecryptedText = "THE TRITHEMIUS CIPHER WAS INVENTED BY THE GERMAN ABBOT JOHANNES TRITHEMIUS IN THE FIFTEENTH CENTURY AND WAS ONE OF THE EARLIEST POLYALPHABETIC CIPHERS",
						EncryptedText = "TIG WVNZOMVSFE PWEXVJ PUN EKTDNUGG FD ZOM POCYNB PRSGM DJDXLMET VUMYNLURED UA HWU WAYNZAKRG CFPWYWE HVM GLE BBT EW LAY ZWOJHETV SSQEHTYRLNRHXS TAIBZNP",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9478, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9478}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.22e-06}]}",
						TypeOfCipher = CipherType.Trithemius,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 275,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 33,
						Title = "A Shifting Veil Over the Words",
						DecryptedText = "UNLIKE THE CAESAR CIPHER WHICH USES A FIXED SHIFT THE TRITHEMIUS CIPHER PROGRESSIVELY INCREASES THE SHIFT BY ONE FOR EACH SUCCESSIVE LETTER IN THE MESSAGE",
						EncryptedText = "UONLOJ ZOM LKPENF RYGZXL RDFAG UTGV E KOEMM CSUSH IXV LKCODBKHUT ELTMKY XAYRDRGHYMWES DJZPDATGV XMK ZPRPE NL CCU WGK YVYE QTCDGVWNBL TNDEQE WC JYW FYNOXED",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9412, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9412}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.88e-06}]}",
						TypeOfCipher = CipherType.Trithemius,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 275,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 34,
						Title = "The Field Report from the Outer Wall",
						DecryptedText = "THE ENCODED FIELD REPORT DESCRIBED IN CAREFUL DETAIL THE LOCATION OF THE ENEMY SUPPLY DEPOT AND THE ESTIMATED NUMBER OF SOLDIERS GUARDING THE ENTIRE FACILITY",
						EncryptedText = "TIG HRHUKMM PTQYR GUGGKN YAPAQICGG MS IHZNPFX QSIQZD MBZ HLAZTJQQ SK ZOM NXPYL GJFGDR XZLLR ZNE VKI JYAQVKEQQ BJCSWK IA OLJCIFTV KZGYLRXR FUS TDKAKY AWZGKIUA",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9445, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9445}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.55e-06}]}",
						TypeOfCipher = CipherType.Trithemius,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 275,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 35,
						Title = "The Keyword That Shifts All Things",
						DecryptedText = "THE VIGENERE CIPHER USES A KEYWORD TO SHIFT EACH LETTER BY A DIFFERENT AMOUNT MAKING SIMPLE FREQUENCY ANALYSIS INSUFFICIENT TO CRACK THE ENCODED MESSAGE",
						EncryptedText = "LLG MMZWRGII VATJVV NKIU R OXQAQIH MG WJZJM WEEY PXLXGI FR S HKWJXJIPK EFGYPK QTCMPX WBETNV JKWUWVRVQ EPRPRKMU ZRLMJHZGBWRV KS VJEEB XAW IPTSWWH OVWLSKG",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9367, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9367}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.33e-06}]}",
						TypeOfCipher = CipherType.Vigenere,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 400,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 36,
						Title = "Three Centuries of Silence",
						DecryptedText = "BLAISE DE VIGENERE PUBLISHED HIS FAMOUS CIPHER IN THE SIXTEENTH CENTURY AND FOR THREE HUNDRED YEARS IT WAS CONSIDERED COMPLETELY UNBREAKABLE BY CRYPTANALYSTS",
						EncryptedText = "DTPPWV FM KPKVPMGL TLDTXZLVF PXZ JROWJZ GZRPTY ME VPT ZMOVMTUXY EMCAYIA ICK JFT BWYIV JCCKVVF GTHVJ KB LHW TQVHPHVTMS JSDRTTAICA CCIVVCSPIPV DG RYCGVICHPPUBH",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9312, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9312}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.88e-06}]}",
						TypeOfCipher = CipherType.Vigenere,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 400,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 37,
						Title = "The Distance Between Repeated Shadows",
						DecryptedText = "THE KASISKI EXAMINATION METHOD CAN BE USED TO DETERMINE THE KEY LENGTH OF A VIGENERE ENCODED MESSAGE BY FINDING REPEATED SEQUENCES AND MEASURING THE DISTANCES",
						EncryptedText = "VYC ZTGKJIX XLCDGCTHKFL BXHJFB RTB DV SHXR VF BTMSTDGCX HJV ITR ZGEEIA CH R TXZSPVPT XBEFBTW AGJQPZS DP DXGRKEE GXDGRRTW GGHSTGQGJ YCW AGRQJKWPX RWX RKJRPGQGJ",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9389, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9389}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.11e-06}]}",
						TypeOfCipher = CipherType.Vigenere,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 400,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 38,
						Title = "Dispatches Between the Divided Armies",
						DecryptedText = "DURING THE AMERICAN CIVIL WAR BOTH THE UNION AND CONFEDERATE ARMIES USED THE VIGENERE CIPHER TO SEND SENSITIVE MILITARY COMMUNICATIONS BETWEEN FIELD COMMANDERS",
						EncryptedText = "XHZWAA GPS NGRZWPUA KWICY EOE VBBV GBR CBVIA IBQ WBVTRXRZOGY NZAVYF CGRX GPS ICTMBRLR KWCBRZ HB MRVR FYAAWGCIM AVFVBOES PWAZOAQQNNVWBF VRBKRYA NWRFQ KCZGNVRRLF",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9298, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9298}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.02e-06}]}",
						TypeOfCipher = CipherType.Vigenere,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 400,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 39,
						Title = "The Index of Falling Letters",
						DecryptedText = "THE INDEX OF COINCIDENCE IS A POWERFUL STATISTICAL TOOL USED BY CRYPTANALYSTS TO DETECT POLYALPHABETIC ENCRYPTION AND ESTIMATE THE LENGTH OF THE KEYWORD USED",
						EncryptedText = "WLP BNGII HF FSTGCLHPGCH MD T PRAPKFXP DMAWMDMIFEW MORP FLEG FJ VRBTETNDPJLTV XZ WEWINM PRPJTLSLLUEWMN XNFVJITLSY TNG IDMIPEEX TKI WXNJXS HF WLP DEBAZKD XWPW",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9334, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9334}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.66e-06}]}",
						TypeOfCipher = CipherType.Vigenere,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 400,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 40,
						Title = "The Rearranged Supply Route",
						DecryptedText = "THESUPPLYCONVOYISSCHEDULEDTODEPARTFROMTHEBASEATARRIVALOFTHENORTHERNGROUPANDWILLTAKETHEEASTERNROUTETHROUGHTHEPASSTOAVOIDENEMYPATROLS",
						EncryptedText = "TPVCEPOAROONALHETUPANTPNSLERBALNRPLTTUOEOEA#SCIDOTHAVHHOWKARHTSIYLHLOHDAMSRFRGNTEREGAVERUOSUDFETAEEUIESORHTDPSEYYETRTEITTRDAENTHSOMO",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9134, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9134}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.66e-06}]}",
						TypeOfCipher = CipherType.Columnar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 350,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 41,
						Title = "Contact Protocol at the Prescribed Hour",
						DecryptedText = "THEOPERATIVEWILLMAKECONTACTATTHEPRESCRIBEDLOCATIONWHENANALLCLEARSIGNALISGIVENANDWILLTHENDELIVERTHEINTELLIGENCEREPORTTOTHESTATIONCHIEF",
						EncryptedText = "PILEATEBCNACSLVDTERNIERHTH#TEVLCCHSEAWNLIIEWHLTTGRTEIIHREMOTECDTHAEGSNIEIHEEETSOEEAWANAPRLIELANGALNVELNPOTNFOTIKTTRIOONLRAINLDEILCOTAC#",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9087, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9087}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 9.13e-06}]}",
						TypeOfCipher = CipherType.Columnar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 350,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 42,
						Title = "The Scrambled Grid of Forgotten Names",
						DecryptedText = "COLUMNARANSPOSITIONISATYPEOFTRANSPOSITIONCIPHERTHATREARRANGESLETTERSACCORDINGTOASPECIFICKEYWORDORDERINGOFTHECOLUMNSINTHEGRIDLAYOUT",
						EncryptedText = "MNIIPROOHARETCIAIEDRFOSELTLROOTFSTITENESRTECODGEMTIOOAPIAONICRRALROGPIWRNHUNRYUASNYTPIPHAGTADOCKREOCNHDUCNSTSEASNETRSECNSFYOITLIGA",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9112, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9112}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.88e-06}]}",
						TypeOfCipher = CipherType.Columnar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 350,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 43,
						Title = "Positions Without Letters, Letters Without Order",
						DecryptedText = "THEANCIENTMETHODOFTRANSPOSITIONCIPHERSWORKBYREARRANGINGTHELETTERSOFTHEMESSAGEWITHOUTCHANGINGTHELETTERTHEMSELVESONLYCHANGINGTHEIRPOSITIONS",
						EncryptedText = "ENOAIIWRNHEHAHATTMSHGPOATDNTPOEGEREGONHESOATONNMOSIHRAILSMEUGERENNHSSHEHRSCSYATTTSTHGTEECNRITITTONRBRGTFSICNEHVYIITCEFPOEKRNEOEWTILTLLGEI#",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9098, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9098}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 9.02e-06}]}",
						TypeOfCipher = CipherType.Columnar,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 350,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 44,
						Title = "The Spiral Path of the Cartographer",
						DecryptedText = "THEROUTECIPHERARRANGESTHETEXTINTOAGRIDANDTHENREADSITOUTINASPIRALPATTERNSTARTINGFROMTHETOPANDWORKINGITSWAYINWARD",
						EncryptedText = "THEROUTECIPSOETAIOGR##########DIPNTINATHERARRANGETHUPTTNAWNIYAWSTAGTNRGHETEXTINTOLREIKROWDNFEAERIDANDTAAHTMORRSADSIRTSNPI",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9198, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9198}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.02e-06}]}",
						TypeOfCipher = CipherType.Route,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 375,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 45,
						Title = "A Message Hidden in Plain Geometry",
						DecryptedText = "SECRETMESSAGESWEREHIDDENINSIDEINNOCENTLOOKINGTEXTSBYARRANGINGTHELETTERSONAGRIDANDTHENREADINGTHEMOUTINASPECIFICORDER",
						EncryptedText = "SECRETMESSADNNREIATC######REDROIDDTAGOEGESWEREHIDNIRLREUIFICEPSANIATNTCNINSIDEIKAEGROMEHTGNNEGEENTLOOYHANEHTDRIXTSBTNOSNG",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9134, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9134}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.66e-06}]}",
						TypeOfCipher = CipherType.Route,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 375,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 46,
						Title = "The Clockwise Descent of the Cryptographer",
						DecryptedText = "THECRYPTOGRAPHERPLACEDTHEPLAINTEXTINTOASQUAREGRIDANDREADTHECHARACTERSOUTUSINGASPIRALROUTETHATMOVEDCLOCKWISEINWARD",
						EncryptedText = "THECRYPTOGRDXRATGTCW########DRALEAEDETTAPHERPLACEEAECNUDNIESIWKCOTSRTGIHEPLAINTURAIOEVOMTAHPSHRNTOASQDRSRLARIOEIDANAUTUCH",
						MLPrediction = "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9167, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9167}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.33e-06}]}",
						TypeOfCipher = CipherType.Route,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 375,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 47,
						Title = "The Message That Devours Its Own Key",
						DecryptedText = "THE AUTOKEY CIPHER IMPROVES UPON THE VIGENERE BY USING THE PLAINTEXT ITSELF AS PART OF THE KEY AFTER THE INITIAL KEYWORD MAKING IT SIGNIFICANTLY HARDER TO CRACK",
						EncryptedText = "DLC TBXOEXM MMNJMG PQGZAKVG PTGH IVR OPKZVKVR FP YTGHY BUK ISEXYTMKM MQLMEX ED UAJI OW MVJ DLC KJREW MLV BUMBVIE SEJGSPZ ARNUNQ QG YQZFQLVKFVVLL ALPKEI WS TKOEB",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9234, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9234}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.66e-06}]}",
						TypeOfCipher = CipherType.Autokey,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 500,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 48,
						Title = "The Intelligence Officer's Running Cipher",
						DecryptedText = "THE INTELLIGENCE OFFICER CAREFULLY ENCODED THE SENSITIVE DISPATCH USING THE AUTOKEY METHOD WHICH INCORPORATED THE MESSAGE TEXT DIRECTLY INTO THE ENCRYPTION KEYSTREAM",
						EncryptedText = "TST PNMLPTVZIYNM UJSKGSW HITIWWLCC JHNZBIQ VVH WHGZMLMIW LBAKEWKZ JSBPN NZM NAMVOES FSDLMP AAPQK EUKQYXBTOKTR KHX QHLZESI LWXZ HBVBVWTP MPMZ RPR XBVYCTGKFL ZXGGGBIYE",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9178, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9178}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 8.22e-06}]}",
						TypeOfCipher = CipherType.Autokey,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 500,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 49,
						Title = "No Repeating Pattern, No Easy Answer",
						DecryptedText = "CRYPTANALYSTS STRUGGLED FOR YEARS TO DEVELOP RELIABLE METHODS FOR BREAKING AUTOKEY CIPHERS BECAUSE THE RUNNING KEY ELIMINATES THE REPEATING PATTERNS FOUND IN VIGENERE",
						EncryptedText = "QDCVTCEYARSGS DRJNYYEVX LUC CHFFJ RS DVNXZRT MIWWPSPP UEUSSPW YVF EJJOBJEK AEBBQES VWZLCTA QLGRMTI VHY JYGUMEA XRG RRSQGRLBQA GHX VWILEKMCK PTBGKGNL YSLAV NB PVJMAZZK",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9212, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9212}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.88e-06}]}",
						TypeOfCipher = CipherType.Autokey,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 500,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					},
					new TextCipher
					{
						Id = 50,
						Title = "The Diplomat's Self-Consuming Letter",
						DecryptedText = "THE DIPLOMAT ENCODED HIS REPORT USING THE PREARRANGED KEYWORD AND THE AUTOKEY METHOD ENSURING THAT EVEN IF THE KEYWORD WERE DISCOVERED THE MESSAGE WOULD REMAIN SAFE",
						EncryptedText = "ULX DBWPRUPE SZCHHRF VLW ULXGIX JGZGA LPR VKLEGIENXVD XKCZYVB WBU WHR DNASKYR AOXFAH XUGXVVFA KPNZ XCEG MA XUM PXFAYVB SSIH ZMJGRDWTSY XYI PXZWMKW OOAPZ FYXDZR EANR",
						MLPrediction = "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9189, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9189}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 8.11e-06}]}",
						TypeOfCipher = CipherType.Autokey,
						ChallengeType = ChallengeType.Standard,
						CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						ApprovedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						AllowTypeHint = true,
						AllowHint = true,
						AllowSolution = true,
						Status = ApprovalStatus.Approved,
						IsDeleted = false,
						IsPlaintextValid = true,
						IsLLMRecommended = true,
						Points = 500,
						CreatedByUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
					}
				);
			});


			builder.Entity<Badge>().HasData(
				new Badge { Id = 1, Title = "First Blood", Description = "Solve your first cipher", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_1.png" },
				new Badge { Id = 2, Title = "Apprentice Cryptanalyst", Description = "Solve 25 ciphers", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_2.png" },
				new Badge { Id = 3, Title = "Seasoned Decoder", Description = "Solve 50 ciphers", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_3.png" },
				new Badge { Id = 4, Title = "Master Cryptanalyst", Description = "Solve 100 ciphers", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_4.png" },
				new Badge { Id = 5, Title = "Diverse Solver", Description = "Solve ciphers from 5 different types", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_5.png" },
				new Badge { Id = 6, Title = "Polyglot Decoder", Description = "Solve ciphers from 10 different types", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_6.png" },
				new Badge { Id = 7, Title = "Cipher Creator", Description = "Have your first cipher approved", Category = BadgeCategory.OnUpload, ImagePath = "../Images/Badges/Badge_7.png" },
				new Badge { Id = 8, Title = "Community Contributor", Description = "Have 5 ciphers approved", Category = BadgeCategory.OnUpload, ImagePath = "../Images/Badges/Badge_8.png" },
				new Badge { Id = 9, Title = "Architect of Ciphers", Description = "Have 15 ciphers approved", Category = BadgeCategory.OnUpload, ImagePath = "../Images/Badges/Badge_9.png" },
				new Badge { Id = 10, Title = "Helpful Mind", Description = "First approved suggested answer", Category = BadgeCategory.OnSuggesting, ImagePath = "../Images/Badges/Badge_10.png" },
				new Badge { Id = 11, Title = "Trusted Contributor", Description = "10 approved suggested answers", Category = BadgeCategory.OnSuggesting, ImagePath = "../Images/Badges/Badge_11.png" },
				new Badge { Id = 12, Title = "No Mercy", Description = "Solve 10 ciphers without using hints", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_12.png" },
				new Badge { Id = 13, Title = "Flawless Solver", Description = "Solve 10 ciphers correctly on the first attempt", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_13.png" },
				new Badge { Id = 14, Title = "Curious Mind", Description = "Use hints on 25 different ciphers", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_14.png" },
				new Badge { Id = 15, Title = "Against the Odds", Description = "Solve a cipher solved by fewer than 3 users", Category = BadgeCategory.OnSolve, ImagePath = "../Images/Badges/Badge_15.png" }
			);

			builder.Entity<Tag>().HasData(
				new Tag { Id = 1, Type = TagType.Image },
				new Tag { Id = 2, Type = TagType.Puzzle },
				new Tag { Id = 3, Type = TagType.Historical },
				new Tag { Id = 4, Type = TagType.Short },
				new Tag { Id = 5, Type = TagType.Long },
				new Tag { Id = 6, Type = TagType.Beginner_Friendly },
				new Tag { Id = 7, Type = TagType.Tricky }
			);

			builder.Entity<CipherTag>().HasData(
				// The ROT13 Challenge - Beginner, Short
				new CipherTag { CipherId = 1, TagId = 6 },
				new CipherTag { CipherId = 1, TagId = 4 },

				// Caesar's Secret - Historical, Beginner
				new CipherTag { CipherId = 2, TagId = 3 },
				new CipherTag { CipherId = 2, TagId = 6 },

				// Mirror of Letters (Atbash) - Historical, Short
				new CipherTag { CipherId = 3, TagId = 3 },
				new CipherTag { CipherId = 3, TagId = 4 },

				// Scrambled Alphabet (SimpleSubstitution) - Puzzle, Tricky
				new CipherTag { CipherId = 4, TagId = 2 },
				new CipherTag { CipherId = 4, TagId = 7 },

				// The Vigenere Veil - Puzzle, Tricky
				new CipherTag { CipherId = 5, TagId = 2 },
				new CipherTag { CipherId = 5, TagId = 7 },

				// Autokey Enigma - Puzzle, Tricky, Long
				new CipherTag { CipherId = 6, TagId = 2 },
				new CipherTag { CipherId = 6, TagId = 7 },
				new CipherTag { CipherId = 6, TagId = 5 },

				// The Trithemius Ladder - Historical, Puzzle
				new CipherTag { CipherId = 7, TagId = 3 },
				new CipherTag { CipherId = 7, TagId = 2 },

				// The Rail Fence - Puzzle, Short
				new CipherTag { CipherId = 8, TagId = 2 },
				new CipherTag { CipherId = 8, TagId = 4 },

				// The Columnar Maze - Puzzle, Tricky
				new CipherTag { CipherId = 9, TagId = 2 },
				new CipherTag { CipherId = 9, TagId = 7 },

				// The Base64 Barrier - Beginner, Short
				new CipherTag { CipherId = 10, TagId = 6 },
				new CipherTag { CipherId = 10, TagId = 4 },

				// Dots and Dashes (Morse) - Beginner, Long
				new CipherTag { CipherId = 11, TagId = 6 },
				new CipherTag { CipherId = 11, TagId = 5 },

				// The Binary Message - Beginner, Short
				new CipherTag { CipherId = 12, TagId = 6 },
				new CipherTag { CipherId = 12, TagId = 4 },

				// Hex Decoded - Beginner, Short
				new CipherTag { CipherId = 13, TagId = 6 },
				new CipherTag { CipherId = 13, TagId = 4 },

				// Julius's Whisper (Caesar harder) - Historical, Tricky
				new CipherTag { CipherId = 14, TagId = 3 },
				new CipherTag { CipherId = 14, TagId = 7 },

				// The Unknown Veil (Experimental) - Puzzle, Tricky, Long
				new CipherTag { CipherId = 15, TagId = 2 },
				new CipherTag { CipherId = 15, TagId = 7 },
				new CipherTag { CipherId = 15, TagId = 5 }
			);
		}
	}
}
