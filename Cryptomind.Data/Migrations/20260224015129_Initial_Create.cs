using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    SolvedCount = table.Column<int>(type: "int", nullable: false),
                    AttemptedCiphers = table.Column<int>(type: "int", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false),
                    BanReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaderBoardPlace = table.Column<int>(type: "int", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    EarnedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cipher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DecryptedText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncryptedText = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MLPrediction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LLMData_Reasoning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_Issues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_PredictedType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_Confidence = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_SolutionCorrect = table.Column<bool>(type: "bit", nullable: true),
                    LLMData_IsAppropriate = table.Column<bool>(type: "bit", nullable: true),
                    LLMData_IsSolvable = table.Column<bool>(type: "bit", nullable: true),
                    LLMData_CachedHint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_CachedSolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LLMData_CachedTypeHint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfCipher = table.Column<int>(type: "int", nullable: true),
                    ChallengeType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllowTypeHint = table.Column<bool>(type: "bit", nullable: false),
                    AllowHint = table.Column<bool>(type: "bit", nullable: false),
                    AllowSolution = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPlaintextValid = table.Column<bool>(type: "bit", nullable: false),
                    IsLLMRecommended = table.Column<bool>(type: "bit", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OCRConfidence = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cipher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cipher_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBadge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BadgeId = table.Column<int>(type: "int", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBadge_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBadge_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DecryptedText = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    UplodaedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerSuggestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerSuggestions_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CipherTags",
                columns: table => new
                {
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CipherTags", x => new { x.CipherId, x.TagId });
                    table.ForeignKey(
                        name: "FK_CipherTags_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CipherTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HintRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HintType = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HintContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HintRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HintRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HintRequests_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSolution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CipherId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeSolved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    UsedTypeHint = table.Column<bool>(type: "bit", nullable: false),
                    UsedSolutionHint = table.Column<bool>(type: "bit", nullable: false),
                    UsedFullSolution = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSolution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSolution_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSolution_Cipher_CipherId",
                        column: x => x.CipherId,
                        principalTable: "Cipher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c3d4e5f6-a7b8-9012-cdef-123456789012", null, "Admin", "ADMIN" },
                    { "d4e5f6a7-b8c9-0123-defa-234567890123", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AttemptedCiphers", "BanReason", "BannedAt", "ConcurrencyStamp", "DeactivatedAt", "Email", "EmailConfirmed", "IsBanned", "IsDeactivated", "LeaderBoardPlace", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RegisteredAt", "Score", "SecurityStamp", "SolvedCount", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "a1b2c3d4-e5f6-7890-abcd-ef1234567890", 0, 0, null, null, "f299abbc-2718-41f9-879c-e8a0ed7c4287", null, "admin@cryptomind.com", true, false, false, 0, false, null, "ADMIN@CRYPTOMIND.COM", "ADMIN@CRYPTOMIND.COM", "AQAAAAIAAYagAAAAEL0uZxT/Sr1G8eXKFV1uCrc4x85jN6fSgS99819Fs77TaWr9gWYdhMGHSRtX/7eSUQ==", null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "a1a1a1a1-b2b2-c3c3-d4d4-e5e5e5e5e5e5", 0, false, "admin@cryptomind.com" },
                    { "b2c3d4e5-f6a7-8901-bcde-f12345678901", 0, 0, null, null, "6105ecf1-3ed5-4a00-911a-01bf403bc200", null, "user@cryptomind.com", true, false, false, 0, false, null, "USER@CRYPTOMIND.COM", "USER@CRYPTOMIND.COM", "AQAAAAIAAYagAAAAEHp55MEeP28NmVCeCm5hscaT6py6TfwMxBguugamAOnTk/gU3Kt1rDQtqVf9a1VVmQ==", null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "f6f6f6f6-e5e5-d4d4-c3c3-b2b2b2b2b2b2", 0, false, "user@cryptomind.com" }
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "Category", "Description", "EarnedBy", "ImagePath", "Title" },
                values: new object[,]
                {
                    { 1, 0, "Solve your first cipher", 0, "../Images/Badges/Badge_1.png", "First Blood" },
                    { 2, 0, "Solve 25 ciphers", 0, "../Images/Badges/Badge_2.png", "Apprentice Cryptanalyst" },
                    { 3, 0, "Solve 50 ciphers", 0, "../Images/Badges/Badge_3.png", "Seasoned Decoder" },
                    { 4, 0, "Solve 100 ciphers", 0, "../Images/Badges/Badge_4.png", "Master Cryptanalyst" },
                    { 5, 0, "Solve ciphers from 5 different types", 0, "../Images/Badges/Badge_5.png", "Diverse Solver" },
                    { 6, 0, "Solve ciphers from 10 different types", 0, "../Images/Badges/Badge_6.png", "Polyglot Decoder" },
                    { 7, 2, "Have your first cipher approved", 0, "../Images/Badges/Badge_7.png", "Cipher Creator" },
                    { 8, 2, "Have 5 ciphers approved", 0, "../Images/Badges/Badge_8.png", "Community Contributor" },
                    { 9, 2, "Have 15 ciphers approved", 0, "../Images/Badges/Badge_9.png", "Architect of Ciphers" },
                    { 10, 1, "First approved suggested answer", 0, "../Images/Badges/Badge_10.png", "Helpful Mind" },
                    { 11, 1, "10 approved suggested answers", 0, "../Images/Badges/Badge_11.png", "Trusted Contributor" },
                    { 12, 0, "Solve 10 ciphers without using hints", 0, "../Images/Badges/Badge_12.png", "No Mercy" },
                    { 13, 0, "Solve 10 ciphers correctly on the first attempt", 0, "../Images/Badges/Badge_13.png", "Flawless Solver" },
                    { 14, 0, "Use hints on 25 different ciphers", 0, "../Images/Badges/Badge_14.png", "Curious Mind" },
                    { 15, 0, "Solve a cipher solved by fewer than 3 users", 0, "../Images/Badges/Badge_15.png", "Against the Odds" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 7, 7 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c3d4e5f6-a7b8-9012-cdef-123456789012", "a1b2c3d4-e5f6-7890-abcd-ef1234567890" },
                    { "d4e5f6a7-b8c9-0123-defa-234567890123", "b2c3d4e5-f6a7-8901-bcde-f12345678901" }
                });

            migrationBuilder.InsertData(
                table: "Cipher",
                columns: new[] { "Id", "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "AllowHint", "AllowSolution", "AllowTypeHint", "ApprovedAt", "ChallengeType", "CreatedAt", "CreatedByUserId", "DecryptedText", "DeletedAt", "EncryptedText", "EntityType", "IsDeleted", "IsLLMRecommended", "IsPlaintextValid", "MLPrediction", "Points", "RejectedAt", "RejectionReason", "Status", "Title", "TypeOfCipher" },
                values: new object[,]
                {
                    { 1, "", "", "", "High", true, true, null, "ROT13", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Cryptography is the practice of securing information by transforming it into an unreadable format. Only those with the correct key can decode the message and read its original contents.", null, "Pbclgbtencul vf gur cenpgvpr bs frphevat vasbezngvba ol genafsbezvat vg vagb na haernqnoyr sbezng. Bayl gubfr jvgu gur pbeerpg xrl pna qrpbqr gur zrffntr naq ernq vgf bevtvany pbagragf.", "TextCipher", false, false, true, "ROT13", 10, null, null, 1, "The ROT13 Challenge", 3 },
                    { 2, "", "", "", "High", true, true, null, "Caesar", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "The art of war is of vital importance to the state. It is a matter of life and death, a road either to safety or to ruin. Hence it is a subject of inquiry which can on no account be neglected.", null, "Wkh duw ri zdu lv ri ylwdo lpsruwdqfh wr wkh vwdwh. Lw lv d pdwwhu ri olih dqg ghdwk, d urdg hlwkhu wr vdihwb ru wr uxlq. Khqfh lw lv d vxemhfw ri lqtxlub zklfk fdq rq qr dffrxqw eh qhjohfwhg.", "TextCipher", false, false, true, "Caesar", 10, null, null, 1, "Caesar's Secret", 0 },
                    { 3, "", "", "", "High", true, true, null, "Atbash", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "The system of cryptography depends on the strongness of mathematical problems that are believed to be computationally intractable. The secrets of modern encryption rest on this foundation.", null, "Gsv hbhgvn lu xibkgltizksb wvkvmwh lm gsv hgizmtgsvm lu nzgsvnzgrixzo kiliyovnh gszg ziv yvorvevw gl yv xlnkfgzgrlmzoob rmgizxgzyov. Gsv hvxfirgh lu nlwvim vmxibkgrlm ivhg lm gsrh ulfmwzgrlm.", "TextCipher", false, false, true, "Atbash", 15, null, null, 1, "Mirror of Letters", 1 },
                    { 4, "", "", "", "High", true, true, null, "SimpleSubstitution", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Following old habits can keep you at an undesired place. Remove insight from the role to reach and reveal one's keys. Understanding the subtleness of the representations is the deep insight.", null, "Xbyybdwhn oxo thyepc iye xc kj exoyuncxwnh cwzbo. Kbbenh xwcejke tybw mbn jyzb ze dbyobex bde'c tbnc. Ynobycezwowhn exn cbljybcco bq zbn ynjybcbnezexobwc oc exn monp xwcocnex.", "TextCipher", false, false, true, "SimpleSubstitution", 25, null, null, 1, "Scrambled Alphabet", 2 },
                    { 5, "", "", "", "High", true, true, null, "Vigenere", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Cryptology is the art of hiding information beyond its statistical qualities. The key is a word used to encode and decode the message based on a road map.", null, "Lxfopvefrnhr xh qsi zyg yv aimgmrk mrhsvqexmsr fciymrk mxw wxexmwxmgep uyepmxmiw. Xli cli mw e asv ywih xs irgsHi erh higsHi xli qiwweki fewiH sr e vseH qex.", "TextCipher", false, false, true, "Vigenere", 30, null, null, 1, "The Vigenere Veil", 4 },
                    { 6, "", "", "", "High", true, true, null, "Autokey", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Autokey is a sophistication where the key changes automatically based on the plaintext itself. This renders it significantly more secure than standard polyalphabetic ciphers.", null, "Bpsxozgfmz al i kmzbsyblqkibqwv aowabsz cvsfs bvs zsg qvivusa iybwzibqkittr pibsr wv bvs xtiqvbslb qbastd. Bvqa zmvrsza qb awuvoqkivbtr uwfs lmkgem bviv abivrivp xwtritxvibsmqk kqxvsfl.", "TextCipher", false, false, true, "Autokey", 35, null, null, 1, "Autokey Enigma", 5 },
                    { 7, "", "", "", "High", true, true, null, "Trithemius", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Trithemius is a polyalphabetic cipher, where the alphabet is shifted in a regular manner without any key. The result is a regular moving state of the alphabet shifted in order.", null, "Alcru lr t uidlbxevlypyu mltrag, qsxpxr max tufetmlx lv yaxnxw lq t pxznetm ftqqxo ulmaxry tgr uxr. Qeb exjbiv fp t pxznetm nlqfqz pftcx bq max tufetmax ualmary mr fqvbq.", "TextCipher", false, false, true, "Trithemius", 30, null, null, 1, "The Trithemius Ladder", 6 },
                    { 8, "", "", "", "High", true, true, null, "RailFence", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Transposition of information can be described decoded by discovering the statistical structure of the text. The letters are rearranged in a zigzag pattern across multiple rails.", null, "Tersoi snomto aefnaifrtnoi cne eb dseeicrphd sdneoeircd yb giieosnvbr het attniclstiias rrutscuet fo het ttxe. Het eltters rea rea rneadgrar ni a zagigzag ttanrep scroass plutefilm sialr.", "TextCipher", false, false, true, "RailFence", 20, null, null, 1, "The Rail Fence", 7 },
                    { 9, "", "", "", "Medium", true, true, null, "Columnar", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Information is arranged in rows and columns, then read off by column in a specific order. The key determines the order in which the columns are read to recover the plaintext.", null, "Iitmnofra si darre ni wors nda ulnocsm, hnet arde fof yb ncolum ni a pecifsi rerod. Teh yke medinretes eth rored ni hcwhi hte nlomcsu rae arde ot decor teh tpxtliean.", "TextCipher", false, false, true, "Columnar", 25, null, null, 1, "The Columnar Maze", 8 },
                    { 10, "", "", "", "High", true, true, null, "Base64", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Base64 is not an encryption mechanism but an encoding format that represents binary data in an ASCII string format using sixty-four characters.", null, "QmFzZTY0IGlzIG5vdCBhbiBlbmNyeXB0aW9uIG1lY2hhbmlzbSBidXQgYW4gZW5jb2RpbmcgZm9ybWF0IHRoYXQgcmVwcmVzZW50cyBiaW5hcnkgZGF0YSBpbiBhbiBBU0NJSSBzdHJpbmcgZm9ybWF0IHVzaW5nIHNpeHR5LWZvdXIgY2hhcmFjdGVycy4=", "TextCipher", false, false, true, "Base64", 10, null, null, 1, "The Base64 Barrier", 10 },
                    { 11, "", "", "", "High", true, true, null, "Morse", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "MORSE CODE IS NOT A CIPHER BUT AN ENCODING SYSTEM THAT REPRESENTS LETTERS AS SEQUENCES OF DOTS AND DASHES", null, "-- --- .-. ... . / -.-. --- -.. . / .. ... / -. --- - / .- / -.-. .. .--. .... . .-. / -... ..- - / .- -. / . -. -.-. --- -.. .. -. --. / ... -.-- ... - . -- / - .... .- - / .-. . .--. .-. . ... . -. - ... / .-.. . - - . .-. ... / .- ... / ... . --.- ..- . -. -.-. . ... / --- ..-. / -.. --- - ... / .- -. -.. / -.. .- ... .... . .", "TextCipher", false, false, true, "Morse", 10, null, null, 1, "Dots and Dashes", 11 },
                    { 12, "", "", "", "High", true, true, null, "Binary", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Binary uses zeros and ones", null, "01000010 01101001 01101110 01100001 01110010 01111001 00100000 01110101 01110011 01100101 01110011 00100000 01111010 01100101 01110010 01101111 01110011 00100000 01100001 01101110 01100100 00100000 01101111 01101110 01100101 01110011", "TextCipher", false, false, true, "Binary", 10, null, null, 1, "The Binary Message", 12 },
                    { 13, "", "", "", "High", true, true, null, "Hex", null, true, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Hex encoding represents data using sixteen distinct symbols from the hexadecimal number system, using digits zero through nine and letters a through f for values ten to fifteen", null, "48657820656e636f64696e6720726570726573656e74732064617461207573696e672073697874656574206469737469 6e637420737 96d626f6c73 2066726f6d 207468652068657861646563696d616c206e756d626572207379737465 6d2c207573696e67206469676974732030207468726f75676820392061 6e64206c657474657273206120 7468726f756768206620 666f722076616c7565732074656e20746f2066696674656 56e", "TextCipher", false, false, true, "Hex", 10, null, null, 1, "Hex Decoded", 13 },
                    { 14, "", "", "", "High", true, true, null, "Caesar", null, true, true, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "Perhaps exists a difference between information and intelligence. One is a more lasting on passing details round, the other is a more solidling by wearing from a wider picku. Only those who can ciphers rise based on fals.", null, "Mnjpncn rflbcb l asfmncnwln unwonnw jwxfanzlcrxw lwm jwcnuurpnwln. Cwn rb l exfn qlacrwp xw ylabrwp mncluub fxdwm, cwn xcsnf rb l sxfn bxurmurwp eq dwnlfrwp qfxz l efrmnf yrnad. Xwuq csxbn tsx alw arqsncb frnb elbnm xw qlab.", "TextCipher", false, false, true, "Caesar", 20, null, null, 1, "Julius's Whisper", 0 },
                    { 15, "", "", "", "Medium", true, null, null, "Vigenere", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "Pbzr byhgvbaf ner abg ernqvyl xabja. Guvf zrffntr znl or rapelcgrq jvgu n pvcure gung unf ab pbasvezrq fbyhgvba. Lbhe gnfx vf gb fghql gur cnggrea, fhttrfg n cbffvoyr zrgubq, naq fhowzvg lbhe ernfbarq thrff sbe pbzzhavgl irevsvpngvba.", "TextCipher", false, true, false, "Vigenere", 50, null, null, 1, "The Unknown Veil", null }
                });

            migrationBuilder.InsertData(
                table: "CipherTags",
                columns: new[] { "CipherId", "TagId" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 1, 6 },
                    { 2, 3 },
                    { 2, 6 },
                    { 3, 3 },
                    { 3, 4 },
                    { 4, 2 },
                    { 4, 7 },
                    { 5, 2 },
                    { 5, 7 },
                    { 6, 2 },
                    { 6, 5 },
                    { 6, 7 },
                    { 7, 2 },
                    { 7, 3 },
                    { 8, 2 },
                    { 8, 4 },
                    { 9, 2 },
                    { 9, 7 },
                    { 10, 4 },
                    { 10, 6 },
                    { 11, 5 },
                    { 11, 6 },
                    { 12, 4 },
                    { 12, 6 },
                    { 13, 4 },
                    { 13, 6 },
                    { 14, 3 },
                    { 14, 7 },
                    { 15, 2 },
                    { 15, 5 },
                    { 15, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_CipherId",
                table: "AnswerSuggestions",
                column: "CipherId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSuggestions_UserId_CipherId_DecryptedText",
                table: "AnswerSuggestions",
                columns: new[] { "UserId", "CipherId", "DecryptedText" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_CreatedByUserId",
                table: "Cipher",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_EncryptedText",
                table: "Cipher",
                column: "EncryptedText",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cipher_Title",
                table: "Cipher",
                column: "Title",
                unique: true,
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CipherTags_TagId",
                table: "CipherTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_HintRequests_CipherId",
                table: "HintRequests",
                column: "CipherId");

            migrationBuilder.CreateIndex(
                name: "IX_HintRequests_UserId",
                table: "HintRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadge_BadgeId",
                table: "UserBadge",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadge_UserId_BadgeId",
                table: "UserBadge",
                columns: new[] { "UserId", "BadgeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_CipherId",
                table: "UserSolution",
                column: "CipherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSolution_UserId_CipherId",
                table: "UserSolution",
                columns: new[] { "UserId", "CipherId" },
                unique: true,
                filter: "[IsCorrect] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerSuggestions");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CipherTags");

            migrationBuilder.DropTable(
                name: "HintRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserBadge");

            migrationBuilder.DropTable(
                name: "UserSolution");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "Cipher");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
