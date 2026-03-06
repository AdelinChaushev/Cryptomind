import random
from src.cipher_generator import CipherGenerator

with open('data/corpus/corpus.txt', 'r', encoding='utf-8') as f:
    corpus = f.read()

sentences = [s.strip() for s in corpus.split('.') if 200 <= len(s.strip()) <= 400]

plaintext = random.choice(sentences)

print(f"Plaintext ({len(plaintext)} characters):")
print(f"\n{plaintext}\n")

cipher_gen = CipherGenerator()

examples = []

examples.append(("Caesar", cipher_gen.caesar(plaintext, shift=7)))
examples.append(("ROT13", cipher_gen.rot13(plaintext)))
examples.append(("Atbash", cipher_gen.atbash(plaintext)))
examples.append(("SimpleSubstitution", cipher_gen.simple_substitution(plaintext)))
examples.append(("Vigenere", cipher_gen.vigenere(plaintext, key="CRYPTOGRAPHY")))
examples.append(("Autokey", cipher_gen.autokey(plaintext, key="SECRET")))
examples.append(("Trithemius", cipher_gen.trithemius(plaintext)))
examples.append(("RailFence", cipher_gen.rail_fence(plaintext, rails=4)))
examples.append(("Columnar", cipher_gen.columnar_transposition(plaintext, key="CIPHER")))
examples.append(("Route", cipher_gen.route_cipher(plaintext)))
examples.append(("Base64", cipher_gen.base64_encode(plaintext[:100])))
examples.append(("Morse", cipher_gen.morse_code(plaintext[:50])))
examples.append(("Binary", cipher_gen.binary_encode(plaintext[:30])))
examples.append(("Hex", cipher_gen.hex_encode(plaintext[:100])))
examples.append(("Plaintext", plaintext))

with open('testing/random_examples.txt', 'w', encoding='utf-8') as f:
    f.write("Plaintext:\n")
    f.write(plaintext + "\n\n")

    for cipher_type, ciphertext in examples:
        f.write(f"{cipher_type}\n")
        f.write(ciphertext + "\n\n")

print("Would you like to test these with the predictor? (y/n): ", end="", flush=True)

try:
    response = input().strip().lower()

    if response == 'y':
        from src.predictor import CipherPredictor
        predictor = CipherPredictor()

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
                    status = "correct"
                else:
                    status = "wrong"

                print(f"{cipher_type:20s} -> {predicted:20s} ({confidence:.1f}%) [{status}]")

            except Exception as e:
                print(f"{cipher_type:20s} -> Error: {e}")

        print(f"\n{correct}/{total} correct ({correct/total*100:.1f}%)")

except KeyboardInterrupt:
    pass