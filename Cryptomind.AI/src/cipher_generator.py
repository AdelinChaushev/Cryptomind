import random
import string
import base64

class CipherGenerator:
    """Generate encrypted texts for all cipher types"""
    
    def __init__(self):
        self.alphabet = string.ascii_uppercase
    
    def caesar(self, plaintext, shift=None):
        """Caesar cipher with given or random shift"""
        if shift is None:
            shift = random.randint(1, 25)
        
        result = []
        for char in plaintext.upper():
            if char in self.alphabet:
                idx = (self.alphabet.index(char) + shift) % 26
                result.append(self.alphabet[idx])
            else:
                result.append(char)
        
        return ''.join(result)
    
    def rot13(self, plaintext):
        """ROT13 cipher (Caesar with shift 13)"""
        return self.caesar(plaintext, 13)
    
    def atbash(self, plaintext):
        """Atbash cipher (reversed alphabet)"""
        result = []
        for char in plaintext.upper():
            if char in self.alphabet:
                idx = 25 - self.alphabet.index(char)
                result.append(self.alphabet[idx])
            else:
                result.append(char)
        
        return ''.join(result)
    
    def simple_substitution(self, plaintext, key=None):
        """Simple substitution with random or given key"""
        if key is None:
            key = list(self.alphabet)
            random.shuffle(key)
            key = ''.join(key)
        
        result = []
        for char in plaintext.upper():
            if char in self.alphabet:
                idx = self.alphabet.index(char)
                result.append(key[idx])
            else:
                result.append(char)
        
        return ''.join(result)
    
    def vigenere(self, plaintext, key=None):
        """Vigenère cipher with random or given key"""
        if key is None:
            key_length = random.randint(3, 12)
            key = ''.join(random.choices(self.alphabet, k=key_length))
        
        result = []
        key_index = 0
        
        for char in plaintext.upper():
            if char in self.alphabet:
                char_idx = self.alphabet.index(char)
                key_char_idx = self.alphabet.index(key[key_index % len(key)])
                encrypted_idx = (char_idx + key_char_idx) % 26
                result.append(self.alphabet[encrypted_idx])
                key_index += 1
            else:
                result.append(char)
        
        return ''.join(result)
    
    def autokey(self, plaintext, key=None):
        """Autokey cipher"""
        if key is None:
            key = random.choice(self.alphabet)
        
        plaintext_upper = plaintext.upper()
        letters_only = ''.join(c for c in plaintext_upper if c in self.alphabet)
        
        full_key = key + letters_only[:-1]
        result = []
        key_index = 0
        
        for char in plaintext_upper:
            if char in self.alphabet:
                char_idx = self.alphabet.index(char)
                key_char_idx = self.alphabet.index(full_key[key_index])
                encrypted_idx = (char_idx + key_char_idx) % 26
                result.append(self.alphabet[encrypted_idx])
                key_index += 1
            else:
                result.append(char)
        
        return ''.join(result)
    
    def beaufort(self, plaintext, key=None):
        """Beaufort cipher"""
        if key is None:
            key_length = random.randint(3, 12)
            key = ''.join(random.choices(self.alphabet, k=key_length))
        
        result = []
        key_index = 0
        
        for char in plaintext.upper():
            if char in self.alphabet:
                char_idx = self.alphabet.index(char)
                key_char_idx = self.alphabet.index(key[key_index % len(key)])
                encrypted_idx = (key_char_idx - char_idx) % 26
                result.append(self.alphabet[encrypted_idx])
                key_index += 1
            else:
                result.append(char)
        
        return ''.join(result)
    
    def trithemius(self, plaintext):
        """Trithemius cipher (progressive Caesar)"""
        result = []
        shift = 0
        
        for char in plaintext.upper():
            if char in self.alphabet:
                idx = (self.alphabet.index(char) + shift) % 26
                result.append(self.alphabet[idx])
                shift += 1
            else:
                result.append(char)
        
        return ''.join(result)
    
    def rail_fence(self, plaintext, rails=None):
        """Rail Fence cipher"""
        if rails is None:
            rails = random.randint(2, 6)
        
        # Remove spaces for transposition
        text = ''.join(plaintext.split())
        
        if len(text) < rails:
            return text
        
        fence = [[] for _ in range(rails)]
        rail = 0
        direction = 1
        
        for char in text:
            fence[rail].append(char)
            rail += direction
            
            if rail == 0 or rail == rails - 1:
                direction *= -1
        
        return ''.join(''.join(row) for row in fence)
    
    def columnar_transposition(self, plaintext, key=None):
        """Columnar transposition"""
        if key is None:
            key_length = random.randint(3, 8)
            key = ''.join(random.sample(self.alphabet, key_length))
        
        # Remove spaces
        text = ''.join(plaintext.split())
        
        # Create column order from key
        sorted_key = sorted(enumerate(key), key=lambda x: x[1])
        column_order = [i[0] for i in sorted_key]
        
        # Pad text
        num_cols = len(key)
        num_rows = (len(text) + num_cols - 1) // num_cols
        padded_text = text + 'X' * (num_rows * num_cols - len(text))
        
        # Create grid
        grid = [padded_text[i:i+num_cols] for i in range(0, len(padded_text), num_cols)]
        
        # Read columns in key order
        result = []
        for col_idx in column_order:
            for row in grid:
                if col_idx < len(row):
                    result.append(row[col_idx])
        
        return ''.join(result)
    
    def route_cipher(self, plaintext):
        """Route cipher (spiral read)"""
        # Remove spaces
        text = ''.join(plaintext.split())
        
        # Determine grid size
        size = int(len(text) ** 0.5) + 1
        padded = text + 'X' * (size * size - len(text))
        
        # Create grid
        grid = [[padded[i * size + j] for j in range(size)] for i in range(size)]
        
        # Spiral read
        result = []
        top, bottom, left, right = 0, size - 1, 0, size - 1
        
        while top <= bottom and left <= right:
            for i in range(left, right + 1):
                result.append(grid[top][i])
            top += 1
            
            for i in range(top, bottom + 1):
                result.append(grid[i][right])
            right -= 1
            
            if top <= bottom:
                for i in range(right, left - 1, -1):
                    result.append(grid[bottom][i])
                bottom -= 1
            
            if left <= right:
                for i in range(bottom, top - 1, -1):
                    result.append(grid[i][left])
                left += 1
        
        return ''.join(result)
    
    def base64_encode(self, plaintext):
        """Base64 encoding"""
        return base64.b64encode(plaintext.encode()).decode()
    
    def morse_code(self, plaintext):
        """Morse code"""
        morse_dict = {
            'A': '.-', 'B': '-...', 'C': '-.-.', 'D': '-..', 'E': '.',
            'F': '..-.', 'G': '--.', 'H': '....', 'I': '..', 'J': '.---',
            'K': '-.-', 'L': '.-..', 'M': '--', 'N': '-.', 'O': '---',
            'P': '.--.', 'Q': '--.-', 'R': '.-.', 'S': '...', 'T': '-',
            'U': '..-', 'V': '...-', 'W': '.--', 'X': '-..-', 'Y': '-.--',
            'Z': '--..', ' ': '/'
        }
        
        result = []
        for char in plaintext.upper():
            if char in morse_dict:
                result.append(morse_dict[char])
        
        return ' '.join(result)
    
    def binary_encode(self, plaintext):
        """Binary encoding"""
        return ' '.join(format(ord(char), '08b') for char in plaintext)
    
    def hex_encode(self, plaintext):
        """Hexadecimal encoding"""
        return plaintext.encode().hex()
    
    def plaintext(self, text):
        """Return plaintext as-is"""
        return text
    
    def generate(self, cipher_type, plaintext):
        """Generate cipher of specified type"""
        method_map = {
            'Caesar': self.caesar,
            'ROT13': self.rot13,
            'Atbash': self.atbash,
            'SimpleSubstitution': self.simple_substitution,
            'Vigenere': self.vigenere,
            'Autokey': self.autokey,
            'Beaufort': self.beaufort,
            'Trithemius': self.trithemius,
            'RailFence': self.rail_fence,
            'Columnar': self.columnar_transposition,
            'Route': self.route_cipher,
            'Base64': self.base64_encode,
            'Morse': self.morse_code,
            'Binary': self.binary_encode,
            'Hex': self.hex_encode,
            'Plaintext': self.plaintext
        }
        
        if cipher_type not in method_map:
            raise ValueError(f"Unknown cipher type: {cipher_type}")
        
        return method_map[cipher_type](plaintext)