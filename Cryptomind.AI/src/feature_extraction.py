import numpy as np
from collections import Counter
import re
from scipy.stats import entropy

class FeatureExtractor:
    """Extract 89 statistical features from ciphertext"""
    
    def __init__(self):
        self.alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
        # Most common English bigrams
        self.top_bigrams = [
            'TH', 'HE', 'IN', 'ER', 'AN', 'RE', 'ON', 'AT', 'EN', 'ND',
            'TI', 'ES', 'OR', 'TE', 'OF', 'ED', 'IS', 'IT', 'AL', 'AR'
        ]
        # English letter frequencies
        self.english_freq = {
        'E': 0.127, 'T': 0.091, 'A': 0.082, 'O': 0.075, 'I': 0.070,
        'N': 0.067, 'S': 0.063, 'H': 0.061, 'R': 0.060, 'D': 0.043,
        'L': 0.040, 'C': 0.028, 'U': 0.028, 'M': 0.024, 'W': 0.024,
        'F': 0.022, 'G': 0.020, 'Y': 0.020, 'P': 0.019, 'B': 0.015,
        'V': 0.010, 'K': 0.008, 'J': 0.001, 'X': 0.001, 'Q': 0.001, 'Z': 0.001
    }
    def _discriminative_features(self, text, letters_only):
        """
        4 discriminative features to distinguish statistically similar ciphers

        These help with:
        - Caesar vs SimpleSubstitution
        - Substitution vs Polyalphabetic (Vigenere/Autokey)
        - Trithemius vs Vigenere
        - Columnar vs RailFence
        """
        if len(letters_only) == 0:
            return [0.0, 0.0, 0.0, 0.0]

        counter = Counter(letters_only)
        freqs = [count / len(letters_only) for count in counter.values()]

        # Feature 1: Frequency variance
        # HIGH for substitution (preserves English spiky distribution)
        # LOW for polyalphabetic (flattens distribution)
        freq_variance = np.var(freqs) if len(freqs) > 1 else 0.0

        # Feature 2: Max frequency ratio
        # HIGH for substitution (~0.13 for 'E')
        # LOW for polyalphabetic (~0.04 uniform)
        max_freq = max(freqs) if freqs else 0.0

        # Feature 3: Distance from uniform distribution
        # Measures how "flat" the distribution is
        # LOW for substitution (far from uniform)
        # HIGH for polyalphabetic (close to uniform)
        uniform_freq = 1.0 / 26.0
        chi_sq_uniform = sum(
            ((counter.get(letter, 0) / len(letters_only) - uniform_freq) ** 2) / uniform_freq
            for letter in self.alphabet
        )
        # Normalize: 0 = perfectly uniform, 1 = very non-uniform
        flatness_score = 1.0 - min(chi_sq_uniform / 10.0, 1.0)

        # Feature 4: Digraph repetition pattern
        # Helps distinguish transposition from other types
        # HIGH for transposition (preserves digraphs)
        # MEDIUM for substitution (changes but maintains patterns)
        # LOW for polyalphabetic (destroys patterns)
        if len(letters_only) >= 2:
            digraphs = [letters_only[i:i+2] for i in range(len(letters_only) - 1)]
            digraph_counter = Counter(digraphs)
            repeated = sum(1 for count in digraph_counter.values() if count > 1)
            repetition_score = repeated / len(digraphs) if digraphs else 0.0
        else:
            repetition_score = 0.0

        return [freq_variance, max_freq, flatness_score, repetition_score]
    
    def extract(self, text):
        """Extract all 89 features from text"""
        text = text.upper()
        letters_only = re.sub(r'[^A-Z]', '', text)
        
        features = []
        
        # Features 1-26: Letter frequencies
        features.extend(self._letter_frequencies(letters_only))
        
        # Features 27-46: Top 20 bigram frequencies
        features.extend(self._bigram_frequencies(letters_only))
        
        # Feature 47: Index of Coincidence
        features.append(self._index_of_coincidence(letters_only))
        
        # Feature 48: Chi-squared statistic
        features.append(self._chi_squared(letters_only))
        
        # Feature 49: Entropy
        features.append(self._text_entropy(letters_only))
        
        # Features 50-54: Frequency distribution statistics
        features.extend(self._frequency_stats(letters_only))
        
        # Features 55-64: Positional entropy (10 segments)
        features.extend(self._positional_entropy(letters_only))
        
        # Features 65-74: Bigram disruption scores
        features.extend(self._bigram_disruption(letters_only))
        
        # Features 75-82: Polyalphabetic indicators
        features.extend(self._polyalphabetic_features(letters_only))
        
        # Features 83-88: Pattern features
        features.extend(self._pattern_features(text, letters_only))
        
        # Feature 89: Text length (normalized)
        features.append(self._normalized_length(letters_only))

       # Features 90-93: Discriminative features
        features.extend(self._discriminative_features(text, letters_only))
        
        # Feature 94: Text length (normalized) - moved to end
        features.append(self._normalized_length(letters_only))
        
        return np.array(features, dtype=np.float32)
    
    def _letter_frequencies(self, text):
        """Calculate frequency of each letter A-Z"""
        if len(text) == 0:
            return [0.0] * 26
        
        counter = Counter(text)
        total = len(text)
        return [counter.get(letter, 0) / total for letter in self.alphabet]
    
    def _bigram_frequencies(self, text):
        """Calculate frequency of top 20 English bigrams"""
        if len(text) < 2:
            return [0.0] * 20
        
        bigrams = [text[i:i+2] for i in range(len(text) - 1)]
        counter = Counter(bigrams)
        total = len(bigrams)
        
        return [counter.get(bg, 0) / total for bg in self.top_bigrams]
    
    def _index_of_coincidence(self, text):
        """Calculate Index of Coincidence"""
        if len(text) < 2:
            return 0.0
        
        n = len(text)
        counter = Counter(text)
        
        sum_freq = sum(count * (count - 1) for count in counter.values())
        ic = sum_freq / (n * (n - 1))
        
        return ic
    
    def _chi_squared(self, text):
        """Chi-squared test against English frequency"""
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
        
        # Normalize to 0-1 range
        return min(chi_sq / 10.0, 1.0)
    
    def _text_entropy(self, text):
        """Calculate Shannon entropy"""
        if len(text) == 0:
            return 0.0
        
        counter = Counter(text)
        probs = [count / len(text) for count in counter.values()]
        
        return entropy(probs, base=2) / 4.7  # Normalize (max ~4.7 for uniform)
    
    def _frequency_stats(self, text):
        """Statistical measures of frequency distribution"""
        if len(text) == 0:
            return [0.0] * 5
        
        freqs = list(Counter(text).values())
        freqs.sort(reverse=True)
        
        # Pad with zeros if less than 5 frequencies
        while len(freqs) < 5:
            freqs.append(0)
        
        total = len(text)
        
        return [
            freqs[0] / total,  # Most common letter ratio
            freqs[1] / total if len(freqs) > 1 else 0,  # 2nd most common
            freqs[2] / total if len(freqs) > 2 else 0,  # 3rd most common
            np.std(freqs) / total if len(freqs) > 1 else 0,  # Std dev
            (freqs[0] - freqs[-1]) / total if len(freqs) > 1 else 0  # Range
        ]
    
    def _positional_entropy(self, text):
        """Calculate entropy for 10 text segments"""
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
        """Measure how much common bigrams are disrupted"""
        if len(text) < 2:
            return [0.0] * 10
        
        bigrams = [text[i:i+2] for i in range(len(text) - 1)]
        counter = Counter(bigrams)
        
        # Check top 10 English bigrams preservation
        scores = []
        for bg in self.top_bigrams[:10]:
            score = counter.get(bg, 0) / len(bigrams) if len(bigrams) > 0 else 0
            scores.append(score)
        
        return scores
    
    def _polyalphabetic_features(self, text):
        """Features specific to polyalphabetic ciphers"""
        if len(text) < 10:
            return [0.0] * 8
        
        features = []
        
        # IC at different key length hypotheses (3, 4, 5, 6, 7, 8)
        for key_len in range(3, 9):
            ic_avg = 0.0
            for offset in range(key_len):
                substring = text[offset::key_len]
                if len(substring) > 1:
                    ic_avg += self._index_of_coincidence(substring)
            ic_avg /= key_len
            features.append(ic_avg)
        
        # Kasiski examination - average distance between repeated trigrams
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
        
        # Autocorrelation at offset 3
        autocorr = self._autocorrelation(text, 3)
        features.append(autocorr)
        
        return features
    
    def _autocorrelation(self, text, offset):
        """Calculate autocorrelation at given offset"""
        if len(text) <= offset:
            return 0.0
        
        matches = sum(1 for i in range(len(text) - offset) if text[i] == text[i + offset])
        return matches / (len(text) - offset)
    
    def _pattern_features(self, original_text, letters_only):
        """Pattern-based features from original text"""
        features = []
        
        # Special characters ratio
        if len(original_text) > 0:
            special_chars = len(re.findall(r'[^A-Za-z0-9\s]', original_text))
            features.append(special_chars / len(original_text))
        else:
            features.append(0.0)
        
        # Space ratio
        if len(original_text) > 0:
            spaces = original_text.count(' ')
            features.append(spaces / len(original_text))
        else:
            features.append(0.0)
        
        # Digit ratio
        if len(original_text) > 0:
            digits = len(re.findall(r'\d', original_text))
            features.append(digits / len(original_text))
        else:
            features.append(0.0)
        
        # Repeated character sequences (2+ same chars)
        if len(letters_only) > 0:
            repeated = len(re.findall(r'(.)\1+', letters_only))
            features.append(repeated / len(letters_only))
        else:
            features.append(0.0)
        
        # Unique letter ratio
        if len(letters_only) > 0:
            unique = len(set(letters_only))
            features.append(unique / 26.0)
        else:
            features.append(0.0)
        
        # Vowel ratio
        if len(letters_only) > 0:
            vowels = sum(1 for c in letters_only if c in 'AEIOU')
            features.append(vowels / len(letters_only))
        else:
            features.append(0.0)
        
        return features
    
    def _normalized_length(self, text):
        """Normalize text length to 0-1 range"""
        # Normalize with sigmoid-like function
        # 100 chars = ~0.5, 500 chars = ~0.9
        length = len(text)
        normalized = 1 - np.exp(-length / 150.0)
        return normalized