import urllib.request
import re
import random
import json
import datetime
import os
from src.cipher_generator import CipherGenerator
from src.predictor import CipherPredictor

external_sources = []

# Source 1: Art of War
try:
    print("Downloading: The Art of War...", end=" ", flush=True)
    url = "https://www.gutenberg.org/files/132/132-0.txt"
    response = urllib.request.urlopen(url, timeout=15)
    text = response.read().decode('utf-8')
    start = text.find("I. LAYING PLANS")
    end = text.find("*** END OF THE PROJECT GUTENBERG")
    if start > 0 and end > 0:
        text = text[start:end]
    external_sources.append(("Art of War", text))
    print(f"({len(text) // 1024} KB)")
except Exception as e:
    print(f"{e}")

# Source 2: Metamorphosis
try:
    print("Downloading: Metamorphosis (Kafka)...", end=" ", flush=True)
    url = "https://www.gutenberg.org/files/5200/5200-0.txt"
    response = urllib.request.urlopen(url, timeout=15)
    text = response.read().decode('utf-8')
    start = text.find("One morning, when Gregor Samsa")
    end = text.find("*** END OF THE PROJECT GUTENBERG")
    if start > 0 and end > 0:
        text = text[start:end]
    external_sources.append(("Metamorphosis", text))
    print(f"({len(text) // 1024} KB)")
except Exception as e:
    print(f"{e}")

# Source 3: A Tale of Two Cities
try:
    print("Downloading: A Tale of Two Cities...", end=" ", flush=True)
    url = "https://www.gutenberg.org/files/98/98-0.txt"
    response = urllib.request.urlopen(url, timeout=15)
    text = response.read().decode('utf-8')
    start = text.find("It was the best of times")
    end = text.find("*** END OF THE PROJECT GUTENBERG")
    if start > 0 and end > 0:
        text = text[start:end]
    external_sources.append(("Tale of Two Cities", text))
    print(f"({len(text) // 1024} KB)")
except Exception as e:
    print(f"{e}")

# Fallback if downloads fail
if not external_sources:
    print("\nCould not download. Using fallback text...")
    fallback = """Modern cryptographic systems employ sophisticated mathematical algorithms to ensure secure communications in the digital age. These systems utilize complex number theory, including prime factorization and discrete logarithms, to create encryption schemes that are computationally infeasible to break without the proper decryption key. The development of quantum computing poses new challenges to traditional cryptographic methods, leading researchers to explore quantum-resistant algorithms that can withstand attacks from both classical and quantum computers. Understanding classical cipher systems provides essential foundation for comprehending these contemporary security mechanisms."""
    external_sources.append(("Fallback", fallback * 20))

test_sentences = []

for source_name, text in external_sources:
    text = re.sub(r'\s+', ' ', text)
    text = re.sub(r'[_\*\[\]]+', '', text)
    
    sentences = []
    for sent in re.split(r'[.!?]+', text):
        sent = sent.strip()
        if 200 <= len(sent) <= 400:
            letter_count = sum(1 for c in sent if c.isalpha())
            if letter_count >= 150:
                sentences.append(sent)
    
    print(f"  {source_name}: {len(sentences)} valid sentences")
    test_sentences.extend(sentences)

random.shuffle(test_sentences)

print(f"\n{len(test_sentences)} sentences available")

if len(test_sentences) < 110:
    print(f"\nOnly {len(test_sentences)} sentences available — some cipher types may get fewer samples")

# Initialize
cipher_gen = CipherGenerator()
predictor = CipherPredictor()

cipher_types = [
    'Caesar', 'ROT13', 'Atbash', 'SimpleSubstitution',
    'Vigenere', 'Autokey', 'Trithemius',
    'RailFence', 'Columnar', 'Route',
    'Plaintext'
]

SAMPLES_PER_TYPE = min(50, len(test_sentences) // len(cipher_types))

if SAMPLES_PER_TYPE < 10:
    print(f"Limited to {SAMPLES_PER_TYPE} samples per type")
    print("   (Minimum 10 recommended for statistical significance)")

results = {cipher: {'correct': 0, 'total': 0, 'failures': []} for cipher in cipher_types}

sentence_index = 0

for cipher_type in cipher_types:
    print(f"Testing {cipher_type:20s} ", end="", flush=True)
    
    for i in range(SAMPLES_PER_TYPE):
        if sentence_index >= len(test_sentences):
            print("\nRan out of test sentences!")
            break
            
        plaintext = test_sentences[sentence_index]
        sentence_index += 1
        
        try:
            if cipher_type == 'Plaintext':
                ciphertext = plaintext
            else:
                ciphertext = cipher_gen.generate(cipher_type, plaintext)
            
            if len(ciphertext) < 150:
                print("!", end="", flush=True)
                continue
            
            prediction = predictor.predict(ciphertext)
            top_pred = prediction['top_prediction']
            predicted_type = top_pred['type']
            confidence = top_pred['confidence'] * 100
            
            results[cipher_type]['total'] += 1
            
            if predicted_type == cipher_type:
                results[cipher_type]['correct'] += 1
                print("✓", end="", flush=True)
            else:
                results[cipher_type]['failures'].append({
                    'sample': i+1,
                    'length': len(ciphertext),
                    'predicted': predicted_type,
                    'confidence': confidence
                })
                print("X", end="", flush=True)
                
        except Exception as e:
            print(f"\nError: {e}")
            continue
    
    correct = results[cipher_type]['correct']
    total = results[cipher_type]['total']
    
    if total == 0:
        print(f"  Skipped (no valid samples)")
    else:
        accuracy = (correct / total * 100)
        print(f"  {correct}/{total} ({accuracy:.0f}%)")

# Calculate overall stats
total_correct = sum(r['correct'] for r in results.values())
total_tests = sum(r['total'] for r in results.values())

if total_tests == 0:
    print("\nNo valid test samples were generated. Check internet connection or text sources.")
    exit(1)

overall_accuracy = (total_correct / total_tests * 100)

print(f"\nOverall accuracy: {total_correct}/{total_tests} ({overall_accuracy:.2f}%)")

families = {
    'Substitution': ['Caesar', 'ROT13', 'Atbash', 'SimpleSubstitution'],
    'Polyalphabetic': ['Vigenere', 'Autokey', 'Trithemius'],
    'Transposition': ['RailFence', 'Columnar', 'Route'],
    'Plaintext': ['Plaintext']
}

print("\nResults by cipher family:")

for family, types in families.items():
    family_correct = sum(results[c]['correct'] for c in types if c in results)
    family_total = sum(results[c]['total'] for c in types if c in results)
    
    if family_total == 0:
        continue
        
    family_accuracy = (family_correct / family_total * 100)
    
    print(f"\n{family}: {family_correct}/{family_total} ({family_accuracy:.1f}%)")
    
    for cipher in types:
        if cipher in results and results[cipher]['total'] > 0:
            r = results[cipher]
            accuracy = (r['correct'] / r['total'] * 100)
            status = "good" if accuracy == 100 else "warning" if accuracy >= 80 else "bad"
            print(f"  [{status}] {cipher:20s} {r['correct']:2d}/{r['total']:2d} ({accuracy:5.1f}%)")
            
            if r['failures']:
                for failure in r['failures'][:2]:
                    print(f"    - Sample {failure['sample']}: predicted as {failure['predicted']} ({failure['confidence']:.1f}%)")

print(f"\nTraining corpus accuracy:   ~96.4%")
print(f"External validation:         {overall_accuracy:.1f}%")
print(f"Overfitting estimate:        {abs(96.4 - overall_accuracy):.1f}%")

if overall_accuracy >= 93:
    print("\nMinimal overfitting (<3%). Model generalizes very well.")
elif overall_accuracy >= 88:
    print("\nLow overfitting (<8%). Acceptable generalization.")
elif overall_accuracy >= 80:
    print("\nSome overfitting (>10%). Still usable but could improve.")
else:
    print("\nHigh overfitting (>15%). Model may be memorizing patterns.")

misclassifications = {}
for cipher, data in results.items():
    for failure in data['failures']:
        pair = f"{cipher} → {failure['predicted']}"
        misclassifications[pair] = misclassifications.get(pair, 0) + 1

if misclassifications:
    print("\nMost common misclassifications:")
    for pair, count in sorted(misclassifications.items(), key=lambda x: x[1], reverse=True)[:5]:
        print(f"  {pair}: {count} times")
else:
    print("\nNo misclassifications detected.")

# Save results

os.makedirs('testing', exist_ok=True)
os.makedirs('testing/results', exist_ok=True)
timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

test_data = {
    'metadata': {
        'timestamp': timestamp,
        'sources': [s[0] for s in external_sources],
        'total_samples': total_tests,
        'samples_per_type': SAMPLES_PER_TYPE,
        'overall_accuracy': overall_accuracy,
        'overfitting_estimate': abs(96.4 - overall_accuracy)
    },
    'test_samples': []
}

sentence_index = 0

for cipher_type in cipher_types:
    for i in range(SAMPLES_PER_TYPE):
        if sentence_index >= len(test_sentences):
            break
            
        plaintext = test_sentences[sentence_index]
        
        try:
            if cipher_type == 'Plaintext':
                ciphertext = plaintext
            else:
                ciphertext = cipher_gen.generate(cipher_type, plaintext)
            
            if len(ciphertext) < 150:
                sentence_index += 1
                continue
            
            was_correct = True
            predicted_as = cipher_type
            confidence_score = None
            
            for failure in results[cipher_type]['failures']:
                if failure['sample'] == i + 1:
                    was_correct = False
                    predicted_as = failure['predicted']
                    confidence_score = failure['confidence']
                    break
            
            test_data['test_samples'].append({
                'index': sentence_index,
                'cipher_type': cipher_type,
                'sample_number': i + 1,
                'plaintext': plaintext[:100] + '...' if len(plaintext) > 100 else plaintext,
                'ciphertext': ciphertext[:100] + '...' if len(ciphertext) > 100 else ciphertext,
                'plaintext_length': len(plaintext),
                'ciphertext_length': len(ciphertext),
                'correct': was_correct,
                'predicted_as': predicted_as,
                'confidence': confidence_score
            })
            
        except Exception as e:
            pass
        
        sentence_index += 1

with open('testing/external_validation_data.json', 'w', encoding='utf-8') as f:
    json.dump(test_data, f, indent=2, ensure_ascii=False)



with open('testing/external_validation_sentences.txt', 'w', encoding='utf-8') as f:
    f.write("External validation test sentences\n")
    f.write(f"Total sentences extracted: {len(test_sentences)}\n\n")
    
    for idx, sentence in enumerate(test_sentences, 1):
        f.write(f"[{idx}] ({len(sentence)} chars)\n")
        f.write(sentence + "\n\n")
        if idx >= 100:
            f.write(f"\n... (truncated after 100 sentences)\n")
            f.write(f"Total sentences available: {len(test_sentences)}\n")
            break



with open('testing/results/external_validation_results.txt', 'w', encoding='utf-8') as f:
    f.write("Cryptomind - External Data Validation Results\n\n")
    f.write(f"Test Date: {timestamp}\n")
    f.write(f"Test Sources: {', '.join([s[0] for s in external_sources])}\n")
    f.write(f"Total Samples: {total_tests}\n")
    f.write(f"Samples per Type: {SAMPLES_PER_TYPE}\n")
    f.write(f"Text Length: 200-400 characters\n\n")
    f.write(f"Overall Accuracy: {total_correct}/{total_tests} ({overall_accuracy:.2f}%)\n\n")
    
    for cipher, data in results.items():
        if data['total'] > 0:
            acc = (data['correct'] / data['total'] * 100)
            f.write(f"{cipher:20s} {data['correct']:2d}/{data['total']:2d} ({acc:5.1f}%)\n")