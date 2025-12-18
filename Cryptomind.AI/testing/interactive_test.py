"""
Interactive cipher identification tool
Enter ciphertext and get instant predictions
"""
from src.predictor import CipherPredictor
import sys

def print_banner():
    print("\n" + "="*70)
    print("        🔐 CRYPTOMIND - INTERACTIVE CIPHER IDENTIFIER 🔐")
    print("="*70)
    print("Enter encrypted text to identify the cipher type")
    print("Type 'exit' or 'quit' to end the session")
    print("Type 'help' for usage examples")
    print("="*70 + "\n")

def print_help():
    print("\n" + "="*70)
    print("USAGE EXAMPLES:")
    print("="*70)
    print("\n1. Caesar Cipher:")
    print('   Input: "KHOOR ZRUOG"')
    print('   Output: Caesar (shift of 3)\n')
    
    print("2. Base64:")
    print('   Input: "SGVsbG8gV29ybGQ="')
    print('   Output: Base64 encoding\n')
    
    print("3. Vigenere:")
    print('   Input: "LXFOPVEFRNHR"')
    print('   Output: Vigenere cipher\n')
    
    print("TIPS:")
    print("  • Use at least 150 characters for best accuracy")
    print("  • System works best with 200-400 character texts")
    print("  • Supports: Caesar, Vigenere, Atbash, RailFence, and more")
    print("="*70 + "\n")

def format_prediction(result):
    """Format prediction results nicely"""
    top = result['top_prediction']
    
    print("\n" + "─"*70)
    print("📊 PREDICTION RESULTS")
    print("─"*70)
    
    # Main prediction
    print(f"\n🎯 Identified Cipher: {top['type']}")
    print(f"   Family: {top['family']}")
    print(f"   Overall Confidence: {top['confidence']*100:.2f}%")
    print(f"   ├─ Family Confidence: {top['family_confidence']*100:.2f}%")
    print(f"   └─ Type Confidence: {top['type_confidence']*100:.2f}%")
    
    # Text info
    print(f"\n📏 Text Statistics:")
    print(f"   Total Length: {result['text_length']} characters")
    print(f"   Letter Count: {result['letter_count']} letters")
    
    # Alternative predictions
    if len(result['all_predictions']) > 1:
        print(f"\n🔄 Alternative Predictions:")
        for i, pred in enumerate(result['all_predictions'][1:3], 2):  # Show top 2 alternatives
            print(f"   #{i}: {pred['type']} ({pred['confidence']*100:.2f}%)")
    
    print("─"*70 + "\n")

def main():
    print_banner()
    
    # Initialize predictor
    print("Loading models...", end=" ", flush=True)
    try:
        predictor = CipherPredictor()
        print("✅ Ready!\n")
    except Exception as e:
        print(f"\n❌ Error loading models: {e}")
        print("Make sure you're in the project directory and models are trained.")
        sys.exit(1)
    
    # Main loop
    while True:
        try:
            # Get input
            user_input = input("Enter ciphertext (or 'help'/'exit'): ").strip()
            
            # Check for commands
            if user_input.lower() in ['exit', 'quit', 'q']:
                print("\n👋 Thanks for using Cryptomind! Goodbye!\n")
                break
            
            if user_input.lower() == 'help':
                print_help()
                continue
            
            if not user_input:
                print("⚠️  Please enter some text!\n")
                continue
            
            # Check length warning
            if len(user_input) < 50:
                print(f"⚠️  Warning: Text is very short ({len(user_input)} chars). Accuracy may be low.")
                print("   Recommended: 150+ characters for reliable results.\n")
            elif len(user_input) < 150:
                print(f"⚠️  Note: Text is short ({len(user_input)} chars). Optimal range is 200-400 chars.\n")
            
            # Predict
            print("🔍 Analyzing...", end=" ", flush=True)
            result = predictor.predict(user_input)
            print("Done!")
            
            # Show results
            format_prediction(result)
            
        except KeyboardInterrupt:
            print("\n\n👋 Interrupted. Goodbye!\n")
            break
        except Exception as e:
            print(f"\n❌ Error: {e}\n")
            import traceback
            traceback.print_exc()
            continue

if __name__ == "__main__":
    main()