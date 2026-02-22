import re
from collections import Counter
from typing import Dict


class EnglishScorer:  
    # Expected English letter frequencies (percentage)
    ENGLISH_FREQ = {
        'A': 8.167, 'B': 1.492, 'C': 2.782, 'D': 4.253, 'E': 12.702,
        'F': 2.228, 'G': 2.015, 'H': 6.094, 'I': 6.966, 'J': 0.153,
        'K': 0.772, 'L': 4.025, 'M': 2.406, 'N': 6.749, 'O': 7.507,
        'P': 1.929, 'Q': 0.095, 'R': 5.987, 'S': 6.327, 'T': 9.056,
        'U': 2.758, 'V': 0.978, 'W': 2.360, 'X': 0.150, 'Y': 1.974,
        'Z': 0.074
    }
    
    # Common English bigrams (two-letter combinations)
    COMMON_BIGRAMS = {
        'TH', 'HE', 'IN', 'ER', 'AN', 'RE', 'ON', 'AT', 'EN', 'ND',
        'TI', 'ES', 'OR', 'TE', 'OF', 'ED', 'IS', 'IT', 'AL', 'AR',
        'ST', 'TO', 'NT', 'NG', 'SE', 'HA', 'AS', 'OU', 'IO', 'LE'
    }
    
    # Common English trigrams (three-letter combinations)
    COMMON_TRIGRAMS = {
        'THE', 'AND', 'ING', 'HER', 'HAT', 'HIS', 'THA', 'ERE', 'FOR', 'ENT',
        'ION', 'TER', 'WAS', 'YOU', 'ITH', 'VER', 'ALL', 'WIT', 'THI', 'TIO'
    }
    
    # Common English words dictionary
    COMMON_WORDS = {
        'THE', 'BE', 'TO', 'OF', 'AND', 'A', 'IN', 'THAT', 'HAVE', 'I',
        'IT', 'FOR', 'NOT', 'ON', 'WITH', 'HE', 'AS', 'YOU', 'DO', 'AT',
        'THIS', 'BUT', 'HIS', 'BY', 'FROM', 'THEY', 'WE', 'SAY', 'HER', 'SHE',
        'OR', 'AN', 'WILL', 'MY', 'ONE', 'ALL', 'WOULD', 'THERE', 'THEIR', 'WHAT',
        'SO', 'UP', 'OUT', 'IF', 'ABOUT', 'WHO', 'GET', 'WHICH', 'GO', 'ME',
        'WHEN', 'MAKE', 'CAN', 'LIKE', 'TIME', 'NO', 'JUST', 'HIM', 'KNOW', 'TAKE',
        'PEOPLE', 'INTO', 'YEAR', 'YOUR', 'GOOD', 'SOME', 'COULD', 'THEM', 'SEE', 'OTHER',
        'THAN', 'THEN', 'NOW', 'LOOK', 'ONLY', 'COME', 'ITS', 'OVER', 'THINK', 'ALSO',
        'BACK', 'AFTER', 'USE', 'TWO', 'HOW', 'OUR', 'WORK', 'FIRST', 'WELL', 'WAY',
        'EVEN', 'NEW', 'WANT', 'BECAUSE', 'ANY', 'THESE', 'GIVE', 'DAY', 'MOST', 'US',
        'IS', 'WAS', 'ARE', 'BEEN', 'HAS', 'HAD', 'WERE', 'SAID', 'DID', 'HAVING',
        'MAY', 'SHOULD', 'MIGHT', 'MUST', 'SHALL', 'BEING', 'DOES', 'AM', 'COULD',
        'VERY', 'MORE', 'HERE', 'SUCH', 'WHERE', 'MUCH', 'MANY', 'WELL', 'THOSE',
        'TELL', 'UNDER', 'NAME', 'DEVELOP', 'SAME', 'BECOME', 'HOWEVER', 'ANOTHER',
        'WORLD', 'STILL', 'NATION', 'HAND', 'OLD', 'LIFE', 'WRITE',
        'SEEM', 'PROVIDE', 'THREE', 'SMALL', 'BOTH', 'FIND', 'HOUSE', 'DURING',
        'WITHOUT', 'AGAIN', 'PLACE', 'AROUND', 'CASE', 'SHOW', 'OWN', 'POINT', 'LEAVE',
        'CALL', 'WHILE', 'LAST', 'RIGHT', 'MOVE', 'THING', 'GENERAL', 'SCHOOL', 'NEVER',
        'START', 'CITY', 'EARTH', 'EYE', 'LIGHT', 'THOUGHT', 'HEAD', 'POWER', 'BUSINESS',
        'SYSTEM', 'PROGRAM', 'QUESTION', 'RUN', 'GROUP', 'BELIEVE', 'HOLD', 'LARGE',
        'GOVERNMENT', 'HAPPEN', 'FACE', 'PUBLIC', 'PRESENT', 'BRING', 'IMPORTANT', 'NOTHING',
        'AREA', 'DONE', 'WATER', 'AWAY', 'ALWAYS', 'COURSE', 'FORM', 'IDEA', 'PART',
        'ONCE', 'WORD', 'INFORMATION', 'HIGH', 'EVERY', 'NEAR', 'ADD', 'FOOD', 'BETWEEN',
        'THROUGH', 'BEFORE', 'LINE', 'TOO', 'MEANS', 'BOY', 'FOLLOW', 'CAME', 'SET',
        'PUT', 'END', 'WHY', 'ASKED', 'WENT', 'MEN', 'READ', 'NEED', 'LAND', 'DIFFERENT',
        'HOME', 'TRY', 'KIND', 'PICTURE', 'CHANGE', 'OFF', 'PLAY', 'SPELL', 'AIR', 'ANIMAL',
        'PAGE', 'LETTER', 'MOTHER', 'ANSWER', 'FOUND', 'STUDY', 'LEARN', 'AMERICA'
    }
    
    def score_text(self, text: str) -> Dict:
        # Validation - text too short
        if not text or len(text.strip()) < 10:
            return self._error_result('Text too short for analysis (minimum 10 characters)')
        
        # Clean and prepare text
        cleaned_text = self._clean_text(text)
        
        if len(cleaned_text) < 10:
            return self._error_result('Insufficient letters after cleaning')
        
        # Calculate individual metrics
        freq_score = self._calculate_frequency_score(cleaned_text)
        bigram_score = self._calculate_bigram_score(cleaned_text)
        trigram_score = self._calculate_trigram_score(cleaned_text)
        word_score = self._calculate_word_score(text)
        ic_score = self._calculate_ic_score(cleaned_text)
        
        # Weighted combination of metrics
        # Word presence is most important (35%), then frequency (25%), IC (20%), 
        # bigrams (10%), trigrams (10%)
        confidence = (
            word_score * 0.35 +
            freq_score * 0.25 +
            ic_score * 0.20 +
            bigram_score * 0.10 +
            trigram_score * 0.10
        )
        
        # Determine if text is English
        # Multiple paths to acceptance:
        # 1. High overall confidence (>0.5)
        # 2. High word score + reasonable IC
        # 3. Good word score + good bigram patterns
        is_english = (
            confidence > 0.5 or 
            (word_score > 0.6 and ic_score > 0.3) or
            (word_score > 0.5 and bigram_score > 0.5)
        )
        
        return {
            'is_english': is_english,
            'confidence': round(confidence, 3),
            'metrics': {
                'frequency': round(freq_score, 3),
                'bigrams': round(bigram_score, 3),
                'trigrams': round(trigram_score, 3),
                'words': round(word_score, 3),
                'ic': round(ic_score, 3)
            },
            'details': {
                'text_length': len(text),
                'letter_count': len(cleaned_text),
                'word_count': len(text.split()),
                'ic_value': round(self._calculate_index_of_coincidence(cleaned_text), 4)
            }
        }
    
    def _error_result(self, error_message: str) -> Dict:
        return {
            'is_english': False,
            'confidence': 0.0,
            'metrics': {
                'frequency': 0.0,
                'bigrams': 0.0,
                'trigrams': 0.0,
                'words': 0.0,
                'ic': 0.0
            },
            'details': {
                'text_length': 0,
                'letter_count': 0,
                'word_count': 0,
                'ic_value': 0.0,
                'error': error_message
            }
        }
    
    def _clean_text(self, text: str) -> str:
        return re.sub(r'[^A-Za-z]', '', text).upper()
    
    def _calculate_frequency_score(self, text: str) -> float:
        if len(text) == 0:
            return 0.0
        
        letter_counts = Counter(text)
        total_letters = len(text)
        
        # Chi-squared statistic
        chi_squared = 0.0
        for letter in 'ABCDEFGHIJKLMNOPQRSTUVWXYZ':
            observed = (letter_counts.get(letter, 0) / total_letters) * 100
            expected = self.ENGLISH_FREQ[letter]
            chi_squared += ((observed - expected) ** 2) / (expected + 0.01)
        
        # Convert to 0-1 score (lower chi-squared = better match)
        # English text: chi-squared ≈ 20-50
        # Random text: chi-squared > 100
        score = max(0.0, 1.0 - (chi_squared / 100.0))
        return min(1.0, score)
    
    def _calculate_bigram_score(self, text: str) -> float:
        if len(text) < 2:
            return 0.0
        
        bigrams = [text[i:i+2] for i in range(len(text) - 1)]
        common_count = sum(1 for bg in bigrams if bg in self.COMMON_BIGRAMS)
        
        # English text typically has 30-40% common bigrams
        score = (common_count / len(bigrams)) / 0.4
        return min(1.0, score)
    
    def _calculate_trigram_score(self, text: str) -> float:
        if len(text) < 3:
            return 0.0
        
        trigrams = [text[i:i+3] for i in range(len(text) - 2)]
        common_count = sum(1 for tg in trigrams if tg in self.COMMON_TRIGRAMS)
        
        # English text typically has 15-20% common trigrams
        score = (common_count / len(trigrams)) / 0.2
        return min(1.0, score)
    
    def _calculate_word_score(self, text: str) -> float:
        words = re.findall(r'\b[A-Za-z]+\b', text)
        
        if not words:
            return 0.0
        
        # Full-weight for recognized words
        recognized_words = sum(1 for word in words if word.upper() in self.COMMON_WORDS)
        
        # Half-weight for short words (likely articles, pronouns not in dictionary)
        short_words = sum(0.5 for word in words if len(word) <= 3 and word.isalpha())
        
        total_weight = recognized_words + short_words
        score = total_weight / len(words)
        
        return min(1.0, score)
    
    def _calculate_ic_score(self, text: str) -> float:
        ic = self._calculate_index_of_coincidence(text)
        
        # Score based on proximity to English IC
        if 0.055 <= ic <= 0.075:
            return 1.0  # Perfect English range
        elif 0.045 <= ic < 0.055:
            return 0.7  # Acceptable
        elif 0.075 < ic <= 0.085:
            return 0.7  # Slightly high but okay
        else:
            return 0.3  # Too far from English
    
    def _calculate_index_of_coincidence(self, text: str) -> float:
        if len(text) < 2:
            return 0.0
        
        N = len(text)
        letter_counts = Counter(text)
        
        coincidences = sum(count * (count - 1) for count in letter_counts.values())
        ic = coincidences / (N * (N - 1)) if N > 1 else 0
        
        return ic