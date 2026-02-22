import random
from src.cipher_generator import CipherGenerator

# Load corpus
print("\nLoading corpus...", end=" ", flush=True)
with open('data/corpus/corpus.txt', 'r', encoding='utf-8') as f:
    corpus = f.read()

# Extract sentences (200-400 chars)
sentences = [s.strip() for s in corpus.split('.') if 200 <= len(s.strip()) <= 400]
print(f"✅ ({len(sentences)} sentences available)")

# Pick a random sentence
plaintext = random.choice(sentences)

print("PLAINTEXT (Selected randomly from corpus)")
print(f"\nLength: {len(plaintext)} characters")
print(f"\n{plaintext}\n")

# Initialize generator
cipher_gen = CipherGenerator()

examples = []

# Caesar
caesar = cipher_gen.caesar(plaintext, shift=7)
examples.append(("Caesar", caesar))

# ROT13
rot13 = cipher_gen.rot13(plaintext)
examples.append(("ROT13", rot13))

# Atbash
atbash = cipher_gen.atbash(plaintext)
examples.append(("Atbash", atbash))

# SimpleSubstitution
simple = cipher_gen.simple_substitution(plaintext)
examples.append(("SimpleSubstitution", simple))

# Vigenere
vigenere = cipher_gen.vigenere(plaintext, key="CRYPTOGRAPHY")
examples.append(("Vigenere", vigenere))

# Autokey
autokey = cipher_gen.autokey(plaintext, key="SECRET")
examples.append(("Autokey", autokey))

# Trithemius
trithemius = cipher_gen.trithemius(plaintext)
examples.append(("Trithemius", trithemius))

# RailFence
railfence = cipher_gen.rail_fence(plaintext, rails=4)
examples.append(("RailFence", railfence))

# Columnar
columnar = cipher_gen.columnar_transposition(plaintext, key="CIPHER")
examples.append(("Columnar", columnar))

# Route
route = cipher_gen.route_cipher(plaintext)
examples.append(("Route", route))

# Base64
short_text = plaintext[:100]  # Shorter for encoding examples
base64 = cipher_gen.base64_encode(short_text)
examples.append(("Base64", base64))

# Morse
morse_text = plaintext[:50]  # Even shorter for Morse
morse = cipher_gen.morse_code(morse_text)
examples.append(("Morse", morse))

# Binary
binary_text = plaintext[:30]
binary = cipher_gen.binary_encode(binary_text)
examples.append(("Binary", binary))

# Hex
hex_text = plaintext[:100]
hex_enc = cipher_gen.hex_encode(hex_text)
examples.append(("Hex", hex_enc))

# Plaintext
examples.append(("Plaintext", plaintext))

# Save to file
print("Saving examples to file...")

with open('testing/random_examples.txt', 'w', encoding='utf-8') as f:
    f.write("RANDOM CIPHER EXAMPLES\n")
    f.write("PLAINTEXT:\n")
    f.write(plaintext + "\n\n")
    
    for cipher_type, ciphertext in examples:
        f.write(f"{cipher_type.upper()}\n")
        f.write("-"*80 + "\n")
        f.write(ciphertext + "\n\n")

print("Examples saved to testing/random_examples.txt")

# Interactive testing option
print("QUICK TEST")
print("\nWould you like to test these with the predictor? (y/n): ", end="", flush=True)

try:
    response = input().strip().lower()
    
    if response == 'y':
        print("\n🔧 Loading predictor...")
        from src.predictor import CipherPredictor
        predictor = CipherPredictor()
        print("Loaded\n")
        
        print("TESTING PREDICTIONS")
        
        correct = 0
        total = 0
        
        for cipher_type, ciphertext in examples:
            try:
                result = predictor.predict(ciphertext)
                predicted = result['top_prediction']['type']
                confidence = result['top_prediction']['confidence'] * 100
                
                total += 1
                if predicted == cipher_type:
                    correct += 1
                    status = "✅"
                else:
                    status = "❌"
                
                print(f"{status} {cipher_type:20s} → {predicted:20s} ({confidence:.1f}%)")
                
            except Exception as e:
                print(f"⚠️  {cipher_type:20s} → Error: {e}")
        
        print(f"Results: {correct}/{total} correct ({correct/total*100:.1f}%)")
        
except KeyboardInterrupt:
    print("\n\nSkipped testing.")

print("\nDone!")