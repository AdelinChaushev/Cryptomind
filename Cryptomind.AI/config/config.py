import os

class Config:
    # Paths
    BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    DATA_DIR = os.path.join(BASE_DIR, 'data')
    TRAINING_DIR = os.path.join(DATA_DIR, 'training')
    CORPUS_DIR = os.path.join(DATA_DIR, 'corpus')
    MODELS_DIR = os.path.join(BASE_DIR, 'models')
    
    # Layer 1 paths
    LAYER1_TRAINING_DIR = os.path.join(TRAINING_DIR, 'layer1')
    LAYER1_MODEL_PATH = os.path.join(MODELS_DIR, 'layer1', 'family_classifier.h5')
    
    # Layer 2 paths
    LAYER2_TRAINING_DIR = os.path.join(TRAINING_DIR, 'layer2')
    LAYER2_MODELS_DIR = os.path.join(MODELS_DIR, 'layer2')
    
    # Cipher families and types
    # Cipher families and types
    # Cipher families and types
    FAMILIES = {
        'Substitution': ['Caesar', 'Atbash', 'SimpleSubstitution', 'ROT13'],
        'Polyalphabetic': ['Vigenere', 'Autokey', 'Trithemius'], 
        'Transposition': ['RailFence', 'Columnar', 'Route'],
        'Plaintext': ['Plaintext']
    }

    # Encoding types detected with rules (not ML)
    ENCODING_TYPES = ['Base64', 'Morse', 'Binary', 'Hex']
    
    # Training parameters
    SAMPLES_PER_TYPE = 5000
    MIN_TEXT_LENGTH = 150
    MAX_TEXT_LENGTH = 450
    VALIDATION_SPLIT = 0.15
    TEST_SPLIT = 0.15
    
    # Model parameters
    LAYER1_EPOCHS = 50
    LAYER2_EPOCHS = 40
    BATCH_SIZE = 64
    LEARNING_RATE = 0.001
    
    # Feature count
    NUM_FEATURES = 94
    
    # API settings
    API_HOST = '0.0.0.0'
    API_PORT = 5002
    
    # Confidence thresholds
    MIN_CONFIDENCE = 0.3
    TOP_K_PREDICTIONS = 2
    
    @staticmethod
    def get_family_from_type(cipher_type):
        """Get family name from cipher type"""
        for family, types in Config.FAMILIES.items():
            if cipher_type in types:
                return family
        return None
    
    @staticmethod
    def get_all_types():
        """Get flat list of all cipher types"""
        all_types = []
        for types in Config.FAMILIES.values():
            all_types.extend(types)
        return all_types