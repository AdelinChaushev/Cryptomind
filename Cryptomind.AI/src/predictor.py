import numpy as np
import tensorflow as tf
keras = tf.keras
import os
import json
from config.config import Config
from src.feature_extraction import FeatureExtractor

class CipherPredictor:  
    def __init__(self):
        self.feature_extractor = FeatureExtractor()
        self.layer1_model = None
        self.layer1_labels = None
        self.layer2_models = {}
        self.layer2_labels = {}
        
        self.load_models()
    
    def load_models(self):     
        # Load Layer 1 model
        if os.path.exists(Config.LAYER1_MODEL_PATH):
            self.layer1_model = keras.models.load_model(
                Config.LAYER1_MODEL_PATH, compile=False
            )

            label_map_path = os.path.join(Config.LAYER1_TRAINING_DIR, 'label_map.json')
            with open(label_map_path, 'r') as f:
                label_map = json.load(f)
                self.layer1_labels = {int(k): v for k, v in label_map.items()}

            print(f"Layer 1 model loaded")
        else:
            print("Layer 1 model not found!")

        # Load Layer 2 models
        for family in Config.FAMILIES.keys():
            if len(Config.FAMILIES[family]) == 1:
                continue
            
            model_path = os.path.join(Config.LAYER2_MODELS_DIR, family.lower(), 
                                     f'{family.lower()}_classifier.h5')

            if os.path.exists(model_path):
                self.layer2_models[family] = keras.models.load_model(
                    model_path, compile=False
                )

                label_map_path = os.path.join(Config.LAYER2_TRAINING_DIR, 
                                              family.lower(), 'label_map.json')
                with open(label_map_path, 'r') as f:
                    label_map = json.load(f)
                    self.layer2_labels[family] = {int(k): v for k, v in label_map.items()}

                print(f"{family} model loaded")

        print("All models loaded!\n")

    def detect_encoding(self, text):
        text_no_space = text.replace(' ', '').replace('\n', '')

        if all(c in '.-/ ' for c in text):
            return 'Morse'

        if all(c in '01 ' for c in text) and len(text_no_space) > 20:
            return 'Binary'

        if all(c in '0123456789abcdefABCDEF' for c in text_no_space) and len(text_no_space) % 2 == 0:
            return 'Hex'

        base64_chars = set('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=')
        if all(c in base64_chars for c in text_no_space):
            if text_no_space.endswith('=') or text_no_space.endswith('=='):
                return 'Base64'

        return None
    
    def predict(self, ciphertext, return_top_k=True):
        # Check for encoding types first (rule-based)
        encoding_type = self.detect_encoding(ciphertext)
        if encoding_type:
            return {
                'top_prediction': {
                    'family': 'Encoding',
                    'type': encoding_type,
                    'confidence': 0.99,
                    'family_confidence': 0.99,
                    'type_confidence': 1.0
                },
                'all_predictions': [{
                    'family': 'Encoding',
                    'type': encoding_type,
                    'confidence': 0.99,
                    'family_confidence': 0.99,
                    'type_confidence': 1.0
                }],
                'text_length': len(ciphertext),
                'letter_count': len([c for c in ciphertext.upper() if c.isalpha()])
            }
        
        # Extract features
        features = self.feature_extractor.extract(ciphertext)
        features = features.reshape(1, -1)
        
        # Layer 1: Predict family
        family_probs = self.layer1_model.predict(features, verbose=0)[0]
        family_idx = np.argmax(family_probs)
        family_name = self.layer1_labels[family_idx]
        family_confidence = float(family_probs[family_idx])
        
        # Get top-k families
        top_k_families = []
        if return_top_k:
            top_k_indices = np.argsort(family_probs)[-Config.TOP_K_PREDICTIONS:][::-1]
            for idx in top_k_indices:
                top_k_families.append({
                    'family': self.layer1_labels[idx],
                    'confidence': float(family_probs[idx])
                })
        
        # Layer 2: Predict specific type
        predictions = []
        
        for family_info in (top_k_families if return_top_k else [{'family': family_name, 'confidence': family_confidence}]):
            current_family = family_info['family']
            family_conf = family_info['confidence']
            
            if len(Config.FAMILIES[current_family]) == 1:
                cipher_type = Config.FAMILIES[current_family][0]
                predictions.append({
                    'family': current_family,
                    'type': cipher_type,
                    'confidence': family_conf,
                    'family_confidence': family_conf,
                    'type_confidence': 1.0
                })
            else:
                if current_family in self.layer2_models:
                    type_probs = self.layer2_models[current_family].predict(features, verbose=0)[0]
                    
                    type_idx = np.argmax(type_probs)
                    cipher_type = self.layer2_labels[current_family][type_idx]
                    type_confidence = float(type_probs[type_idx])
                    
                    combined_confidence = family_conf * type_confidence
                    
                    predictions.append({
                        'family': current_family,
                        'type': cipher_type,
                        'confidence': combined_confidence,
                        'family_confidence': family_conf,
                        'type_confidence': type_confidence
                    })
        
        predictions.sort(key=lambda x: x['confidence'], reverse=True)
        
        result = {
            'top_prediction': predictions[0] if predictions else None,
            'all_predictions': predictions[:Config.TOP_K_PREDICTIONS] if return_top_k else predictions[:1],
            'text_length': len(ciphertext),
            'letter_count': len([c for c in ciphertext.upper() if c.isalpha()])
        }
        
        return result