using Cryptomind.Common.Helpers;
using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Seeders
{
	public static class DailyChallengeSeeder
	{
		private static readonly List<string> Quotes = new()
		{
			// Cryptography & Security
			"The only way to keep a secret is to never have one.",
			"In cryptography, trust is the enemy of security.",
			"Secrecy is the beginning of tyranny.",
			"Security without privacy is a prison. Privacy without security is anarchy.",
			"He who holds the key holds the kingdom.",
			"Every secret creates a potential enemy.",
			"The message is not the medium.",
			"If you want to keep a secret you must also hide it from yourself.",
			"Secrecy is the lifeblood of intelligence.",
			"The best code is the one that needs no breaking, because it was never written.",

			// Science & Mathematics
			"Mathematics is the language in which God wrote the universe.",
			"In mathematics you do not understand things. You just get used to them.",
			"Pure mathematics is, in its way, the poetry of logical ideas.",
			"Mathematics is not about numbers, equations, computations, or algorithms. It is about understanding.",
			"Without mathematics, there is nothing you can do. Everything around you is mathematics.",
			"Mathematics is the music of reason.",
			"The laws of nature are but the mathematical thoughts of God.",
			"The essence of mathematics lies in its freedom.",
			"Numbers are the highest degree of knowledge. It is knowledge itself.",
			"There is no royal road to geometry.",

			// Logic & Reason
			"The man who asks a question is a fool for a minute, the man who does not ask is a fool for life.",
			"Logic will get you from A to Z; imagination will get you everywhere.",
			"The function of education is to teach one to think intensively and to think critically.",
			"The greatest enemy of knowledge is not ignorance, it is the illusion of knowledge.",
			"It is not the answer that enlightens, but the question.",
			"The measure of intelligence is the ability to change.",
			"The more I learn, the more I realize how much I do not know.",
			"Real knowledge is to know the extent of one's ignorance.",
			"The only true wisdom is in knowing you know nothing.",
			"An unexamined life is not worth living.",
			"Wonder is the beginning of wisdom.",

			// History & Philosophy
			"Those who cannot remember the past are condemned to repeat it.",
			"History is written by the victors.",
			"In war, truth is the first casualty.",
			"The art of war is of vital importance to the state.",
			"Know your enemy and know yourself and you can fight a hundred battles without disaster.",
			"All warfare is based on deception.",
			"In the middle of every difficulty lies opportunity.",
			"Success is not final, failure is not fatal. It is the courage to continue that counts.",
			"The price of greatness is responsibility.",
			"We are what we repeatedly do. Excellence, then, is not an act, but a habit.",
			"He who knows others is wise. He who knows himself is enlightened.",

			// Technology & Innovation
			"Any sufficiently advanced technology is indistinguishable from magic.",
			"The computer was born to solve problems that did not exist before the computer.",
			"Technology is a useful servant but a dangerous master.",
			"It has become appallingly obvious that our technology has exceeded our humanity.",
			"The real problem is not whether machines think but whether men do.",
			"The best way to predict the future is to invent it.",
			"Innovation distinguishes between a leader and a follower.",
			"The only way to discover the limits of the possible is to go beyond them into the impossible.",

			// Alan Turing & Computing
			"We can only see a short distance ahead, but we can see plenty there that needs to be done.",
			"Sometimes it is the people no one can imagine anything of who do the things no one can imagine.",
			"Science is what we understand well enough to explain to a computer. Art is everything else we do.",
			"The greatest scientists are artists as well.",
			"Anyone who has never made a mistake has never tried anything new.",
			"Imagination is more important than knowledge.",

			// Wisdom & Life
			"Two roads diverged in a wood, and I took the one less traveled by, and that has made all the difference.",
			"The truth will set you free, but first it will make you miserable.",
			"Whether you think you can or you think you cannot, you are right.",
			"Life is what happens to us while we are making other plans.",
			"The only impossible journey is the one you never begin.",
			"It does not matter how slowly you go as long as you do not stop.",
			"Everything you have ever wanted is on the other side of fear.",
			"Do not watch the clock. Do what it does. Keep going.",
			"A journey of a thousand miles begins with a single step.",
			"Fall seven times, stand up eight.",
			"The secret of getting ahead is getting started.",
			"The mind is everything. What you think you become.",

			// Great Thinkers
			"We are all made of star stuff.",
			"The cosmos is within us. We are made of star stuff. We are a way for the universe to know itself.",
			"The most beautiful thing we can experience is the mysterious.",
			"Two things are infinite: the universe and human stupidity; and I am not sure about the universe.",
			"Try not to become a man of success, but rather try to become a man of value.",
			"Life is like riding a bicycle. To keep your balance, you must keep moving.",
			"If you cannot explain it simply, you do not understand it well enough.",
			"Not everything that can be counted counts, and not everything that counts can be counted.",
			"Education is not the learning of facts, but the training of the mind to think.",

			// Hidden Messages & Information
			"The message is only as strong as the cipher that protects it.",
			"Every letter placed in sequence tells a story waiting to be revealed.",
			"Between the lines of every text lies the true meaning of what was intended.",
			"What is written without effort is in general read without pleasure.",
			"The pen is mightier than the sword, but the cipher is mightier than both.",
			"Words are, of course, the most powerful drug used by mankind.",
			"The limits of my language mean the limits of my world.",
			"All the information in the universe is contained in a single message.",

			// Extra quotes for variety
			"Not all those who wander are lost.",
			"The cave you fear to enter holds the treasure you seek.",
			"Do not go where the path may lead, go instead where there is no path and leave a trail.",
			"You cannot cross the sea merely by standing and staring at the water.",
			"The secret of change is to focus all of your energy not on fighting the old, but on building the new.",
			"Life shrinks or expands in proportion to one's courage.",
			"To live is the rarest thing in the world. Most people just exist.",
			"In the depth of winter, I finally learned that there was in me an invincible summer.",
			"It always seems impossible until it is done.",
			"What lies behind us and what lies before us are tiny matters compared to what lies within us.",
			"The only way out is through.",
			"Everything you can imagine is real.",
			"The world is a book, and those who do not travel read only one page.",
			"You have power over your mind, not outside events. Realize this, and you will find strength.",
			"The impediment to action advances action. What stands in the way becomes the way.",
			"He who conquers himself is the mightiest warrior.",
			"Difficulties strengthen the mind, as labor does the body.",
			"Begin at once to live, and count each separate day as a separate life.",
			"Luck is what happens when preparation meets opportunity.",
			"It is not death that a man should fear, but he should fear never beginning to live.",
			"Perfection is achieved not when there is nothing more to add, but when there is nothing left to take away.",
			"The danger of the past was that men became slaves. The danger of the future is that men may become robots.",
			"We know what we are, but know not what we may be.",
			"All our dreams can come true if we have the courage to pursue them.",
			"The future belongs to those who believe in the beauty of their dreams.",
			"Act as if what you do makes a difference. It does.",
			"Keep your face always toward the sunshine, and shadows will fall behind you.",
			"The only person you are destined to become is the person you decide to be.",
		};

		private static readonly List<(string Name, CipherType Type)> CipherTypes = new()
		{
			("Caesar", CipherType.Caesar),
			("Atbash", CipherType.Atbash),
			("SimpleSubstitution", CipherType.SimpleSubstitution),
			("ROT13", CipherType.ROT13),
			("Vigenere", CipherType.Vigenere),
			("Autokey", CipherType.Autokey),
			("Trithemius", CipherType.Trithemius),
			("RailFence", CipherType.RailFence),
			("Columnar", CipherType.Columnar),
			("Route", CipherType.Route),
		};

		public static async Task SeedAsync(CryptomindDbContext context)
		{
			if (await context.DailyChallengeEntries.AnyAsync())
				return;

			var entries = new List<DailyChallengeEntry>();

			for (int i = 0; i < Quotes.Count; i++)
			{
				var (cipherName, cipherType) = CipherTypes[i % CipherTypes.Count];
				string plainText = Quotes[i];

				string encryptedText = cipherName switch
				{
					"Caesar" => CipherGeneratorHelper.Caesar(plainText),
					"Atbash" => CipherGeneratorHelper.Atbash(plainText),
					"SimpleSubstitution" => CipherGeneratorHelper.SimpleSubstitution(plainText),
					"ROT13" => CipherGeneratorHelper.Rot13(plainText),
					"Vigenere" => CipherGeneratorHelper.Vigenere(plainText),
					"Autokey" => CipherGeneratorHelper.Autokey(plainText),
					"Trithemius" => CipherGeneratorHelper.Trithemius(plainText),
					"RailFence" => CipherGeneratorHelper.RailFence(plainText),
					"Columnar" => CipherGeneratorHelper.ColumnarTransposition(plainText),
					"Route" => CipherGeneratorHelper.RouteCipher(plainText),
					_ => throw new ArgumentException($"Unknown cipher: {cipherName}")
				};

				entries.Add(new DailyChallengeEntry
				{
					PlainText = plainText,
					EncryptedText = encryptedText,
					CipherType = cipherType,
					IsUsed = false,
					AssignedDate = null,
				});
			}

			await context.DailyChallengeEntries.AddRangeAsync(entries);
			await context.SaveChangesAsync();
		}
	}
}
