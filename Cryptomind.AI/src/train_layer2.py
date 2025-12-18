import numpy as np
import os
import json
import tensorflow as tf
from tensorflow import keras
from sklearn.model_selection import train_test_split
from config.config import Config

class Layer2Trainer:
    """Train type classifiers for each family (Layer 2)"""
    
    def build_model(self, num_classes):
        """Build neural network for type classification"""
        model = keras.Sequential([
            keras.layers.Input(shape=(Config.NUM_FEATURES,)),
            keras.layers.Dense(128, activation='relu'),
            keras.layers.BatchNormalization(),
            keras.layers.Dropout(0.3),
            
            keras.layers.Dense(64, activation='relu'),
            keras.layers.BatchNormalization(),
            keras.layers.Dropout(0.2),
            
            keras.layers.Dense(32, activation='relu'),
            keras.layers.Dropout(0.2),
            
            keras.layers.Dense(num_classes, activation='softmax')
        ])
        
        model.compile(
            optimizer=keras.optimizers.Adam(learning_rate=Config.LEARNING_RATE),
            loss='sparse_categorical_crossentropy',
            metrics=['accuracy']
        )
        
        return model
    
    def train_family_model(self, family):
        """Train model for specific family"""
        print(f"\n=== Training {family} Type Classifier ===")
        
        family_dir = os.path.join(Config.LAYER2_TRAINING_DIR, family.lower())
        
        # Check if data exists
        if not os.path.exists(family_dir):
            print(f"No training data found for {family}")
            return None
        
        # Load data
        print("Loading training data...")
        X = np.load(os.path.join(family_dir, 'X.npy'))
        y = np.load(os.path.join(family_dir, 'y.npy'))
        
        with open(os.path.join(family_dir, 'label_map.json'), 'r') as f:
            label_map = json.load(f)
        
        print(f"Loaded {len(X)} samples")
        print(f"Classes: {len(label_map)}")
        
        # Split data
        X_train, X_temp, y_train, y_temp = train_test_split(
            X, y, test_size=(Config.VALIDATION_SPLIT + Config.TEST_SPLIT), random_state=42
        )
        
        val_ratio = Config.VALIDATION_SPLIT / (Config.VALIDATION_SPLIT + Config.TEST_SPLIT)
        X_val, X_test, y_val, y_test = train_test_split(
            X_temp, y_temp, test_size=(1 - val_ratio), random_state=42
        )
        
        print(f"Training: {len(X_train)}, Validation: {len(X_val)}, Test: {len(X_test)}")
        
        # Build model
        model = self.build_model(len(label_map))
        
        # Callbacks
        callbacks = [
            keras.callbacks.EarlyStopping(
                monitor='val_loss',
                patience=8,
                restore_best_weights=True
            ),
            keras.callbacks.ReduceLROnPlateau(
                monitor='val_loss',
                factor=0.5,
                patience=4,
                min_lr=1e-6
            )
        ]
        
        # Train
        print("Training...")
        history = model.fit(
            X_train, y_train,
            validation_data=(X_val, y_val),
            epochs=Config.LAYER2_EPOCHS,
            batch_size=Config.BATCH_SIZE,
            callbacks=callbacks,
            verbose=1
        )
        
        # Evaluate
        test_loss, test_acc = model.evaluate(X_test, y_test, verbose=0)
        print(f"Test Accuracy: {test_acc*100:.2f}%")
        
        # Save model
        model_dir = os.path.join(Config.LAYER2_MODELS_DIR, family.lower())
        os.makedirs(model_dir, exist_ok=True)
        model_path = os.path.join(model_dir, f'{family.lower()}_classifier.h5')
        model.save(model_path)
        
        print(f"✓ Model saved to: {model_path}")
        
        return history
    
    def train_all(self):
        """Train all Layer 2 models"""
        print("\n=== Training Layer 2: Type Classifiers ===")
        
        for family in Config.FAMILIES.keys():
            if len(Config.FAMILIES[family]) == 1:
                print(f"\nSkipping {family} (only one type)")
                continue
            
            self.train_family_model(family)
        
        print("\n✓ All Layer 2 models trained!")

def train_layer2():
    """Main training function for Layer 2"""
    trainer = Layer2Trainer()
    trainer.train_all()

if __name__ == '__main__':
    train_layer2()