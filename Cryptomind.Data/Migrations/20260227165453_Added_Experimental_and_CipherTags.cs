using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cryptomind.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_Experimental_and_CipherTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 4, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 9, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 11, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 13, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 14, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 15, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 15, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 15, 7 });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "aa74da97-7e16-4bcf-beae-aa307f24a016", "AQAAAAIAAYagAAAAEB7MqnzQt5TkIe8xj1jhC75uDBUiRGKPbtuUlJKrfAjVhMcGrVwIAaX/c9P/v4CWKw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d3bb9f58-c28b-4960-83d2-a57a9cc5e641", "AQAAAAIAAYagAAAAEI3KNEEgM0Su6xBSwMKxTyE7XV3FrU2jmbuXLYfyrZp3EzDjrmEmvSjTWQrAPztbqQ==" });

            migrationBuilder.InsertData(
                table: "Cipher",
                columns: new[] { "Id", "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "AllowHint", "AllowSolution", "AllowTypeHint", "ApprovedAt", "ChallengeType", "CreatedAt", "CreatedByUserId", "DecryptedText", "DeletedAt", "EncryptedText", "EntityType", "IsDeleted", "IsLLMRecommended", "IsPlaintextValid", "MLPrediction", "Points", "RejectedAt", "RejectionReason", "Status", "Title", "TypeOfCipher" },
                values: new object[,]
                {
                    { 51, "", "", "", "High", true, false, null, "SimpleSubstitution", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "HVM SNXDJ PDYAPUPHFN UMXRMJ VPU OFUH JXDSMNFAU ZFNNMULFDJMDZM TPHV X ZPLVMN HVXH DF OMOQMN FB HVM ZFANH VXJ MEMN OXDXSMJ HF JMZPLVMN JMULPHM CMXNU FB XHHMOLHU QC HVM BPDMUH ZNCLHXDXRCUHU PD HVM KPDSJFO", "TextCipher", false, true, false, "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9534, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9534}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 4.66e-06}]}", 250, null, null, 1, "The Unbroken Seal of the Grand Inquisitor", 2 },
                    { 52, "", "", "", "High", true, false, null, "Vigenere", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "IOE NKQTXCE JTG NJYNRW PQUVRR MVQ XUVRLHUVHTBKG MGYIIXR NJA TUKSQ EHGRL GGGCIIXR ODUTNBBUCN WUTH MEWENKSP IV BR XBODKEQ BBEIYUPMWACZ FBK OZ DWEETHUDU TUTH IPZ NROSD RVMCESFTK AAW KTDZE CNFBDZE EXAMXUS HGYZDDN", "TextCipher", false, true, false, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9187, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9187}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 8.13e-06}]}", 400, null, null, 1, "Fragments from a Burned Archive", 4 },
                    { 53, "", "", "", "High", true, false, null, "Autokey", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "ZOS VBISSPII DSZT HFI SBBNP RMFSLSI TWFUVF HNGRTSMSRXCK FIWZ ZMV QAGZYNOGW UYD MLE RQVVHIQ VSAX KTW OXTAAEIP UVFSOPRV TZM HHHOUHW UIWXAFO BG I PRIFSKQIWV NNTI WMIE ATZ OGHY JSPPLKPA HRVWYVWKQIWV NTZ TXGZZR LFGQGN", "TextCipher", false, true, false, "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9023, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9023}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 9.77e-06}]}", 500, null, null, 1, "The Diplomat's Final Transmission", 5 },
                    { 54, "", "", "", "Medium", true, false, null, "Columnar", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "OLPSDAISTEOTUBBNITTOETDTDEFARNEENAHLEYEGEIRAHSEITNSAHRTIOTNENHTCBIHH#HRNIYRGCSTFADRAWHRTECE#BRCHARNIEANRCOAYOIEETEARASILEGSTMTMOTDOMNPFRDN", "TextCipher", false, true, false, "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.8934, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.8934}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 1.066e-05}]}", 350, null, null, 1, "The Rearranged Testament of Brother Aldric", 8 },
                    { 55, "", "", "", "Medium", true, false, null, "Route", null, null, false, false, false, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", null, null, "THECARTOGRAPHRIEFLSSLIAD#ECNARAEPPASILNIAEEOSNEERDIEDBEFOREFTNEISEAODEVLOSNUGNOTEGNFROAVEALINGTHEAEHHEBTTREWSNAEHVECTTFLCOORDINDTTMSNOCOTDEWRBRTHEHIDUDDIHCIHYEESSBNETPHI", "TextCipher", false, true, false, "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9056, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9056}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 9.44e-06}]}", 375, null, null, 1, "The Cartographer's Last Coordinates", 9 }
                });

            migrationBuilder.InsertData(
                table: "CipherTags",
                columns: new[] { "CipherId", "TagId" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 2, 5 },
                    { 3, 5 },
                    { 3, 6 },
                    { 4, 5 },
                    { 4, 6 },
                    { 5, 5 },
                    { 5, 6 },
                    { 6, 6 },
                    { 7, 4 },
                    { 7, 6 },
                    { 8, 6 },
                    { 9, 4 },
                    { 9, 6 },
                    { 11, 4 },
                    { 13, 3 },
                    { 14, 6 },
                    { 15, 3 },
                    { 15, 6 },
                    { 16, 3 },
                    { 16, 6 },
                    { 17, 3 },
                    { 17, 6 },
                    { 18, 3 },
                    { 18, 6 },
                    { 19, 3 },
                    { 19, 7 },
                    { 20, 3 },
                    { 20, 7 },
                    { 21, 3 },
                    { 21, 4 },
                    { 22, 3 },
                    { 22, 4 },
                    { 23, 3 },
                    { 23, 4 },
                    { 24, 2 },
                    { 24, 5 },
                    { 24, 7 },
                    { 25, 2 },
                    { 25, 5 },
                    { 25, 7 },
                    { 26, 2 },
                    { 26, 5 },
                    { 26, 7 },
                    { 27, 2 },
                    { 27, 5 },
                    { 27, 7 },
                    { 28, 2 },
                    { 28, 4 },
                    { 29, 2 },
                    { 29, 5 },
                    { 29, 7 },
                    { 30, 2 },
                    { 30, 5 },
                    { 30, 7 },
                    { 31, 2 },
                    { 31, 4 },
                    { 32, 2 },
                    { 32, 3 },
                    { 32, 7 },
                    { 33, 2 },
                    { 33, 3 },
                    { 33, 7 },
                    { 34, 2 },
                    { 34, 3 },
                    { 34, 7 },
                    { 35, 2 },
                    { 35, 3 },
                    { 35, 5 },
                    { 35, 7 },
                    { 36, 2 },
                    { 36, 3 },
                    { 36, 5 },
                    { 36, 7 },
                    { 37, 2 },
                    { 37, 3 },
                    { 37, 5 },
                    { 37, 7 },
                    { 38, 2 },
                    { 38, 3 },
                    { 38, 5 },
                    { 38, 7 },
                    { 39, 2 },
                    { 39, 3 },
                    { 39, 5 },
                    { 39, 7 },
                    { 40, 2 },
                    { 40, 5 },
                    { 40, 7 },
                    { 41, 2 },
                    { 41, 5 },
                    { 41, 7 },
                    { 42, 2 },
                    { 42, 5 },
                    { 42, 7 },
                    { 43, 2 },
                    { 43, 5 },
                    { 43, 7 },
                    { 44, 2 },
                    { 44, 7 },
                    { 45, 2 },
                    { 45, 7 },
                    { 46, 2 },
                    { 46, 7 },
                    { 47, 2 },
                    { 47, 5 },
                    { 47, 7 },
                    { 48, 2 },
                    { 48, 5 },
                    { 48, 7 },
                    { 49, 2 },
                    { 49, 5 },
                    { 49, 7 },
                    { 50, 2 },
                    { 50, 5 },
                    { 50, 7 },
                    { 51, 2 },
                    { 51, 5 },
                    { 51, 7 },
                    { 52, 2 },
                    { 52, 5 },
                    { 52, 7 },
                    { 53, 2 },
                    { 53, 5 },
                    { 53, 7 },
                    { 54, 2 },
                    { 54, 5 },
                    { 54, 7 },
                    { 55, 2 },
                    { 55, 5 },
                    { 55, 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 3, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 4, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 5, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 7, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 8, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 9, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 9, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 11, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 13, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 14, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 15, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 15, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 16, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 16, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 17, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 17, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 18, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 18, 6 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 19, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 19, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 20, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 20, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 21, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 21, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 22, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 22, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 23, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 23, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 24, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 24, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 24, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 25, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 25, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 25, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 26, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 26, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 26, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 27, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 27, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 27, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 28, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 28, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 29, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 29, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 29, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 30, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 30, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 30, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 31, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 31, 4 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 32, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 32, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 32, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 33, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 33, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 33, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 34, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 34, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 34, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 35, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 35, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 35, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 35, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 36, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 36, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 36, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 36, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 37, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 37, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 37, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 37, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 38, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 38, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 38, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 38, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 39, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 39, 3 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 39, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 39, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 40, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 40, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 40, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 41, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 41, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 41, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 42, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 42, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 42, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 43, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 43, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 43, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 44, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 44, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 45, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 45, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 46, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 46, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 47, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 47, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 47, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 48, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 48, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 48, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 49, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 49, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 49, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 50, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 50, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 50, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 51, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 51, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 51, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 52, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 52, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 52, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 53, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 53, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 53, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 54, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 54, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 54, 7 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 55, 2 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 55, 5 });

            migrationBuilder.DeleteData(
                table: "CipherTags",
                keyColumns: new[] { "CipherId", "TagId" },
                keyValues: new object[] { 55, 7 });

            migrationBuilder.DeleteData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Cipher",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d8bc6e7a-b8f8-4bac-8287-36c8242084e6", "AQAAAAIAAYagAAAAEHBxvjlYt3zocyw1KEXenTNlD26PG53wqarvxeiCLB2fuZB+KSyQ8LfHJkEGRvbj1g==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "907d3d22-4ef4-4f47-be50-09b6df8a9c7f", "AQAAAAIAAYagAAAAEACCi+80Q2sAJioI9RZPI4liUBhGtt4qnFxH/4upis+DpIE/YfEviIi5+/34kbTJRQ==" });

            migrationBuilder.InsertData(
                table: "CipherTags",
                columns: new[] { "CipherId", "TagId" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 2, 3 },
                    { 3, 3 },
                    { 3, 4 },
                    { 4, 2 },
                    { 4, 7 },
                    { 5, 2 },
                    { 5, 7 },
                    { 6, 2 },
                    { 6, 7 },
                    { 7, 2 },
                    { 7, 3 },
                    { 8, 2 },
                    { 9, 2 },
                    { 9, 7 },
                    { 11, 5 },
                    { 13, 4 },
                    { 14, 7 },
                    { 15, 2 },
                    { 15, 5 },
                    { 15, 7 }
                });
        }
    }
}
