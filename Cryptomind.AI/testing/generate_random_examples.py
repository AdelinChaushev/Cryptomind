"""
Generate random cipher examples for testing
Creates examples of all 14 cipher types from random corpus text
"""
import random
from src.cipher_generator import CipherGenerator

print("="*80)
print("RANDOM CIPHER EXAMPLES GENERATOR")
print("="*80)

# Load corpus
print("\nLoading corpus...", end=" ", flush=True)
with open('data/corpus/corpus.txt', 'r', encoding='utf-8') as f:
    corpus = f.read()

# Extract sentences (200-400 chars)
sentences = [s.strip() for s in corpus.split('.') if 200 <= len(s.strip()) <= 400]
print(f"✅ ({len(sentences)} sentences available)")

# Pick a random sentence
plaintext = random.choice(sentences)

print("\n" + "="*80)
print("PLAINTEXT (Selected randomly from corpus)")
print("="*80)
print(f"\nLength: {len(plaintext)} characters")
print(f"\n{plaintext}\n")

# Initialize generator
cipher_gen = CipherGenerator()

# Generate all cipher types
print("="*80)
print("GENERATED CIPHERS")
print("="*80)

examples = []

# Substitution Ciphers
print("\n📝 SUBSTITUTION CIPHERS:")
print("-"*80)

# Caesar
caesar = cipher_gen.caesar(plaintext, shift=7)
print(f"\n1. Caesar (shift=7):")
print(f"   {caesar[:100]}...")
examples.append(("Caesar", caesar))

# ROT13
rot13 = cipher_gen.rot13(plaintext)
print(f"\n2. ROT13:")
print(f"   {rot13[:100]}...")
examples.append(("ROT13", rot13))

# Atbash
atbash = cipher_gen.atbash(plaintext)
print(f"\n3. Atbash:")
print(f"   {atbash[:100]}...")
examples.append(("Atbash", atbash))

# SimpleSubstitution
simple = cipher_gen.simple_substitution(plaintext)
print(f"\n4. Simple Substitution:")
print(f"   {simple[:100]}...")
examples.append(("SimpleSubstitution", simple))

# Polyalphabetic Ciphers
print("\n\n📝 POLYALPHABETIC CIPHERS:")
print("-"*80)

# Vigenere
vigenere = cipher_gen.vigenere(plaintext, key="CRYPTOGRAPHY")
print(f"\n5. Vigenere (key=CRYPTOGRAPHY):")
print(f"   {vigenere[:100]}...")
examples.append(("Vigenere", vigenere))

# Autokey
autokey = cipher_gen.autokey(plaintext, key="SECRET")
print(f"\n6. Autokey (key=SECRET):")
print(f"   {autokey[:100]}...")
examples.append(("Autokey", autokey))

# Trithemius
trithemius = cipher_gen.trithemius(plaintext)
print(f"\n7. Trithemius:")
print(f"   {trithemius[:100]}...")
examples.append(("Trithemius", trithemius))

# Transposition Ciphers
print("\n\n📝 TRANSPOSITION CIPHERS:")
print("-"*80)

# RailFence
railfence = cipher_gen.rail_fence(plaintext, rails=4)
print(f"\n8. Rail Fence (rails=4):")
print(f"   {railfence[:100]}...")
examples.append(("RailFence", railfence))

# Columnar
columnar = cipher_gen.columnar_transposition(plaintext, key="CIPHER")
print(f"\n9. Columnar (key=CIPHER):")
print(f"   {columnar[:100]}...")
examples.append(("Columnar", columnar))

# Route
route = cipher_gen.route_cipher(plaintext)
print(f"\n10. Route Cipher:")
print(f"   {route[:100]}...")
examples.append(("Route", route))

# Encoding Types
print("\n\n📝 ENCODINGS:")
print("-"*80)

# Base64
short_text = plaintext[:100]  # Shorter for encoding examples
base64 = cipher_gen.base64_encode(short_text)
print(f"\n11. Base64 (first 100 chars of plaintext):")
print(f"   {base64[:100]}...")
examples.append(("Base64", base64))

# Morse
morse_text = plaintext[:50]  # Even shorter for Morse
morse = cipher_gen.morse_code(morse_text)
print(f"\n12. Morse Code (first 50 chars of plaintext):")
print(f"   {morse[:100]}...")
examples.append(("Morse", morse))

# Binary
binary_text = plaintext[:30]
binary = cipher_gen.binary_encode(binary_text)
print(f"\n13. Binary (first 30 chars of plaintext):")
print(f"   {binary[:100]}...")
examples.append(("Binary", binary))

# Hex
hex_text = plaintext[:100]
hex_enc = cipher_gen.hex_encode(hex_text)
print(f"\n14. Hexadecimal (first 100 chars of plaintext):")
print(f"   {hex_enc[:100]}...")
examples.append(("Hex", hex_enc))

# Plaintext
print("\n\n📝 PLAINTEXT:")
print("-"*80)
print(f"\n15. Plaintext (no encryption):")
print(f"   {plaintext[:100]}...")
examples.append(("Plaintext", plaintext))

# Save to file
print("\n" + "="*80)
print("Saving examples to file...")

with open('testing/random_examples.txt', 'w', encoding='utf-8') as f:
    f.write("RANDOM CIPHER EXAMPLES\n")
    f.write("="*80 + "\n\n")
    f.write("PLAINTEXT:\n")
    f.write(plaintext + "\n\n")
    f.write("="*80 + "\n\n")
    
    for cipher_type, ciphertext in examples:
        f.write(f"{cipher_type.upper()}\n")
        f.write("-"*80 + "\n")
        f.write(ciphertext + "\n\n")

print("✅ Examples saved to testing/random_examples.txt")

# Interactive testing option
print("\n" + "="*80)
print("QUICK TEST")
print("="*80)
print("\nWould you like to test these with the predictor? (y/n): ", end="", flush=True)

try:
    response = input().strip().lower()
    
    if response == 'y':
        print("\n🔧 Loading predictor...")
        from src.predictor import CipherPredictor
        predictor = CipherPredictor()
        print("✅ Loaded\n")
        
        print("="*80)
        print("TESTING PREDICTIONS")
        print("="*80)
        
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
        
        print("\n" + "="*80)
        print(f"Results: {correct}/{total} correct ({correct/total*100:.1f}%)")
        print("="*80)
        
except KeyboardInterrupt:
    print("\n\nSkipped testing.")

print("\n✅ Done!")