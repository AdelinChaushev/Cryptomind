import numpy as np
import os
import json
from config.config import Config
from src.cipher_generator import CipherGenerator
from src.feature_extraction import FeatureExtractor
import random

class DataGenerator:   
    def __init__(self):
        self.cipher_gen = CipherGenerator()
        self.feature_extractor = FeatureExtractor()
        self.corpus_texts = []
        
    def load_corpus(self):      
        # Try to load from file if exists
        corpus_file = os.path.join(Config.CORPUS_DIR, 'corpus.txt')
        
        if os.path.exists(corpus_file):
            with open(corpus_file, 'r', encoding='utf-8') as f:
                content = f.read()
                # Split into sentences
                sentences = content.replace('\n', ' ').split('.')
                self.corpus_texts = [s.strip() + '.' for s in sentences if len(s.strip()) > 50]
        else:
            # Generate sample corpus if file doesn't exist
            print("Corpus file not found, generating sample texts...")
            self.corpus_texts = self._generate_sample_corpus()
        
        print(f"Loaded {len(self.corpus_texts)} text samples")
    
    def _generate_sample_corpus(self):
        sample_texts = [
            "The quick brown fox jumps over the lazy dog near the river bank.",
            "Machine learning algorithms can identify patterns in encrypted messages.",
            "Cryptography has been used throughout history to protect sensitive information.",
            "The ancient Greeks developed various methods of secret communication.",
            "Modern encryption relies on complex mathematical algorithms and keys.",
            "Data security is essential in today's digital world and online communications.",
            "Pattern recognition helps in breaking classical cipher systems effectively.",
            "Statistical analysis reveals frequency distributions in encrypted texts.",
            "The development of computers revolutionized cryptanalysis techniques significantly.",
            "Understanding cipher mechanics requires knowledge of both mathematics and linguistics."
        ]
        
        # Expand corpus by combining sentences
        expanded = []
        for _ in range(1000):
            num_sentences = random.randint(2, 5)
            text = ' '.join(random.choices(sample_texts, k=num_sentences))
            expanded.append(text)
        
        return expanded
    
    def get_random_text(self, min_length, max_length):
        if not self.corpus_texts:
            self.load_corpus()
        
        while True:
            text = random.choice(self.corpus_texts)
            
            # Extract random substring if text is too long
            if len(text) > max_length:
                start = random.randint(0, len(text) - max_length)
                text = text[start:start + max_length]
            
            # Only return if within length bounds
            if min_length <= len(text) <= max_length:
                return text
    
    def generate_layer1_data(self):
        print("\n Generating Layer 1 Training Data")

        os.makedirs(Config.LAYER1_TRAINING_DIR, exist_ok=True)

        all_data = []
        all_labels = []
        family_names = list(Config.FAMILIES.keys())

        for family_idx, (family, types) in enumerate(Config.FAMILIES.items()):
            print(f"\nGenerating {family} family samples...")

            samples_per_type = Config.SAMPLES_PER_TYPE // len(types)

            for cipher_type in types:
                print(f"  - {cipher_type}: ", end='')

                successful = 0
                attempts = 0
                max_attempts = samples_per_type * 2  # Allow some failures

                while successful < samples_per_type and attempts < max_attempts:
                    if successful % 500 == 0:
                        print(f"{successful}", end='...')

                    attempts += 1

                    try:
                        # Get random plaintext
                        text_length = random.randint(Config.MIN_TEXT_LENGTH, Config.MAX_TEXT_LENGTH)
                        plaintext = self.get_random_text(Config.MIN_TEXT_LENGTH, text_length)

                        # Encrypt
                        ciphertext = self.cipher_gen.generate(cipher_type, plaintext)

                        # Extract features
                        features = self.feature_extractor.extract(ciphertext)

                        # VALIDATION: Ensure exactly 89 features
                        if len(features) != Config.NUM_FEATURES:
                            print(f"\n  Warning: {cipher_type} produced {len(features)} features instead of {Config.NUM_FEATURES}, skipping...")
                            continue
                        
                        # Check for NaN or Inf values
                        if np.any(np.isnan(features)) or np.any(np.isinf(features)):
                            continue
                        
                        all_data.append(features)
                        all_labels.append(family_idx)
                        successful += 1

                    except Exception as e:
                        print(f"\n  Error generating {cipher_type} sample: {e}")
                        continue
                    
                print(f" Done! ({successful} valid samples)")

        # Convert to numpy arrays
        X = np.array(all_data, dtype=np.float32)
        y = np.array(all_labels, dtype=np.int32)

        print(f"Array shape: {X.shape}")

        # Shuffle
        indices = np.random.permutation(len(X))
        X = X[indices]
        y = y[indices]

        # Save
        np.save(os.path.join(Config.LAYER1_TRAINING_DIR, 'X.npy'), X)
        np.save(os.path.join(Config.LAYER1_TRAINING_DIR, 'y.npy'), y)

        # Save label mapping
        label_map = {i: name for i, name in enumerate(family_names)}
        with open(os.path.join(Config.LAYER1_TRAINING_DIR, 'label_map.json'), 'w') as f:
            json.dump(label_map, f, indent=2)

        print(f"\n Layer 1 data generated: {len(X)} samples, {Config.NUM_FEATURES} features")
        print(f"  Saved to: {Config.LAYER1_TRAINING_DIR}")

    def generate_layer2_data(self):
        print("Generating Layer 2 Training Data")
        
        for family, types in Config.FAMILIES.items():
            if len(types) == 1:
                print(f"\nSkipping {family} (only one type)")
                continue
            
            print(f"\n--- {family} Family ---")
            
            family_dir = os.path.join(Config.LAYER2_TRAINING_DIR, family.lower())
            os.makedirs(family_dir, exist_ok=True)
            
            all_data = []
            all_labels = []
            
            for type_idx, cipher_type in enumerate(types):
                print(f"  Generating {cipher_type}: ", end='')
                
                successful = 0
                attempts = 0
                max_attempts = Config.SAMPLES_PER_TYPE * 2
                
                while successful < Config.SAMPLES_PER_TYPE and attempts < max_attempts:
                    if successful % 500 == 0:
                        print(f"{successful}", end='...')
                    
                    attempts += 1
                    
                    try:
                        # Get random plaintext
                        text_length = random.randint(Config.MIN_TEXT_LENGTH, Config.MAX_TEXT_LENGTH)
                        plaintext = self.get_random_text(Config.MIN_TEXT_LENGTH, text_length)
                        
                        # Encrypt
                        ciphertext = self.cipher_gen.generate(cipher_type, plaintext)
                        
                        # Extract features
                        features = self.feature_extractor.extract(ciphertext)
                        
                        # VALIDATION: Ensure exactly 89 features
                        if len(features) != Config.NUM_FEATURES:
                            continue
                        
                        # Check for NaN or Inf values
                        if np.any(np.isnan(features)) or np.any(np.isinf(features)):
                            continue
                        
                        all_data.append(features)
                        all_labels.append(type_idx)
                        successful += 1
                        
                    except Exception as e:
                        continue
                    
                print(f" Done! ({successful} samples)")
            
            # Convert to numpy arrays
            X = np.array(all_data, dtype=np.float32)
            y = np.array(all_labels, dtype=np.int32)
            
            # Shuffle
            indices = np.random.permutation(len(X))
            X = X[indices]
            y = y[indices]
            
            # Save
            np.save(os.path.join(family_dir, 'X.npy'), X)
            np.save(os.path.join(family_dir, 'y.npy'), y)
            
            # Save label mapping
            label_map = {i: cipher_type for i, cipher_type in enumerate(types)}
            with open(os.path.join(family_dir, 'label_map.json'), 'w') as f:
                json.dump(label_map, f, indent=2)
            
            print(f"{family} data: {len(X)} samples")
        
        print(f"\n All Layer 2 data generated")