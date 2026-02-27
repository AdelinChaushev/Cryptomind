using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cryptomind.Data.Migrations
{
	/// <inheritdoc />
	public partial class SeedCiphers : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "UserName" },
				values: new object[] { "cfb253e0-abf1-4afb-9cca-10b2e1089638", "AQAAAAIAAYagAAAAENXkDwhSp6K2s7Nijjph8Vr+5QzRF3SAm2MLDO6SFroVNlnCshL/Wm1A5sDg4c2Ftg==", "Admin" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "UserName" },
				values: new object[] { "6534036f-c744-4fc8-9fe2-c2eb933acd26", "AQAAAAIAAYagAAAAEMLHQUHLCew7oD8lXcE9FdnyYQy5YPfCq2rzkXJ3AoEuETt8eMu0yETY5h1S0Ia45w==", "User" });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 1,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "The ancient art of cryptography has protected secrets for thousands of years from the earliest Egyptian scribes to modern digital encryption systems", "VGhlIGFuY2llbnQgYXJ0IG9mIGNyeXB0b2dyYXBoeSBoYXMgcHJvdGVjdGVkIHNlY3JldHMgZm9yIHRob3VzYW5kcyBvZiB5ZWFycyBmcm9tIHRoZSBlYXJsaWVzdCBFZ3lwdGlhbiBzY3JpYmVzIHRvIG1vZGVybiBkaWdpdGFsIGVuY3J5cHRpb24gc3lzdGVtcw==", "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9978, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9978}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.2e-07}]}", 50, "The Merchant's Hidden Ledger", 10 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 2,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "Julius Caesar famously used a simple letter shift cipher to communicate with his generals during military campaigns across the vast Roman Empire", "SnVsaXVzIENhZXNhciBmYW1vdXNseSB1c2VkIGEgc2ltcGxlIGxldHRlciBzaGlmdCBjaXBoZXIgdG8gY29tbXVuaWNhdGUgd2l0aCBoaXMgZ2VuZXJhbHMgZHVyaW5nIG1pbGl0YXJ5IGNhbXBhaWducyBhY3Jvc3MgdGhlIHZhc3QgUm9tYW4gRW1waXJl", "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9965, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9965}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.5e-07}]}", 50, "Whispers from the Ivory Tower", 10 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 3,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "The Enigma machine used by Germany in the Second World War was considered unbreakable until brilliant mathematicians at Bletchley Park finally cracked it", "VGhlIEVuaWdtYSBtYWNoaW5lIHVzZWQgYnkgR2VybWFueSBpbiB0aGUgU2Vjb25kIFdvcmxkIFdhciB3YXMgY29uc2lkZXJlZCB1bmJyZWFrYWJsZSB1bnRpbCBicmlsbGlhbnQgbWF0aGVtYXRpY2lhbnMgYXQgQmxldGNobGV5IFBhcmsgZmluYWxseSBjcmFja2VkIGl0", "{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9982, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 0.9982}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 1.8e-07}]}", 50, "The Sealed Dispatch of Bletchley", 10 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 4,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "Cryptography is the science of hiding information from unintended recipients using mathematical transformations and carefully guarded secret keys", "43727970746f6772617068792069732074686520736369656e6365206f6620686964696e6720696e666f726d6174696f6e2066726f6d20756e696e74656e64656420726563697069656e7473207573696e67206d617468656d61746963616c207472616e73666f726d6174696f6e7320616e64206361726566756c6c79206775617264656420736563726574206b657973", "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9971, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9971}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 2.9e-07}]}", 50, "The Archivist's Numbered Scroll", 13 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 5,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "The Vigenere cipher was considered unbreakable for three centuries earning it the famous title of the indecipherable cipher among cryptographers", "54686520566967656e657265206369706865722077617320636f6e7369646572656420756e627265616b61626c6520666f722074687265652063656e747572696573206561726e696e67206974207468652066616d6f7573207469746c65206f662074686520696e646563697068657261626c652063697068657220616d6f6e672063727970746f6772617068657273", "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9968, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9968}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 3.2e-07}]}", 50, "Manuscript of the Indecipherable", 13 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 6,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "Modern encryption algorithms protect billions of digital transactions every day keeping financial data and private communications secure from hackers", "4d6f6465726e20656e6372797074696f6e20616c676f726974686d732070726f746563742062696c6c696f6e73206f66206469676974616c207472616e73616374696f6e7320657665727920646179206b656570696e672066696e616e6369616c206461746120616e64207072697661746520636f6d6d756e69636174696f6e73207365637572652066726f6d206861636b657273", "{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9975, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 0.9975}, {\"family\": \"Encoding\", \"type\": \"Base64\", \"confidence\": 2.5e-07}]}", 50, "The Digital Vault Inscription", 13 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 7,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "HIDE THE GOLD IN THE CAVE", "01001000 01001001 01000100 01000101 00100000 01010100 01001000 01000101 00100000 01000111 01001111 01001100 01000100 00100000 01001001 01001110 00100000 01010100 01001000 01000101 00100000 01000011 01000001 01010110 01000101", "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9969, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9969}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.1e-07}]}", 50, "The Alchemist's Binary Rune", 12 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 8,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "ATTACK AT DAWN BRING FORCES", "01000001 01010100 01010100 01000001 01000011 01001011 00100000 01000001 01010100 00100000 01000100 01000001 01010111 01001110 00100000 01000010 01010010 01001001 01001110 01000111 00100000 01000110 01001111 01010010 01000011 01000101 01010011", "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9963, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9963}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.7e-07}]}", 50, "Signals from the Iron Watchtower", 12 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 9,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "MEET AT THE BRIDGE TONIGHT", "01001101 01000101 01000101 01010100 00100000 01000001 01010100 00100000 01010100 01001000 01000101 00100000 01000010 01010010 01001001 01000100 01000111 01000101 00100000 01010100 01001111 01001110 01001001 01000111 01001000 01010100", "{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9971, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Binary\", \"confidence\": 0.9971}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.9e-07}]}", 50, "The Clockmaker's Pulse Sequence", 12 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 10,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "THE EAGLE HAS LANDED PROCEED TO EXTRACTION", "- .... . / . .- --. .-.. . / .... .- ... / .-.. .- -. -.. . -.. / .--. .-. --- -.-. . . -.. / - --- / . -..- - .-. .- -.-. - .. --- -.", "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9974, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9974}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.6e-07}]}", 50, "Dots and Dashes from the Frontier", 11 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 11,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title" },
				values: new object[] { "SEND REINFORCEMENTS TO THE NORTHERN BORDER", "... . -. -.. / .-. . .. -. ..-. --- .-. -.-. . -- . -. - ... / - --- / - .... . / -. --- .-. - .... . .-. -. / -... --- .-. -.. . .-.", "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9961, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9961}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 3.9e-07}]}", 50, "The Operator's Last Transmission" });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 12,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "CIPHER DECODED ADVANCE TO SECONDARY POSITION", "-.-. .. .--. .... . .-. / -.. . -.-. --- -.. . -.. / .- -.. ...- .- -. -.-. . / - --- / ... . -.-. --- -. -.. .- .-. -.-- / .--. --- ... .. - .. --- -.", "{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9978, \"allPredictions\": [{\"family\": \"Encoding\", \"type\": \"Morse\", \"confidence\": 0.9978}, {\"family\": \"Encoding\", \"type\": \"Hex\", \"confidence\": 2.2e-07}]}", 50, "Echoes Through the Telegraph Wire", 11 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 13,
				columns: new[] { "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG WHILE THE HOUND WATCHED FROM BEHIND THE OLD OAK TREE IN THE SUNNY MEADOW", "GUR DHVPX OEBJA SBK WHZCRQ BIRE GUR YNML QBT JUVYR GUR UBHAQ JNGPURQ SEBZ ORUVAQ GUR BYQ BNX GERR VA GUR FHAAL ZRNQBJ", "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9891, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9891}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.09e-06}]}", 75, "The Mirrored Philosopher's Note", 3 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 14,
				columns: new[] { "AllowSolution", "AllowTypeHint", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { true, true, "CRYPTOGRAPHY IS THE PRACTICE OF SECURING COMMUNICATIONS FROM ADVERSARIES WHO MIGHT INTERCEPT AND READ PRIVATE MESSAGES BETWEEN TRUSTED PARTIES", "PELCGBTENCUL VF GUR CENPGVPR BS FRPHEVAT PBZZHAVPNGVBAF SEBZ NQIREFNEVRF JUB ZVTUG VAGREPRCG NAQ ERNQ CEVINGR ZRFFNTRF ORGJRRA GEHFGRQ CNEGVRF", "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9847, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9847}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.53e-06}]}", 75, "A Letter Left in the Looking Glass", 3 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 15,
				columns: new[] { "AllowHint", "AllowSolution", "AllowTypeHint", "ChallengeType", "DecryptedText", "EncryptedText", "IsLLMRecommended", "IsPlaintextValid", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { true, true, true, 0, "THE CAESAR CIPHER IS ONE OF THE OLDEST AND MOST WIDELY KNOWN ENCRYPTION TECHNIQUES IN THE ENTIRE HISTORY OF SECRET COMMUNICATION AND CRYPTANALYSIS", "GUR PNRFNE PVCURE VF BAR BS GUR BYQRFG NAQ ZBFG JVQRYL XABJA RAPELCGVBA GRPUAVDHRF VA GUR RAGVER UVFGBEL BS FRPERG PBZZHAVPNGVBA NAQ PELCGNANYLFVF", false, true, "{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9863, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"ROT13\", \"confidence\": 0.9863}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.37e-06}]}", 75, "The Reversed Scholar's Testimony", 3 });

			migrationBuilder.InsertData(
				table: "Cipher",
				columns: new[] { "Id", "AllowHint", "AllowSolution", "AllowTypeHint", "ApprovedAt", "ChallengeType", "CreatedAt", "CreatedByUserId", "DecryptedText", "DeletedAt", "EncryptedText", "EntityType", "IsDeleted", "IsLLMRecommended", "IsPlaintextValid", "MLPrediction", "Points", "RejectedAt", "RejectionReason", "Status", "Title", "TypeOfCipher" },
				values: new object[,]
				{
					{ 16, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE ANCIENT ROMANS DEVELOPED MANY SYSTEMS FOR SENDING SECRET MESSAGES ACROSS THEIR VAST EMPIRE WITHOUT ENEMIES BEING ABLE TO READ THE CONTENTS", null, "WKH DQFLHQW URPDQV GHYHORSHG PDQB VBVWHPV IRU VHQGLQJ VHFUHW PHVVDJHV DFURVV WKHLU YDVW HPSLUH ZLWKRXW HQHPLHV EHLQJ DEOH WR UHDG WKH FRQWHQWV", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9913, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9913}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 8.7e-07}]}", 100, null, null, 1, "The Emperor's Forgotten Orders", 0 },
					{ 17, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "SOLDIERS MARCHING THROUGH THE DARK FOREST MUST REMAIN COMPLETELY SILENT AND COMMUNICATE ONLY THROUGH PREARRANGED CODED SIGNALS AND SECRET MESSAGES", null, "ZVSKPLYZ THYJOPUN AOYVBNO AOL KHYR MVYLZA TBZA YLTHPU JVTWSLALSF ZPSLUA HUK JVTTBUPJHAL VUSF AOYVBNO WYLHYYHUNLK JVKLK ZPNUHSZ HUK ZLJYLA TLZZHNLZ", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9887, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9887}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.13e-06}]}", 100, null, null, 1, "A Soldier's Note from the Dark Forest", 0 },
					{ 18, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "IN THE DEPTHS OF THE ROYAL LIBRARY THERE EXISTS AN ANCIENT MANUSCRIPT DESCRIBING THE METHODS USED BY SPIES TO CONCEAL THEIR SECRET COMMUNICATIONS", null, "TY ESP OPAESD ZQ ESP CZJLW WTMCLCJ ESPCP PITDED LY LYNTPYE XLYFDNCTAE OPDNCTMTYR ESP XPESZOD FDPO MJ DATPD EZ NZYNPLW ESPTC DPNCPE NZXXFYTNLETZYD", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9901, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9901}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 9.9e-07}]}", 100, null, null, 1, "Parchment from the Royal Library", 0 },
					{ 19, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE MASTER OF SECRETS WALKED SLOWLY THROUGH THE MARKETPLACE PASSING CODED MESSAGES HIDDEN INSIDE ORDINARY LOOKING OBJECTS TO HIS TRUSTED NETWORK", null, "KYV DRJKVI FW JVTIVKJ NRCBVU JCFNCP KYIFLXY KYV DRIBVKGCRTV GRJJZEX TFUVU DVJJRXVJ YZUUVE ZEJZUV FIUZERIP CFFBZEX FSAVTKJ KF YZJ KILJKVU EVKNFIB", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9878, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9878}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 1.22e-06}]}", 100, null, null, 1, "The Spymaster's Market Errand", 0 },
					{ 20, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "BEYOND THE MOUNTAIN RANGE LIES A HIDDEN VALLEY WHERE THE SECRET ORDER OF CRYPTOGRAPHERS HAS MAINTAINED ITS ANCIENT TRADITION OF SECRET KEEPING", null, "WZTJIY OCZ HJPIOVDI MVIBZ GDZN V CDYYZI QVGGZT RCZMZ OCZ NZXMZO JMYZM JA XMTKOJBMVKCZMN CVN HVDIOVDIZY DON VIXDZIO OMVYDODJI JA NZXMZO FZZKDIB", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9924, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 0.9924}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 7.6e-07}]}", 100, null, null, 1, "Beyond the Northern Mountain Pass", 0 },
					{ 21, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE ANCIENT HEBREW SCHOLARS USED THIS ELEGANT CIPHER TO CONCEAL SACRED TEXTS FROM THOSE WHO WERE NOT INITIATED INTO THEIR SECRET TRADITIONS", null, "GSV ZMXRVMG SVYIVD HXSLOZIH FHVW GSRH VOVTZMG XRKSVI GL XLMXVZO HZXIVW GVCGH UILN GSLHV DSL DVIV MLG RMRGRZGVW RMGL GSVRI HVXIVG GIZWRGRLMH", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9756, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9756}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.44e-06}]}", 125, null, null, 1, "The Hebrew Scribe's Sacred Text", 1 },
					{ 22, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "WHEN THE SPYMASTER RECEIVED THE ENCODED LETTER HE QUICKLY REVERSED EACH CHARACTER AND DECODED THE URGENT MESSAGE ABOUT THE ENEMY TROOP MOVEMENTS", null, "DSVM GSV HKBNZHGVI IVXVREVW GSV VMXLWVW OVGGVI SV JFRXPOB IVEVIHVW VZXS XSZIZXGVI ZMW WVXLWVW GSV FITVMG NVHHZTV ZYLFG GSV VMVNB GILLK NLEVNVMGH", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9712, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9712}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.88e-06}]}", 125, null, null, 1, "Intercepted Letter from the Southern Court", 1 },
					{ 23, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE SIMPLEST FORM OF SUBSTITUTION MAPS EACH LETTER TO ITS MIRROR IMAGE WHERE A BECOMES Z AND B BECOMES Y CONTINUING THROUGHOUT THE ENTIRE ALPHABET", null, "GSV HRNKOVHG ULIN LU HFYHGRGFGRLM NZKH VZXS OVGGVI GL RGH NRIILI RNZTV DSVIV Z YVXLNVH A ZMW Y YVXLNVH B XLMGRMFRMT GSILFTSLFG GSV VMGRIV ZOKSZYVG", "TextCipher", false, false, true, "{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9788, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"Atbash\", \"confidence\": 0.9788}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 2.12e-06}]}", 125, null, null, 1, "The Mirror Inscription of Alexandria", 1 },
					{ 24, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE MYSTERIOUS LETTER ARRIVED AT MIDNIGHT CONTAINING A SECRET MESSAGE THAT ONLY THOSE WHO POSSESSED THE CIPHER KEY COULD POSSIBLY HOPE TO DECIPHER", null, "ZIT DNLZTKOGXL STZZTK QKKOCTR QZ DORFOUIZ EGFZQOFOFU Q LTEKTZ DTLLQUT ZIQZ GFSN ZIGLT VIG HGLLTLLTR ZIT EOHITK ATN EGXSR HGLLOWSN IGHT ZG RTEOHITK", "TextCipher", false, true, true, "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9634, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9634}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 3.66e-06}]}", 250, null, null, 1, "The Midnight Letter", 2 },
					{ 25, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "HIDDEN BENEATH THE FLOORBOARDS OF THE OLD TAVERN WAS A SEALED BOX CONTAINING THE CIPHER KEY AND SEVERAL ENCODED MESSAGES FROM THE UNDERGROUND NETWORK", null, "LKVVCD NCDCMUL ULC XGSSONSMOVI SX ULC SGV UMTCOD RMI M ICMGCV NSE BSDUMKDKDZ ULC BKALCO HCW MDV ICTCOMG CDBSVCV FCIIMZCI XOSF ULC YDVCOZOSYDV DCURSOH", "TextCipher", false, true, true, "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9591, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9591}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 4.09e-06}]}", 250, null, null, 1, "Beneath the Old Tavern Floor", 2 },
					{ 26, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE ENCODED DISPATCH FROM THE SOUTHERN FRONT DESCRIBED THE POSITIONS OF ENEMY FORTIFICATIONS AND THE ROUTES AVAILABLE FOR THE ADVANCING CAVALRY COLUMN", null, "CEI IFQDGIG GARXPCQE UKDO CEI RDVCEIKF UKDFC GIRQKAHIG CEI XDRACADFR DU IFIOW UDKCAUAQPCADFR PFG CEI KDVCIR PSPANPHNI UDK CEI PGSPFQAFM QPSPNKW QDNVOF", "TextCipher", false, true, true, "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9647, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9647}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 3.53e-06}]}", 250, null, null, 1, "The Southern Front Dispatch", 2 },
					{ 27, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "DEEP IN THE ARCHIVES OF THE ROYAL CIPHER OFFICE THERE ARE THOUSANDS OF ENCODED MESSAGES WAITING TO BE CRACKED BY SKILLED CRYPTANALYSTS WITH SUFFICIENT TIME", null, "QFFT WX UJF DYVJWLFA HI UJF YHZDS VWTJFY HIIWVF UJFYF DYF UJHOADXQA HI FXVHQFQ CFAADBFA RDWUWXB UH KF VYDVEFQ KZ AEWSSFQ VYZTUDXDSZAUA RWUJ AOIIWVWFXU UWCF", "TextCipher", false, true, true, "{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9598, \"allPredictions\": [{\"family\": \"Substitution\", \"type\": \"SimpleSubstitution\", \"confidence\": 0.9598}, {\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 4.02e-06}]}", 250, null, null, 1, "Dust and Silence in the Royal Archive", 2 },
					{ 28, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THESECRETMESSAGEHASBEENHIDDENINSIDETHISPATTERNANDYOUWILLNEEDTOFIGUREOUTHOWMANYROWSOFTHEFENCEWERETOUSEDINORDERTODECIPHERIT", null, "TETSHEINIHARDWNTGOONWTEWTEOREHTHSCEMSAEABEHDEISDTIPTENNYUILEDOIUEUHWAYOSFHFNEEEOSDNRETDCPEIEREGSNDNESTAOLEFRTMROECRUIDOIR", "TextCipher", false, false, true, "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9234, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9234}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 7.66e-06}]}", 200, null, null, 1, "The Zigzagging Courier's Note", 7 },
					{ 29, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "CRYPTOGRAPHYISTHESCIENCEOFSECRETWRITINGANDITHASBEENUSEDTHROUGHOUTHISTORYTOPROTECTSENSITIVEINFORMATIONFROMFALLINGINTOTHEHANDSOFADVERSARIES", null, "CGICOEIIEDGITESIARLTAAARORYSSIEFRTTNDTBEETUHHSYOTCNIENMTFOLINOHNFDSRYTAHTEECSCWIGNHSNSHOOTTRPOTETVFRINMANITEDOVRISPPHNERAAURUORSIOOFGHSEE", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9187, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9187}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 8.13e-06}]}", 200, null, null, 1, "A Winding Path Through the Fortress", 7 },
					{ 30, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THEMASTERCRYPTOGRAPHERENCRYPTEDTHEMESSAGEUSINGAFENCEOFSIXRAILSANDONLYTHOSEWHOKNEWTHECORRECTPATTERNCOULDHOPETORECOVERTHEORIGINALTEXT", null, "TREDECLHWTUERTHCYHRETGUNEISTOETCPOLRCOIXERPPETHASEOAAYSNHEACDOOEGEMETANPESIFFRNLEKERTNHTVHITATORCYMSNASXDNWOCRTROEETNLSGREGIOHOEPRA", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9156, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9156}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 8.44e-06}]}", 200, null, null, 1, "The Six-Rail Garrison Communique", 7 },
					{ 31, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "WHENTHEGENERALSSENTTHEIRORDERSTOTHEFRONTLINEOFFICERSTHEYUSEDATRANSPOSITIONCIPHERTOSCRAMBLETHELETTERSANDPREVENTTHEENEMYFROMREADINGTHEPLANS", null, "WETEEEASETHIODRTTERNLNOFCRTEUEARNPSTOCPETSRMLTEETRADRVNTENMFORAIGHPASHNHGNRLSNTERRESOHFOTIEFIESHYSDTASOIINIHROCABEHLTESNPEETHEEYRMEDNTELN", "TextCipher", false, false, true, "{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9301, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 0.9301}, {\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 6.99e-06}]}", 200, null, null, 1, "Two Rows Across the Battlefield", 7 },
					{ 32, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE TRITHEMIUS CIPHER WAS INVENTED BY THE GERMAN ABBOT JOHANNES TRITHEMIUS IN THE FIFTEENTH CENTURY AND WAS ONE OF THE EARLIEST POLYALPHABETIC CIPHERS", null, "TIG WVNZOMVSFE PWEXVJ PUN EKTDNUGG FD ZOM POCYNB PRSGM DJDXLMET VUMYNLURED UA HWU WAYNZAKRG CFPWYWE HVM GLE BBT EW LAY ZWOJHETV SSQEHTYRLNRHXS TAIBZNP", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9478, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9478}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.22e-06}]}", 275, null, null, 1, "The Abbot's Progressive Manuscript", 6 },
					{ 33, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "UNLIKE THE CAESAR CIPHER WHICH USES A FIXED SHIFT THE TRITHEMIUS CIPHER PROGRESSIVELY INCREASES THE SHIFT BY ONE FOR EACH SUCCESSIVE LETTER IN THE MESSAGE", null, "UONLOJ ZOM LKPENF RYGZXL RDFAG UTGV E KOEMM CSUSH IXV LKCODBKHUT ELTMKY XAYRDRGHYMWES DJZPDATGV XMK ZPRPE NL CCU WGK YVYE QTCDGVWNBL TNDEQE WC JYW FYNOXED", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9412, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9412}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.88e-06}]}", 275, null, null, 1, "A Shifting Veil Over the Words", 6 },
					{ 34, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE ENCODED FIELD REPORT DESCRIBED IN CAREFUL DETAIL THE LOCATION OF THE ENEMY SUPPLY DEPOT AND THE ESTIMATED NUMBER OF SOLDIERS GUARDING THE ENTIRE FACILITY", null, "TIG HRHUKMM PTQYR GUGGKN YAPAQICGG MS IHZNPFX QSIQZD MBZ HLAZTJQQ SK ZOM NXPYL GJFGDR XZLLR ZNE VKI JYAQVKEQQ BJCSWK IA OLJCIFTV KZGYLRXR FUS TDKAKY AWZGKIUA", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9445, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Trithemius\", \"confidence\": 0.9445}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 5.55e-06}]}", 275, null, null, 1, "The Field Report from the Outer Wall", 6 },
					{ 35, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE VIGENERE CIPHER USES A KEYWORD TO SHIFT EACH LETTER BY A DIFFERENT AMOUNT MAKING SIMPLE FREQUENCY ANALYSIS INSUFFICIENT TO CRACK THE ENCODED MESSAGE", null, "LLG MMZWRGII VATJVV NKIU R OXQAQIH MG WJZJM WEEY PXLXGI FR S HKWJXJIPK EFGYPK QTCMPX WBETNV JKWUWVRVQ EPRPRKMU ZRLMJHZGBWRV KS VJEEB XAW IPTSWWH OVWLSKG", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9367, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9367}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.33e-06}]}", 400, null, null, 1, "The Keyword That Shifts All Things", 4 },
					{ 36, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "BLAISE DE VIGENERE PUBLISHED HIS FAMOUS CIPHER IN THE SIXTEENTH CENTURY AND FOR THREE HUNDRED YEARS IT WAS CONSIDERED COMPLETELY UNBREAKABLE BY CRYPTANALYSTS", null, "DTPPWV FM KPKVPMGL TLDTXZLVF PXZ JROWJZ GZRPTY ME VPT ZMOVMTUXY EMCAYIA ICK JFT BWYIV JCCKVVF GTHVJ KB LHW TQVHPHVTMS JSDRTTAICA CCIVVCSPIPV DG RYCGVICHPPUBH", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9312, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9312}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.88e-06}]}", 400, null, null, 1, "Three Centuries of Silence", 4 },
					{ 37, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE KASISKI EXAMINATION METHOD CAN BE USED TO DETERMINE THE KEY LENGTH OF A VIGENERE ENCODED MESSAGE BY FINDING REPEATED SEQUENCES AND MEASURING THE DISTANCES", null, "VYC ZTGKJIX XLCDGCTHKFL BXHJFB RTB DV SHXR VF BTMSTDGCX HJV ITR ZGEEIA CH R TXZSPVPT XBEFBTW AGJQPZS DP DXGRKEE GXDGRRTW GGHSTGQGJ YCW AGRQJKWPX RWX RKJRPGQGJ", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9389, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9389}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.11e-06}]}", 400, null, null, 1, "The Distance Between Repeated Shadows", 4 },
					{ 38, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "DURING THE AMERICAN CIVIL WAR BOTH THE UNION AND CONFEDERATE ARMIES USED THE VIGENERE CIPHER TO SEND SENSITIVE MILITARY COMMUNICATIONS BETWEEN FIELD COMMANDERS", null, "XHZWAA GPS NGRZWPUA KWICY EOE VBBV GBR CBVIA IBQ WBVTRXRZOGY NZAVYF CGRX GPS ICTMBRLR KWCBRZ HB MRVR FYAAWGCIM AVFVBOES PWAZOAQQNNVWBF VRBKRYA NWRFQ KCZGNVRRLF", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9298, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9298}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.02e-06}]}", 400, null, null, 1, "Dispatches Between the Divided Armies", 4 },
					{ 39, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE INDEX OF COINCIDENCE IS A POWERFUL STATISTICAL TOOL USED BY CRYPTANALYSTS TO DETECT POLYALPHABETIC ENCRYPTION AND ESTIMATE THE LENGTH OF THE KEYWORD USED", null, "WLP BNGII HF FSTGCLHPGCH MD T PRAPKFXP DMAWMDMIFEW MORP FLEG FJ VRBTETNDPJLTV XZ WEWINM PRPJTLSLLUEWMN XNFVJITLSY TNG IDMIPEEX TKI WXNJXS HF WLP DEBAZKD XWPW", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9334, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Vigenere\", \"confidence\": 0.9334}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 6.66e-06}]}", 400, null, null, 1, "The Index of Falling Letters", 4 },
					{ 40, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THESUPPLYCONVOYISSCHEDULEDTODEPARTFROMTHEBASEATARRIVALOFTHENORTHERNGROUPANDWILLTAKETHEEASTERNROUTETHROUGHTHEPASSTOAVOIDENEMYPATROLS", null, "TPVCEPOAROONALHETUPANTPNSLERBALNRPLTTUOEOEA#SCIDOTHAVHHOWKARHTSIYLHLOHDAMSRFRGNTEREGAVERUOSUDFETAEEUIESORHTDPSEYYETRTEITTRDAENTHSOMO", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9134, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9134}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.66e-06}]}", 350, null, null, 1, "The Rearranged Supply Route", 8 },
					{ 41, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THEOPERATIVEWILLMAKECONTACTATTHEPRESCRIBEDLOCATIONWHENANALLCLEARSIGNALISGIVENANDWILLTHENDELIVERTHEINTELLIGENCEREPORTTOTHESTATIONCHIEF", null, "PILEATEBCNACSLVDTERNIERHTH#TEVLCCHSEAWNLIIEWHLTTGRTEIIHREMOTECDTHAEGSNIEIHEEETSOEEAWANAPRLIELANGALNVELNPOTNFOTIKTTRIOONLRAINLDEILCOTAC#", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9087, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9087}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 9.13e-06}]}", 350, null, null, 1, "Contact Protocol at the Prescribed Hour", 8 },
					{ 42, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "COLUMNARANSPOSITIONISATYPEOFTRANSPOSITIONCIPHERTHATREARRANGESLETTERSACCORDINGTOASPECIFICKEYWORDORDERINGOFTHECOLUMNSINTHEGRIDLAYOUT", null, "MNIIPROOHARETCIAIEDRFOSELTLROOTFSTITENESRTECODGEMTIOOAPIAONICRRALROGPIWRNHUNRYUASNYTPIPHAGTADOCKREOCNHDUCNSTSEASNETRSECNSFYOITLIGA", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9112, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9112}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.88e-06}]}", 350, null, null, 1, "The Scrambled Grid of Forgotten Names", 8 },
					{ 43, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THEANCIENTMETHODOFTRANSPOSITIONCIPHERSWORKBYREARRANGINGTHELETTERSOFTHEMESSAGEWITHOUTCHANGINGTHELETTERTHEMSELVESONLYCHANGINGTHEIRPOSITIONS", null, "ENOAIIWRNHEHAHATTMSHGPOATDNTPOEGEREGONHESOATONNMOSIHRAILSMEUGERENNHSSHEHRSCSYATTTSTHGTEECNRITITTONRBRGTFSICNEHVYIITCEFPOEKRNEOEWTILTLLGEI#", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9098, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Columnar\", \"confidence\": 0.9098}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 9.02e-06}]}", 350, null, null, 1, "Positions Without Letters, Letters Without Order", 8 },
					{ 44, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THEROUTECIPHERARRANGESTHETEXTINTOAGRIDANDTHENREADSITOUTINASPIRALPATTERNSTARTINGFROMTHETOPANDWORKINGITSWAYINWARD", null, "THEROUTECIPSOETAIOGR##########DIPNTINATHERARRANGETHUPTTNAWNIYAWSTAGTNRGHETEXTINTOLREIKROWDNFEAERIDANDTAAHTMORRSADSIRTSNPI", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9198, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9198}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.02e-06}]}", 375, null, null, 1, "The Spiral Path of the Cartographer", 9 },
					{ 45, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "SECRETMESSAGESWEREHIDDENINSIDEINNOCENTLOOKINGTEXTSBYARRANGINGTHELETTERSONAGRIDANDTHENREADINGTHEMOUTINASPECIFICORDER", null, "SECRETMESSADNNREIATC######REDROIDDTAGOEGESWEREHIDNIRLREUIFICEPSANIATNTCNINSIDEIKAEGROMEHTGNNEGEENTLOOYHANEHTDRIXTSBTNOSNG", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9134, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9134}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.66e-06}]}", 375, null, null, 1, "A Message Hidden in Plain Geometry", 9 },
					{ 46, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THECRYPTOGRAPHERPLACEDTHEPLAINTEXTINTOASQUAREGRIDANDREADTHECHARACTERSOUTUSINGASPIRALROUTETHATMOVEDCLOCKWISEINWARD", null, "THECRYPTOGRDXRATGTCW########DRALEAEDETTAPHERPLACEEAECNUDNIESIWKCOTSRTGIHEPLAINTURAIOEVOMTAHPSHRNTOASQDRSRLARIOEIDANAUTUCH", "TextCipher", false, true, true, "{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9167, \"allPredictions\": [{\"family\": \"Transposition\", \"type\": \"Route\", \"confidence\": 0.9167}, {\"family\": \"Transposition\", \"type\": \"RailFence\", \"confidence\": 8.33e-06}]}", 375, null, null, 1, "The Clockwise Descent of the Cryptographer", 9 },
					{ 47, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE AUTOKEY CIPHER IMPROVES UPON THE VIGENERE BY USING THE PLAINTEXT ITSELF AS PART OF THE KEY AFTER THE INITIAL KEYWORD MAKING IT SIGNIFICANTLY HARDER TO CRACK", null, "DLC TBXOEXM MMNJMG PQGZAKVG PTGH IVR OPKZVKVR FP YTGHY BUK ISEXYTMKM MQLMEX ED UAJI OW MVJ DLC KJREW MLV BUMBVIE SEJGSPZ ARNUNQ QG YQZFQLVKFVVLL ALPKEI WS TKOEB", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9234, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9234}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.66e-06}]}", 500, null, null, 1, "The Message That Devours Its Own Key", 5 },
					{ 48, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE INTELLIGENCE OFFICER CAREFULLY ENCODED THE SENSITIVE DISPATCH USING THE AUTOKEY METHOD WHICH INCORPORATED THE MESSAGE TEXT DIRECTLY INTO THE ENCRYPTION KEYSTREAM", null, "TST PNMLPTVZIYNM UJSKGSW HITIWWLCC JHNZBIQ VVH WHGZMLMIW LBAKEWKZ JSBPN NZM NAMVOES FSDLMP AAPQK EUKQYXBTOKTR KHX QHLZESI LWXZ HBVBVWTP MPMZ RPR XBVYCTGKFL ZXGGGBIYE", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9178, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9178}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 8.22e-06}]}", 500, null, null, 1, "The Intelligence Officer's Running Cipher", 5 },
					{ 49, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "CRYPTANALYSTS STRUGGLED FOR YEARS TO DEVELOP RELIABLE METHODS FOR BREAKING AUTOKEY CIPHERS BECAUSE THE RUNNING KEY ELIMINATES THE REPEATING PATTERNS FOUND IN VIGENERE", null, "QDCVTCEYARSGS DRJNYYEVX LUC CHFFJ RS DVNXZRT MIWWPSPP UEUSSPW YVF EJJOBJEK AEBBQES VWZLCTA QLGRMTI VHY JYGUMEA XRG RRSQGRLBQA GHX VWILEKMCK PTBGKGNL YSLAV NB PVJMAZZK", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9212, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9212}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 7.88e-06}]}", 500, null, null, 1, "No Repeating Pattern, No Easy Answer", 5 },
					{ 50, true, true, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "a1b2c3d4-e5f6-7890-abcd-ef1234567890", "THE DIPLOMAT ENCODED HIS REPORT USING THE PREARRANGED KEYWORD AND THE AUTOKEY METHOD ENSURING THAT EVEN IF THE KEYWORD WERE DISCOVERED THE MESSAGE WOULD REMAIN SAFE", null, "ULX DBWPRUPE SZCHHRF VLW ULXGIX JGZGA LPR VKLEGIENXVD XKCZYVB WBU WHR DNASKYR AOXFAH XUGXVVFA KPNZ XCEG MA XUM PXFAYVB SSIH ZMJGRDWTSY XYI PXZWMKW OOAPZ FYXDZR EANR", "TextCipher", false, true, true, "{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9189, \"allPredictions\": [{\"family\": \"Polyalphabetic\", \"type\": \"Autokey\", \"confidence\": 0.9189}, {\"family\": \"Substitution\", \"type\": \"Caesar\", \"confidence\": 8.11e-06}]}", 500, null, null, 1, "The Diplomat's Self-Consuming Letter", 5 }
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 16);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 17);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 18);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 19);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 20);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 21);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 22);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 23);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 24);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 25);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 26);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 27);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 28);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 29);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 30);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 31);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 32);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 33);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 34);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 35);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 36);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 37);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 38);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 39);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 40);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 41);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 42);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 43);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 44);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 45);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 46);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 47);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 48);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 49);

			migrationBuilder.DeleteData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 50);

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "UserName" },
				values: new object[] { "4ea6d7e9-c8ac-4f0a-83f6-e6db8cc7a966", "AQAAAAIAAYagAAAAELcqm2EYSfKDaZYv8we17H+KNVv7lLC+hyEocoEXX10SaQOgBC4t6cJszV5vYphjmA==", "admin@cryptomind.com" });

			migrationBuilder.UpdateData(
				table: "AspNetUsers",
				keyColumn: "Id",
				keyValue: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
				columns: new[] { "ConcurrencyStamp", "PasswordHash", "UserName" },
				values: new object[] { "905fcd84-1eba-41af-9dc7-82370ab4b930", "AQAAAAIAAYagAAAAEPwco7b5ewsjup09QvVcRjX8j7TAqeMxR/plMrwVWA2I2VTRX2H62ZQ1kh1dj1jaxw==", "user@cryptomind.com" });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 1,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "ROT13", null, true, "Cryptography is the practice of securing information by transforming it into an unreadable format. Only those with the correct key can decode the message and read its original contents.", "Pbclgbtencul vf gur cenpgvpr bs frphevat vasbezngvba ol genafsbezvat vg vagb na haernqnoyr sbezng. Bayl gubfr jvgu gur pbeerpg xrl pna qrpbqr gur zrffntr naq ernq vgf bevtvany pbagragf.", "{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.98},{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.04}]}", 10, "The ROT13 Challenge", 3 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 2,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true, "The art of war is of vital importance to the state. It is a matter of life and death, a road either to safety or to ruin. Hence it is a subject of inquiry which can on no account be neglected.", "Wkh duw ri zdu lv ri ylwdo lpsruwdqfh wr wkh vwdwh. Lw lv d pdwwhu ri olih dqg ghdwk, d urdg hlwkhu wr vdihwb ru wr uxlq. Khqfh lw lv d vxemhfw ri lqtxlub zklfk fdq rq qr dffrxqw eh qhjohfwhg.", "{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.97,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.97},{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.06}]}", 10, "Caesar's Secret", 0 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 3,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Atbash", null, true, "The system of cryptography depends on the strongness of mathematical problems that are believed to be computationally intractable. The secrets of modern encryption rest on this foundation.", "Gsv hbhgvn lu xibkgltizksb wvkvmwh lm gsv hgizmtgsvm lu nzgsvnzgrixzo kiliyovnh gszg ziv yvorvevw gl yv xlnkfgzgrlmzoob rmgizxgzyov. Gsv hvxfirgh lu nlwvim vmxibkgrlm ivhg lm gsrh ulfmwzgrlm.", "{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.99,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.99},{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.03}]}", 15, "Mirror of Letters", 1 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 4,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "SimpleSubstitution", null, true, "Following old habits can keep you at an undesired place. Remove insight from the role to reach and reveal one's keys. Understanding the subtleness of the representations is the deep insight.", "Xbyybdwhn oxo thyepc iye xc kj exoyuncxwnh cwzbo. Kbbenh xwcejke tybw mbn jyzb ze dbyobex bde'c tbnc. Ynobycezwowhn exn cbljybcco bq zbn ynjybcbnezexobwc oc exn monp xwcocnex.", "{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.94,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.94},{\"Family\":\"Substitution\",\"Type\":\"Atbash\",\"Confidence\":0.07}]}", 25, "Scrambled Alphabet", 2 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 5,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Vigenere", null, true, "Cryptology is the art of hiding information beyond its statistical qualities. The key is a word used to encode and decode the message based on a road map.", "Lxfopvefrnhr xh qsi zyg yv aimgmrk mrhsvqexmsr fciymrk mxw wxexmwxmgep uyepmxmiw. Xli cli mw e asv ywih xs irgsHi erh higsHi xli qiwweki fewiH sr e vseH qex.", "{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.96,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.96},{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.05}]}", 30, "The Vigenere Veil", 4 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 6,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Autokey", null, true, "Autokey is a sophistication where the key changes automatically based on the plaintext itself. This renders it significantly more secure than standard polyalphabetic ciphers.", "Bpsxozgfmz al i kmzbsyblqkibqwv aowabsz cvsfs bvs zsg qvivusa iybwzibqkittr pibsr wv bvs xtiqvbslb qbastd. Bvqa zmvrsza qb awuvoqkivbtr uwfs lmkgem bviv abivrivp xwtritxvibsmqk kqxvsfl.", "{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.93,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.93},{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.08}]}", 35, "Autokey Enigma", 5 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 7,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Trithemius", null, true, "Trithemius is a polyalphabetic cipher, where the alphabet is shifted in a regular manner without any key. The result is a regular moving state of the alphabet shifted in order.", "Alcru lr t uidlbxevlypyu mltrag, qsxpxr max tufetmlx lv yaxnxw lq t pxznetm ftqqxo ulmaxry tgr uxr. Qeb exjbiv fp t pxznetm nlqfqz pftcx bq max tufetmax ualmary mr fqvbq.", "{\"Family\":\"Polyalphabetic\",\"Type\":\"Trithemius\",\"Confidence\":0.95,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Trithemius\",\"Confidence\":0.95},{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.06}]}", 30, "The Trithemius Ladder", 6 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 8,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "RailFence", null, true, "Transposition of information can be described decoded by discovering the statistical structure of the text. The letters are rearranged in a zigzag pattern across multiple rails.", "Tersoi snomto aefnaifrtnoi cne eb dseeicrphd sdneoeircd yb giieosnvbr het attniclstiias rrutscuet fo het ttxe. Het eltters rea rea rneadgrar ni a zagigzag ttanrep scroass plutefilm sialr.", "{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.92,\"AllPredictions\":[{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.92},{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.09}]}", 20, "The Rail Fence", 7 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 9,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "Medium", true, true, null, "Columnar", null, true, "Information is arranged in rows and columns, then read off by column in a specific order. The key determines the order in which the columns are read to recover the plaintext.", "Iitmnofra si darre ni wors nda ulnocsm, hnet arde fof yb ncolum ni a pecifsi rerod. Teh yke medinretes eth rored ni hcwhi hte nlomcsu rae arde ot decor teh tpxtliean.", "{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.91,\"AllPredictions\":[{\"Family\":\"Transposition\",\"Type\":\"Columnar\",\"Confidence\":0.91},{\"Family\":\"Transposition\",\"Type\":\"RailFence\",\"Confidence\":0.11}]}", 25, "The Columnar Maze", 8 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 10,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Base64", null, true, "Base64 is not an encryption mechanism but an encoding format that represents binary data in an ASCII string format using sixty-four characters.", "QmFzZTY0IGlzIG5vdCBhbiBlbmNyeXB0aW9uIG1lY2hhbmlzbSBidXQgYW4gZW5jb2RpbmcgZm9ybWF0IHRoYXQgcmVwcmVzZW50cyBiaW5hcnkgZGF0YSBpbiBhbiBBU0NJSSBzdHJpbmcgZm9ybWF0IHVzaW5nIHNpeHR5LWZvdXIgY2hhcmFjdGVycy4=", "{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.99,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.99},{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.02}]}", 10, "The Base64 Barrier", 10 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 11,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title" },
				values: new object[] { "", "", "", "High", true, true, null, "Morse", null, true, "MORSE CODE IS NOT A CIPHER BUT AN ENCODING SYSTEM THAT REPRESENTS LETTERS AS SEQUENCES OF DOTS AND DASHES", "-- --- .-. ... . / -.-. --- -.. . / .. ... / -. --- - / .- / -.-. .. .--. .... . .-. / -... ..- - / .- -. / . -. -.-. --- -.. .. -. --. / ... -.-- ... - . -- / - .... .- - / .-. . .--. .-. . ... . -. - ... / .-.. . - - . .-. ... / .- ... / ... . --.- ..- . -. -.-. . ... / --- ..-. / -.. --- - ... / .- -. -.. / -.. .- ... .... . .", "{\"Family\":\"Encoding\",\"Type\":\"Morse\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Morse\",\"Confidence\":0.98},{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.03}]}", 10, "Dots and Dashes" });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 12,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Binary", null, true, "Binary uses zeros and ones", "01000010 01101001 01101110 01100001 01110010 01111001 00100000 01110101 01110011 01100101 01110011 00100000 01111010 01100101 01110010 01101111 01110011 00100000 01100001 01101110 01100100 00100000 01101111 01101110 01100101 01110011", "{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.97,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Binary\",\"Confidence\":0.97},{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.04}]}", 10, "The Binary Message", 12 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 13,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Hex", null, true, "Hex encoding represents data using sixteen distinct symbols from the hexadecimal number system, using digits zero through nine and letters a through f for values ten to fifteen", "48657820656e636f64696e6720726570726573656e74732064617461207573696e672073697874656574206469737469 6e637420737 96d626f6c73 2066726f6d 207468652068657861646563696d616c206e756d626572207379737465 6d2c207573696e67206469676974732030207468726f75676820392061 6e64206c657474657273206120 7468726f756768206620 666f722076616c7565732074656e20746f2066696674656 56e", "{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.98,\"AllPredictions\":[{\"Family\":\"Encoding\",\"Type\":\"Hex\",\"Confidence\":0.98},{\"Family\":\"Encoding\",\"Type\":\"Base64\",\"Confidence\":0.03}]}", 10, "Hex Decoded", 13 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 14,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "AllowSolution", "AllowTypeHint", "DecryptedText", "EncryptedText", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "High", true, true, null, "Caesar", null, true, false, false, "Perhaps exists a difference between information and intelligence. One is a more lasting on passing details round, the other is a more solidling by wearing from a wider picku. Only those who can ciphers rise based on fals.", "Mnjpncn rflbcb l asfmncnwln unwonnw jwxfanzlcrxw lwm jwcnuurpnwln. Cwn rb l exfn qlacrwp xw ylabrwp mncluub fxdwm, cwn xcsnf rb l sxfn bxurmurwp eq dwnlfrwp qfxz l efrmnf yrnad. Xwuq csxbn tsx alw arqsncb frnb elbnm xw qlab.", "{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.96,\"AllPredictions\":[{\"Family\":\"Substitution\",\"Type\":\"Caesar\",\"Confidence\":0.96},{\"Family\":\"Substitution\",\"Type\":\"ROT13\",\"Confidence\":0.05}]}", 20, "Julius's Whisper", 0 });

			migrationBuilder.UpdateData(
				table: "Cipher",
				keyColumn: "Id",
				keyValue: 15,
				columns: new[] { "LLMData_CachedHint", "LLMData_CachedSolution", "LLMData_CachedTypeHint", "LLMData_Confidence", "LLMData_IsAppropriate", "LLMData_IsSolvable", "LLMData_Issues", "LLMData_PredictedType", "LLMData_Reasoning", "LLMData_SolutionCorrect", "AllowHint", "AllowSolution", "AllowTypeHint", "ChallengeType", "DecryptedText", "EncryptedText", "IsLLMRecommended", "IsPlaintextValid", "MLPrediction", "Points", "Title", "TypeOfCipher" },
				values: new object[] { "", "", "", "Medium", true, null, null, "Vigenere", null, null, false, false, false, 1, null, "Pbzr byhgvbaf ner abg ernqvyl xabja. Guvf zrffntr znl or rapelcgrq jvgu n pvcure gung unf ab pbasvezrq fbyhgvba. Lbhe gnfx vf gb fghql gur cnggrea, fhttrfg n cbffvoyr zrgubq, naq fhowzvg lbhe ernfbarq thrff sbe pbzzhavgl irevsvpngvba.", true, false, "{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.89,\"AllPredictions\":[{\"Family\":\"Polyalphabetic\",\"Type\":\"Vigenere\",\"Confidence\":0.89},{\"Family\":\"Polyalphabetic\",\"Type\":\"Autokey\",\"Confidence\":0.14},{\"Family\":\"Substitution\",\"Type\":\"SimpleSubstitution\",\"Confidence\":0.07}]}", 50, "The Unknown Veil", null });
		}
	}
}
