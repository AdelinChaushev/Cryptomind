import numpy as np
import os
import json
import random
from config.config import Config
from src.cipher_generator import CipherGenerator
from src.feature_extraction import FeatureExtractor

class DataGenerator:
    def __init__(self):
        self.cipher_gen = CipherGenerator()
        self.feature_extractor = FeatureExtractor()
        self.corpus_texts = []

    def load_corpus(self):
        corpus_file = os.path.join(Config.CORPUS_DIR, 'corpus.txt')

        if os.path.exists(corpus_file):
            with open(corpus_file, 'r', encoding='utf-8') as f:
                content = f.read()
                sentences = content.replace('\n', ' ').split('.')
                self.corpus_texts = [s.strip() + '.' for s in sentences if len(s.strip()) > 50]
        else:
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

        expanded = []
        for _ in range(1000):
            num_sentences = random.randint(2, 5)
            text = ' '.join(random.choices(sample_texts, k=num_sentences))
            expanded.append(text)

        return expanded

    def get_random_text(self, min_length, max_length):
        if not self.corpus_texts:
            self.load_corpus()
    
        attempts = 0
        while attempts < 1000:
            attempts += 1
            text = random.choice(self.corpus_texts)
    
            while len(text) < min_length:
                text += ' ' + random.choice(self.corpus_texts)
    
            if len(text) > max_length:
                text = text[:max_length]
    
            if min_length <= len(text) <= max_length:
                return text
    
        raise RuntimeError("Could not generate text within length bounds after 1000 attempts")

    def generate_layer1_data(self):
        os.makedirs(Config.LAYER1_TRAINING_DIR, exist_ok=True)

        all_data = []
        all_labels = []
        family_names = list(Config.FAMILIES.keys())

        for family_idx, (family, types) in enumerate(Config.FAMILIES.items()):
            print(f"\n{family}")

            samples_per_type = Config.SAMPLES_PER_TYPE // len(types)

            for cipher_type in types:
                print(f"  {cipher_type}: ", end='', flush=True)

                successful = 0
                attempts = 0
                max_attempts = samples_per_type * 2

                while successful < samples_per_type and attempts < max_attempts:
                    attempts += 1

                    try:
                        text_length = random.randint(Config.MIN_TEXT_LENGTH, Config.MAX_TEXT_LENGTH)
                        plaintext = self.get_random_text(Config.MIN_TEXT_LENGTH, text_length)

                        ciphertext = self.cipher_gen.generate(cipher_type, plaintext)
                        features = self.feature_extractor.extract(ciphertext)

                        if len(features) != Config.NUM_FEATURES:
                            continue

                        if np.any(np.isnan(features)) or np.any(np.isinf(features)):
                            continue

                        all_data.append(features)
                        all_labels.append(family_idx)
                        successful += 1

                    except Exception:
                        continue

                print(f"{successful} samples")

        X = np.array(all_data, dtype=np.float32)
        y = np.array(all_labels, dtype=np.int32)

        indices = np.random.permutation(len(X))
        X = X[indices]
        y = y[indices]

        np.save(os.path.join(Config.LAYER1_TRAINING_DIR, 'X.npy'), X)
        np.save(os.path.join(Config.LAYER1_TRAINING_DIR, 'y.npy'), y)

        label_map = {i: name for i, name in enumerate(family_names)}
        with open(os.path.join(Config.LAYER1_TRAINING_DIR, 'label_map.json'), 'w') as f:
            json.dump(label_map, f, indent=2)

        print(f"\nLayer 1: {len(X)} samples, {Config.NUM_FEATURES} features")

    def generate_layer2_data(self):
        for family, types in Config.FAMILIES.items():
            if len(types) == 1:
                print(f"\nSkipping {family} (only one type)")
                continue

            print(f"\n{family}")

            family_dir = os.path.join(Config.LAYER2_TRAINING_DIR, family.lower())
            os.makedirs(family_dir, exist_ok=True)

            all_data = []
            all_labels = []

            for type_idx, cipher_type in enumerate(types):
                print(f"  {cipher_type}: ", end='', flush=True)

                successful = 0
                attempts = 0
                max_attempts = Config.SAMPLES_PER_TYPE * 2

                while successful < Config.SAMPLES_PER_TYPE and attempts < max_attempts:
                    attempts += 1

                    try:
                        text_length = random.randint(Config.MIN_TEXT_LENGTH, Config.MAX_TEXT_LENGTH)
                        plaintext = self.get_random_text(Config.MIN_TEXT_LENGTH, text_length)

                        ciphertext = self.cipher_gen.generate(cipher_type, plaintext)
                        features = self.feature_extractor.extract(ciphertext)

                        if len(features) != Config.NUM_FEATURES:
                            continue

                        if np.any(np.isnan(features)) or np.any(np.isinf(features)):
                            continue

                        all_data.append(features)
                        all_labels.append(type_idx)
                        successful += 1

                    except Exception:
                        continue

                print(f"{successful} samples")

            X = np.array(all_data, dtype=np.float32)
            y = np.array(all_labels, dtype=np.int32)

            indices = np.random.permutation(len(X))
            X = X[indices]
            y = y[indices]

            np.save(os.path.join(family_dir, 'X.npy'), X)
            np.save(os.path.join(family_dir, 'y.npy'), y)

            label_map = {i: cipher_type for i, cipher_type in enumerate(types)}
            with open(os.path.join(family_dir, 'label_map.json'), 'w') as f:
                json.dump(label_map, f, indent=2)

            print(f"  {len(X)} samples saved")