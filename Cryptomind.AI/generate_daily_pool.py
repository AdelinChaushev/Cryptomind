"""
Daily Challenge Pool Generator
Generates ~100 cipher-encrypted famous quotes for the Cryptomind daily challenge feature.
Run this script once from the Cryptomind.AI/ directory:
    python generate_daily_pool.py

Output: ../Cryptomind.Data/Seeds/daily_pool.json
"""

import sys
import os
import json
import random

sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'src'))

from cipher_generator import CipherGenerator

QUOTES = [
    # Cryptography & Security
    "The only way to keep a secret is to never have one.",
    "In cryptography, trust is the enemy of security.",
    "A code is a puzzle. A riddle. And a cipher is just another way to hide the truth.",
    "Secrecy is the beginning of tyranny.",
    "The strength of a system lies not in the key, but in the minds that cannot break it.",
    "Encryption is the act of turning information into unintelligible noise to all but the intended.",
    "Every secret creates a potential enemy.",
    "The message is not the medium.",
    "If you want to keep a secret you must also hide it from yourself.",
    "A man's got to know his limitations, and so does every cipher.",
    "The best code is the one that needs no breaking, because it was never written.",
    "Secrecy is the lifeblood of intelligence.",
    "Security without privacy is a prison. Privacy without security is anarchy.",
    "He who holds the key holds the kingdom.",
    "The greatest trick a cipher ever pulled was convincing the world it could not be solved.",

    # Science & Mathematics
    "Mathematics is the language in which God wrote the universe.",
    "In mathematics you do not understand things. You just get used to them.",
    "Pure mathematics is, in its way, the poetry of logical ideas.",
    "The miracle of the appropriateness of the language of mathematics for the formulation of the laws of physics is a wonderful gift.",
    "Mathematics is not about numbers, equations, computations, or algorithms. It is about understanding.",
    "Without mathematics, there is nothing you can do. Everything around you is mathematics.",
    "Mathematics is the music of reason.",
    "The laws of nature are but the mathematical thoughts of God.",
    "An equation has no meaning for me unless it expresses a thought of God.",
    "If people do not believe that mathematics is simple, it is only because they do not realize how complicated life is.",
    "Mathematics is the art of giving the same name to different things.",
    "The essence of mathematics lies in its freedom.",
    "To think is to practice brain chemistry.",
    "Numbers are the highest degree of knowledge. It is knowledge itself.",
    "There is no royal road to geometry.",

    # Logic & Reason
    "The man who asks a question is a fool for a minute, the man who does not ask is a fool for life.",
    "Logic will get you from A to Z; imagination will get you everywhere.",
    "The function of education is to teach one to think intensively and to think critically.",
    "The greatest enemy of knowledge is not ignorance, it is the illusion of knowledge.",
    "It is not the answer that enlightens, but the question.",
    "We shall not cease from exploration, and the end of all our exploring will be to arrive where we started.",
    "The measure of intelligence is the ability to change.",
    "The more I learn, the more I realize how much I do not know.",
    "Knowledge is knowing that a tomato is a fruit. Wisdom is not putting it in a fruit salad.",
    "Real knowledge is to know the extent of one's ignorance.",
    "The only true wisdom is in knowing you know nothing.",
    "An unexamined life is not worth living.",
    "Wonder is the beginning of wisdom.",
    "Science is not only a disciple of reason but also one of romance and passion.",
    "The first step toward change is awareness. The second step is acceptance.",

    # History & Philosophy
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
    "The superior man is satisfied and composed; the mean man is always full of distress.",
    "When you reach the end of your rope, tie a knot in it and hang on.",
    "No man is an island, entire of itself. Every man is a piece of the continent.",
    "Ask not what your country can do for you, ask what you can do for your country.",

    # Technology & Innovation
    "Any sufficiently advanced technology is indistinguishable from magic.",
    "The computer was born to solve problems that did not exist before the computer.",
    "It is not the strongest of the species that survives, nor the most intelligent. It is the one most responsive to change.",
    "The Internet is becoming the town square for the global village of tomorrow.",
    "Technology is a useful servant but a dangerous master.",
    "The advance of technology is based on making it fit in so that you do not really even notice it.",
    "We are stuck with technology when what we really want is just stuff that works.",
    "It has become appallingly obvious that our technology has exceeded our humanity.",
    "Science and technology revolutionize our lives, but memory, tradition and myth frame our response.",
    "The real problem is not whether machines think but whether men do.",
    "Computers are incredibly fast, accurate and stupid. Human beings are incredibly slow, inaccurate and brilliant.",
    "The question of whether a computer can think is no more interesting than the question of whether a submarine can swim.",
    "The best way to predict the future is to invent it.",
    "Innovation distinguishes between a leader and a follower.",
    "The only way to discover the limits of the possible is to go beyond them into the impossible.",

    # Alan Turing & Computing Pioneers
    "We can only see a short distance ahead, but we can see plenty there that needs to be done.",
    "A computer would deserve to be called intelligent if it could deceive a human into believing that it was human.",
    "Sometimes it is the people no one can imagine anything of who do the things no one can imagine.",
    "We shall not cease from exploration, and the end of all our exploring will be to arrive where we started and know the place for the first time.",
    "The original question is whether a computer can think. But that is a bit like asking whether a submarine can swim.",
    "Science is what we understand well enough to explain to a computer. Art is everything else we do.",
    "The greatest scientists are artists as well.",
    "Anyone who has never made a mistake has never tried anything new.",
    "The measure of intelligence is the ability to change.",
    "Imagination is more important than knowledge.",

    # Wisdom & Life
    "Two roads diverged in a wood, and I took the one less traveled by, and that has made all the difference.",
    "In the beginning was the Word, and the Word was with the message.",
    "The truth will set you free, but first it will make you miserable.",
    "You miss one hundred percent of the shots you do not take.",
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
    "Twenty years from now you will be more disappointed by the things you did not do than by the ones you did.",

    # Great Thinkers
    "We are all made of star stuff.",
    "Somewhere, something incredible is waiting to be known.",
    "The cosmos is within us. We are made of star stuff. We are a way for the universe to know itself.",
    "For small creatures such as we the vastness is bearable only through love.",
    "The universe is not required to be in perfect harmony with human ambition.",
    "The most beautiful thing we can experience is the mysterious.",
    "Imagination is the highest form of research.",
    "Two things are infinite: the universe and human stupidity; and I am not sure about the universe.",
    "When you are courting a nice girl an hour seems like a second. When you sit on a red-hot cinder a second seems like an hour.",
    "Try not to become a man of success, but rather try to become a man of value.",
    "Life is like riding a bicycle. To keep your balance, you must keep moving.",
    "If you cannot explain it simply, you do not understand it well enough.",
    "Insanity is doing the same thing over and over again and expecting different results.",
    "Not everything that can be counted counts, and not everything that counts can be counted.",
    "Education is not the learning of facts, but the training of the mind to think.",

    # Hidden Messages & Information
    "The message is only as strong as the cipher that protects it.",
    "Every letter placed in sequence tells a story waiting to be revealed.",
    "Between the lines of every text lies the true meaning of what was intended.",
    "What is written without effort is in general read without pleasure.",
    "The pen is mightier than the sword, but the cipher is mightier than both.",
    "Words are, of course, the most powerful drug used by mankind.",
    "The limits of my language mean the limits of my world.",
    "One must always be careful of books, and what is inside them, for words have the power to change us.",
    "All the information in the universe is contained in a single message.",
    "Wherever you go, there you are. The message follows.",
]

# CipherType enum values matching C# backend
CIPHER_TYPES = [
    ('Caesar', 0),
    ('Atbash', 1),
    ('SimpleSubstitution', 2),
    ('ROT13', 3),
    ('Vigenere', 4),
    ('Autokey', 5),
    ('Trithemius', 6),
    ('RailFence', 7),
    ('Columnar', 8),
    ('Route', 9),
    ('Base64', 10),
    ('Morse', 11),
    ('Binary', 12),
    ('Hex', 13),
]


def generate_pool(output_path):
    generator = CipherGenerator()
    pool = []
    random.seed(42)  # Reproducible output

    cipher_index = 0  # Round-robin to ensure all cipher types appear

    for quote in QUOTES:
        cipher_name, cipher_type_id = CIPHER_TYPES[cipher_index % len(CIPHER_TYPES)]
        cipher_index += 1

        try:
            encrypted = generator.generate(cipher_name, quote)
            pool.append({
                "plainText": quote,
                "encryptedText": encrypted,
                "cipherType": cipher_type_id,
                "cipherKey": None
            })
        except Exception as e:
            print(f"Warning: Failed to encrypt quote with {cipher_name}: {e}")

    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(pool, f, indent=2, ensure_ascii=False)

    print(f"Generated {len(pool)} daily challenge entries -> {output_path}")

    # Print summary by cipher type
    from collections import Counter
    counts = Counter(entry['cipherType'] for entry in pool)
    for cipher_name, type_id in CIPHER_TYPES:
        print(f"  {cipher_name} (type {type_id}): {counts.get(type_id, 0)} entries")


if __name__ == '__main__':
    script_dir = os.path.dirname(os.path.abspath(__file__))
    output = os.path.join(script_dir, '..', 'Cryptomind.Data', 'Seeds', 'daily_pool.json')
    output = os.path.normpath(output)
    generate_pool(output)
