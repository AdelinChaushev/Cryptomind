import numpy as np
import os
import json
import tensorflow as tf
keras = tf.keras
from sklearn.model_selection import train_test_split
from config.config import Config

class Layer1Trainer:
    def __init__(self):
        self.model = None
        self.label_map = None

    def build_model(self, num_classes):
        model = keras.Sequential([
            keras.layers.Input(shape=(Config.NUM_FEATURES,)),
            keras.layers.Dense(256, activation='relu'),
            keras.layers.BatchNormalization(),
            keras.layers.Dropout(0.3),

            keras.layers.Dense(128, activation='relu'),
            keras.layers.BatchNormalization(),
            keras.layers.Dropout(0.3),

            keras.layers.Dense(64, activation='relu'),
            keras.layers.BatchNormalization(),
            keras.layers.Dropout(0.2),

            keras.layers.Dense(num_classes, activation='softmax')
        ])

        model.compile(
            optimizer=keras.optimizers.Adam(learning_rate=Config.LEARNING_RATE),
            loss='sparse_categorical_crossentropy',
            metrics=['accuracy']
        )

        return model

    def train(self):
        X = np.load(os.path.join(Config.LAYER1_TRAINING_DIR, 'X.npy'))
        y = np.load(os.path.join(Config.LAYER1_TRAINING_DIR, 'y.npy'))

        with open(os.path.join(Config.LAYER1_TRAINING_DIR, 'label_map.json'), 'r') as f:
            self.label_map = json.load(f)

        print(f"{len(X)} samples, {X.shape[1]} features, {len(self.label_map)} classes")

        X_train, X_temp, y_train, y_temp = train_test_split(
            X, y, test_size=(Config.VALIDATION_SPLIT + Config.TEST_SPLIT), random_state=42
        )

        val_ratio = Config.VALIDATION_SPLIT / (Config.VALIDATION_SPLIT + Config.TEST_SPLIT)
        X_val, X_test, y_val, y_test = train_test_split(
            X_temp, y_temp, test_size=(1 - val_ratio), random_state=42
        )

        print(f"Train: {len(X_train)}, Val: {len(X_val)}, Test: {len(X_test)}")

        self.model = self.build_model(len(self.label_map))

        callbacks = [
            keras.callbacks.EarlyStopping(
                monitor='val_loss',
                patience=10,
                restore_best_weights=True
            ),
            keras.callbacks.ReduceLROnPlateau(
                monitor='val_loss',
                factor=0.5,
                patience=5,
                min_lr=1e-6
            )
        ]

        history = self.model.fit(
            X_train, y_train,
            validation_data=(X_val, y_val),
            epochs=Config.LAYER1_EPOCHS,
            batch_size=Config.BATCH_SIZE,
            callbacks=callbacks,
            verbose=1
        )

        train_loss, train_acc = self.model.evaluate(X_train, y_train, verbose=0)
        val_loss, val_acc = self.model.evaluate(X_val, y_val, verbose=0)
        test_loss, test_acc = self.model.evaluate(X_test, y_test, verbose=0)

        print(f"Train: {train_acc*100:.2f}%  Val: {val_acc*100:.2f}%  Test: {test_acc*100:.2f}%")

        os.makedirs(os.path.dirname(Config.LAYER1_MODEL_PATH), exist_ok=True)
        self.model.save(Config.LAYER1_MODEL_PATH)

        return history

def train_layer1():
    trainer = Layer1Trainer()
    trainer.train()

if __name__ == '__main__':
    train_layer1()