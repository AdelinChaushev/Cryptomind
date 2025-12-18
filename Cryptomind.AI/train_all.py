"""
Master training script - generates data and trains both layers
"""
from src.data_generator import DataGenerator
from src.train_layer1 import train_layer1
from src.train_layer2 import train_layer2

def main():
    print("=" * 60)
    print("CRYPTOMIND ML TRAINING PIPELINE")
    print("=" * 60)
    
    # Step 1: Generate data
    print("\nSTEP 1: Generating Training Data")
    print("-" * 60)
    generator = DataGenerator()
    generator.load_corpus()
    generator.generate_layer1_data()
    generator.generate_layer2_data()
    
    # Step 2: Train Layer 1
    print("\n" + "=" * 60)
    print("STEP 2: Training Layer 1 (Family Classifier)")
    print("-" * 60)
    train_layer1()
    
    # Step 3: Train Layer 2
    print("\n" + "=" * 60)
    print("STEP 3: Training Layer 2 (Type Classifiers)")
    print("-" * 60)
    train_layer2()
    
    print("\n" + "=" * 60)
    print("✓ TRAINING COMPLETE!")
    print("=" * 60)
    print("\nYou can now run the API server with: python run_api.py")

if __name__ == '__main__':
    main()