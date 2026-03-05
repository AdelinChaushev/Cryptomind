import numpy as np
from collections import Counter
import re
from scipy.stats import entropy

class FeatureExtractor:
    def __init__(self):
        self.alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
        self.top_bigrams = [
            'TH', 'HE', 'IN', 'ER', 'AN', 'RE', 'ON', 'AT', 'EN', 'ND',
            'TI', 'ES', 'OR', 'TE', 'OF', 'ED', 'IS', 'IT', 'AL', 'AR'
        ]
        self.english_freq = {
            'E': 0.127, 'T': 0.091, 'A': 0.082, 'O': 0.075, 'I': 0.070,
            'N': 0.067, 'S': 0.063, 'H': 0.061, 'R': 0.060, 'D': 0.043,
            'L': 0.040, 'C': 0.028, 'U': 0.028, 'M': 0.024, 'W': 0.024,
            'F': 0.022, 'G': 0.020, 'Y': 0.020, 'P': 0.019, 'B': 0.015,
            'V': 0.010, 'K': 0.008, 'J': 0.001, 'X': 0.001, 'Q': 0.001, 'Z': 0.001
        }

    def _discriminative_features(self, text, letters_only):
        if len(letters_only) == 0:
            return [0.0, 0.0, 0.0, 0.0]

        counter = Counter(letters_only)
        freqs = [count / len(letters_only) for count in counter.values()]

        freq_variance = np.var(freqs) if len(freqs) > 1 else 0.0
        max_freq = max(freqs) if freqs else 0.0

        uniform_freq = 1.0 / 26.0
        chi_sq_uniform = sum(
            ((counter.get(letter, 0) / len(letters_only) - uniform_freq) ** 2) / uniform_freq
            for letter in self.alphabet
        )
        flatness_score = 1.0 - min(chi_sq_uniform / 10.0, 1.0)

        if len(letters_only) >= 2:
            digraphs = [letters_only[i:i+2] for i in range(len(letters_only) - 1)]
            digraph_counter = Counter(digraphs)
            repeated = sum(1 for count in digraph_counter.values() if count > 1)
            repetition_score = repeated / len(digraphs) if digraphs else 0.0
        else:
            repetition_score = 0.0

        return [freq_variance, max_freq, flatness_score, repetition_score]

    def extract(self, text):
        text = text.upper()
        letters_only = re.sub(r'[^A-Z]', '', text)

        features = []
        features.extend(self._letter_frequencies(letters_only))
        features.extend(self._bigram_frequencies(letters_only))
        features.append(self._index_of_coincidence(letters_only))
        features.append(self._chi_squared(letters_only))
        features.append(self._text_entropy(letters_only))
        features.extend(self._frequency_stats(letters_only))
        features.extend(self._positional_entropy(letters_only))
        features.extend(self._bigram_disruption(letters_only))
        features.extend(self._polyalphabetic_features(letters_only))
        features.extend(self._pattern_features(text, letters_only))
        features.append(self._normalized_length(letters_only))
        features.extend(self._discriminative_features(text, letters_only))
        features.append(self._normalized_length(letters_only))

        return np.array(features, dtype=np.float32)

    def _letter_frequencies(self, text):
        if len(text) == 0:
            return [0.0] * 26

        counter = Counter(text)
        total = len(text)
        return [counter.get(letter, 0) / total for letter in self.alphabet]

    def _bigram_frequencies(self, text):
        if len(text) < 2:
            return [0.0] * 20

        bigrams = [text[i:i+2] for i in range(len(text) - 1)]
        counter = Counter(bigrams)
        total = len(bigrams)

        return [counter.get(bg, 0) / total for bg in self.top_bigrams]

    def _index_of_coincidence(self, text):
        if len(text) < 2:
            return 0.0

        n = len(text)
        counter = Counter(text)

        sum_freq = sum(count * (count - 1) for count in counter.values())
        ic = sum_freq / (n * (n - 1))

        return ic

    def _chi_squared(self, text):
        if len(text) == 0:
            return 0.0

        observed = Counter(text)
        total = len(text)

        chi_sq = 0.0
        for letter in self.alphabet:
            observed_freq = observed.get(letter, 0) / total
            expected_freq = self.english_freq[letter]
            if expected_freq > 0:
                chi_sq += ((observed_freq - expected_freq) ** 2) / expected_freq

        return min(chi_sq / 10.0, 1.0)

    def _text_entropy(self, text):
        if len(text) == 0:
            return 0.0

        counter = Counter(text)
        probs = [count / len(text) for count in counter.values()]

        return entropy(probs, base=2) / 4.7

    def _frequency_stats(self, text):
        if len(text) == 0:
            return [0.0] * 5

        freqs = list(Counter(text).values())
        freqs.sort(reverse=True)

        while len(freqs) < 5:
            freqs.append(0)

        total = len(text)

        return [
            freqs[0] / total,
            freqs[1] / total if len(freqs) > 1 else 0,
            freqs[2] / total if len(freqs) > 2 else 0,
            np.std(freqs) / total if len(freqs) > 1 else 0,
            (freqs[0] - freqs[-1]) / total if len(freqs) > 1 else 0
        ]

    def _positional_entropy(self, text):
        if len(text) < 10:
            return [0.0] * 10

        segment_size = len(text) // 10
        entropies = []

        for i in range(10):
            start = i * segment_size
            end = start + segment_size if i < 9 else len(text)
            segment = text[start:end]

            if len(segment) > 0:
                counter = Counter(segment)
                probs = [count / len(segment) for count in counter.values()]
                ent = entropy(probs, base=2) / 4.7
                entropies.append(ent)
            else:
                entropies.append(0.0)

        return entropies

    def _bigram_disruption(self, text):
        if len(text) < 2:
            return [0.0] * 10

        bigrams = [text[i:i+2] for i in range(len(text) - 1)]
        counter = Counter(bigrams)

        scores = []
        for bg in self.top_bigrams[:10]:
            score = counter.get(bg, 0) / len(bigrams) if len(bigrams) > 0 else 0
            scores.append(score)

        return scores

    def _polyalphabetic_features(self, text):
        if len(text) < 10:
            return [0.0] * 8

        features = []

        for key_len in range(3, 9):
            ic_avg = 0.0
            for offset in range(key_len):
                substring = text[offset::key_len]
                if len(substring) > 1:
                    ic_avg += self._index_of_coincidence(substring)
            ic_avg /= key_len
            features.append(ic_avg)

        trigrams = {}
        for i in range(len(text) - 2):
            trigram = text[i:i+3]
            if trigram in trigrams:
                trigrams[trigram].append(i)
            else:
                trigrams[trigram] = [i]

        distances = []
        for positions in trigrams.values():
            if len(positions) > 1:
                for i in range(len(positions) - 1):
                    distances.append(positions[i+1] - positions[i])

        avg_distance = np.mean(distances) / len(text) if distances else 0.0
        features.append(min(avg_distance, 1.0))

        features.append(self._autocorrelation(text, 3))

        return features

    def _autocorrelation(self, text, offset):
        if len(text) <= offset:
            return 0.0

        matches = sum(1 for i in range(len(text) - offset) if text[i] == text[i + offset])
        return matches / (len(text) - offset)

    def _pattern_features(self, original_text, letters_only):
        features = []

        if len(original_text) > 0:
            special_chars = len(re.findall(r'[^A-Za-z0-9\s]', original_text))
            features.append(special_chars / len(original_text))
        else:
            features.append(0.0)

        if len(original_text) > 0:
            spaces = original_text.count(' ')
            features.append(spaces / len(original_text))
        else:
            features.append(0.0)

        if len(original_text) > 0:
            digits = len(re.findall(r'\d', original_text))
            features.append(digits / len(original_text))
        else:
            features.append(0.0)

        if len(letters_only) > 0:
            repeated = len(re.findall(r'(.)\1+', letters_only))
            features.append(repeated / len(letters_only))
        else:
            features.append(0.0)

        if len(letters_only) > 0:
            unique = len(set(letters_only))
            features.append(unique / 26.0)
        else:
            features.append(0.0)

        if len(letters_only) > 0:
            vowels = sum(1 for c in letters_only if c in 'AEIOU')
            features.append(vowels / len(letters_only))
        else:
            features.append(0.0)

        return features

    def _normalized_length(self, text):
        return 1 - np.exp(-len(text) / 150.0)