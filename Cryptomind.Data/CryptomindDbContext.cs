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

			builder.Entity<Cipher>()
				.OwnsOne(c => c.LLMData, llm =>
				{
					llm.HasData(
						new { CipherId = 1, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "ROT13", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 2, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 3, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Atbash", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 4, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "SimpleSubstitution", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 5, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 6, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Autokey", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 7, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Trithemius", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 8, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "RailFence", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 9, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Columnar", Confidence = "Medium", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 10, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Base64", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 11, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Morse", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 12, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Binary", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 13, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Hex", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 14, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Caesar", Confidence = "High", SolutionCorrect = (bool?)true, IsAppropriate = (bool?)true, IsSolvable = (bool?)true, CachedHint = "", CachedSolution = "", CachedTypeHint = "" },
						new { CipherId = 15, Reasoning = (string?)null, Issues = (List<string>?)null, PredictedType = "Vigenere", Confidence = "Medium", SolutionCorrect = (bool?)null, IsAppropriate = (bool?)true, IsSolvable = (bool?)null, CachedHint = "", CachedSolution = "", CachedTypeHint = "" }
					);
				});

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
				UserName = "admin@cryptomind.com",
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
				UserName = "user@cryptomind.com",
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

			builder.Entity<TextCipher>().HasData(
				new TextCipher
				{
					Id = 1,
					Title = "The ROT13 Challenge",
					EncryptedText = "Pbclgbtencul vf gur cenpgvpr bs frphevat vasbezngvba ol genafsbezvat vg vagb na haernqnoyr sbezng. Bayl gubfr jvgu gur pbeerpg xrl pna qrpbqr gur zrffntr naq ernq vgf bevtvany pbagragf.",
					DecryptedText = "Cryptography is the practice of securing information by transforming it into an unreadable format. Only those with the correct key can decode the message and read its original contents.",
					MLPrediction = "ROT13",
					TypeOfCipher = CipherType.ROT13,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 2,
					Title = "Caesar's Secret",
					EncryptedText = "Wkh duw ri zdu lv ri ylwdo lpsruwdqfh wr wkh vwdwh. Lw lv d pdwwhu ri olih dqg ghdwk, d urdg hlwkhu wr vdihwb ru wr uxlq. Khqfh lw lv d vxemhfw ri lqtxlub zklfk fdq rq qr dffrxqw eh qhjohfwhg.",
					DecryptedText = "The art of war is of vital importance to the state. It is a matter of life and death, a road either to safety or to ruin. Hence it is a subject of inquiry which can on no account be neglected.",
					MLPrediction = "Caesar",
					TypeOfCipher = CipherType.Caesar,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 3,
					Title = "Mirror of Letters",
					EncryptedText = "Gsv hbhgvn lu xibkgltizksb wvkvmwh lm gsv hgizmtgsvm lu nzgsvnzgrixzo kiliyovnh gszg ziv yvorvevw gl yv xlnkfgzgrlmzoob rmgizxgzyov. Gsv hvxfirgh lu nlwvim vmxibkgrlm ivhg lm gsrh ulfmwzgrlm.",
					DecryptedText = "The system of cryptography depends on the strongness of mathematical problems that are believed to be computationally intractable. The secrets of modern encryption rest on this foundation.",
					MLPrediction = "Atbash",
					TypeOfCipher = CipherType.Atbash,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 15,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 4,
					Title = "Scrambled Alphabet",
					EncryptedText = "Xbyybdwhn oxo thyepc iye xc kj exoyuncxwnh cwzbo. Kbbenh xwcejke tybw mbn jyzb ze dbyobex bde'c tbnc. Ynobycezwowhn exn cbljybcco bq zbn ynjybcbnezexobwc oc exn monp xwcocnex.",
					DecryptedText = "Following old habits can keep you at an undesired place. Remove insight from the role to reach and reveal one's keys. Understanding the subtleness of the representations is the deep insight.",
					MLPrediction = "SimpleSubstitution",
					TypeOfCipher = CipherType.SimpleSubstitution,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 25,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 5,
					Title = "The Vigenere Veil",
					EncryptedText = "Lxfopvefrnhr xh qsi zyg yv aimgmrk mrhsvqexmsr fciymrk mxw wxexmwxmgep uyepmxmiw. Xli cli mw e asv ywih xs irgsHi erh higsHi xli qiwweki fewiH sr e vseH qex.",
					DecryptedText = "Cryptology is the art of hiding information beyond its statistical qualities. The key is a word used to encode and decode the message based on a road map.",
					MLPrediction = "Vigenere",
					TypeOfCipher = CipherType.Vigenere,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 30,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 6,
					Title = "Autokey Enigma",
					EncryptedText = "Bpsxozgfmz al i kmzbsyblqkibqwv aowabsz cvsfs bvs zsg qvivusa iybwzibqkittr pibsr wv bvs xtiqvbslb qbastd. Bvqa zmvrsza qb awuvoqkivbtr uwfs lmkgem bviv abivrivp xwtritxvibsmqk kqxvsfl.",
					DecryptedText = "Autokey is a sophistication where the key changes automatically based on the plaintext itself. This renders it significantly more secure than standard polyalphabetic ciphers.",
					MLPrediction = "Autokey",
					TypeOfCipher = CipherType.Autokey,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 35,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 7,
					Title = "The Trithemius Ladder",
					EncryptedText = "Alcru lr t uidlbxevlypyu mltrag, qsxpxr max tufetmlx lv yaxnxw lq t pxznetm ftqqxo ulmaxry tgr uxr. Qeb exjbiv fp t pxznetm nlqfqz pftcx bq max tufetmax ualmary mr fqvbq.",
					DecryptedText = "Trithemius is a polyalphabetic cipher, where the alphabet is shifted in a regular manner without any key. The result is a regular moving state of the alphabet shifted in order.",
					MLPrediction = "Trithemius",
					TypeOfCipher = CipherType.Trithemius,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 30,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 8,
					Title = "The Rail Fence",
					EncryptedText = "Tersoi snomto aefnaifrtnoi cne eb dseeicrphd sdneoeircd yb giieosnvbr het attniclstiias rrutscuet fo het ttxe. Het eltters rea rea rneadgrar ni a zagigzag ttanrep scroass plutefilm sialr.",
					DecryptedText = "Transposition of information can be described decoded by discovering the statistical structure of the text. The letters are rearranged in a zigzag pattern across multiple rails.",
					MLPrediction = "RailFence",
					TypeOfCipher = CipherType.RailFence,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 20,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 9,
					Title = "The Columnar Maze",
					EncryptedText = "Iitmnofra si darre ni wors nda ulnocsm, hnet arde fof yb ncolum ni a pecifsi rerod. Teh yke medinretes eth rored ni hcwhi hte nlomcsu rae arde ot decor teh tpxtliean.",
					DecryptedText = "Information is arranged in rows and columns, then read off by column in a specific order. The key determines the order in which the columns are read to recover the plaintext.",
					MLPrediction = "Columnar",
					TypeOfCipher = CipherType.Columnar,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 25,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 10,
					Title = "The Base64 Barrier",
					EncryptedText = "QmFzZTY0IGlzIG5vdCBhbiBlbmNyeXB0aW9uIG1lY2hhbmlzbSBidXQgYW4gZW5jb2RpbmcgZm9ybWF0IHRoYXQgcmVwcmVzZW50cyBiaW5hcnkgZGF0YSBpbiBhbiBBU0NJSSBzdHJpbmcgZm9ybWF0IHVzaW5nIHNpeHR5LWZvdXIgY2hhcmFjdGVycy4=",
					DecryptedText = "Base64 is not an encryption mechanism but an encoding format that represents binary data in an ASCII string format using sixty-four characters.",
					MLPrediction = "Base64",
					TypeOfCipher = CipherType.Base64,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 11,
					Title = "Dots and Dashes",
					EncryptedText = "-- --- .-. ... . / -.-. --- -.. . / .. ... / -. --- - / .- / -.-. .. .--. .... . .-. / -... ..- - / .- -. / . -. -.-. --- -.. .. -. --. / ... -.-- ... - . -- / - .... .- - / .-. . .--. .-. . ... . -. - ... / .-.. . - - . .-. ... / .- ... / ... . --.- ..- . -. -.-. . ... / --- ..-. / -.. --- - ... / .- -. -.. / -.. .- ... .... . .",
					DecryptedText = "MORSE CODE IS NOT A CIPHER BUT AN ENCODING SYSTEM THAT REPRESENTS LETTERS AS SEQUENCES OF DOTS AND DASHES",
					MLPrediction = "Morse",
					TypeOfCipher = CipherType.Morse,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 12,
					Title = "The Binary Message",
					EncryptedText = "01000010 01101001 01101110 01100001 01110010 01111001 00100000 01110101 01110011 01100101 01110011 00100000 01111010 01100101 01110010 01101111 01110011 00100000 01100001 01101110 01100100 00100000 01101111 01101110 01100101 01110011",
					DecryptedText = "Binary uses zeros and ones",
					MLPrediction = "Binary",
					TypeOfCipher = CipherType.Binary,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 13,
					Title = "Hex Decoded",
					EncryptedText = "48657820656e636f64696e6720726570726573656e74732064617461207573696e672073697874656574206469737469 6e637420737 96d626f6c73 2066726f6d 207468652068657861646563696d616c206e756d626572207379737465 6d2c207573696e67206469676974732030207468726f75676820392061 6e64206c657474657273206120 7468726f756768206620 666f722076616c7565732074656e20746f2066696674656 56e",
					DecryptedText = "Hex encoding represents data using sixteen distinct symbols from the hexadecimal number system, using digits zero through nine and letters a through f for values ten to fifteen",
					MLPrediction = "Hex",
					TypeOfCipher = CipherType.Hex,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = true,
					AllowHint = true,
					AllowSolution = true,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 10,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 14,
					Title = "Julius's Whisper",
					EncryptedText = "Mnjpncn rflbcb l asfmncnwln unwonnw jwxfanzlcrxw lwm jwcnuurpnwln. Cwn rb l exfn qlacrwp xw ylabrwp mncluub fxdwm, cwn xcsnf rb l sxfn bxurmurwp eq dwnlfrwp qfxz l efrmnf yrnad. Xwuq csxbn tsx alw arqsncb frnb elbnm xw qlab.",
					DecryptedText = "Perhaps exists a difference between information and intelligence. One is a more lasting on passing details round, the other is a more solidling by wearing from a wider picku. Only those who can ciphers rise based on fals.",
					MLPrediction = "Caesar",
					TypeOfCipher = CipherType.Caesar,
					ChallengeType = ChallengeType.Standard,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = false,
					AllowHint = true,
					AllowSolution = false,
					IsPlaintextValid = true,
					IsLLMRecommended = false,
					IsDeleted = false,
					Points = 20,
					CreatedByUserId = adminId
				},
				new TextCipher
				{
					Id = 15,
					Title = "The Unknown Veil",
					EncryptedText = "Pbzr byhgvbaf ner abg ernqvyl xabja. Guvf zrffntr znl or rapelcgrq jvgu n pvcure gung unf ab pbasvezrq fbyhgvba. Lbhe gnfx vf gb fghql gur cnggrea, fhttrfg n cbffvoyr zrgubq, naq fhowzvg lbhe ernfbarq thrff sbe pbzzhavgl irevsvpngvba.",
					DecryptedText = null,
					MLPrediction = "Vigenere",
					TypeOfCipher = null,
					ChallengeType = ChallengeType.Experimental,
					CreatedAt = seededAt,
					ApprovedAt = seededAt,
					Status = ApprovalStatus.Approved,
					AllowTypeHint = false,
					AllowHint = false,
					AllowSolution = false,
					IsPlaintextValid = false,
					IsLLMRecommended = true,
					IsDeleted = false,
					Points = 50,
					CreatedByUserId = adminId
				}
			);

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
