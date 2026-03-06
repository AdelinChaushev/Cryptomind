from src.data_generator import DataGenerator
from src.train_layer1 import train_layer1
from src.train_layer2 import train_layer2

def main():
    generator = DataGenerator()
    generator.load_corpus()
    generator.generate_layer1_data()
    generator.generate_layer2_data()
    
    train_layer1()
    train_layer2()
    print("TRAINING COMPLETE!")
if __name__ == '__main__':
    main()