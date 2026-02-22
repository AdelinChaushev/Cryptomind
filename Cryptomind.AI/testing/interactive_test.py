from src.predictor import CipherPredictor
import sys

def format_prediction(result):
    top = result['top_prediction']
    
    print("PREDICTION RESULTS")
    
    # Main prediction
    print(f"\n Identified Cipher: {top['type']}")
    print(f"   Family: {top['family']}")
    print(f"   Overall Confidence: {top['confidence']*100:.2f}%")
    print(f"    Family Confidence: {top['family_confidence']*100:.2f}%")
    print(f"    Type Confidence: {top['type_confidence']*100:.2f}%")
    
    # Alternative predictions
    if len(result['all_predictions']) > 1:
        print(f"\nAlternative Predictions:")
        for i, pred in enumerate(result['all_predictions'][1:3], 2):  # Show top 2 alternatives
            print(f"   #{i}: {pred['type']} ({pred['confidence']*100:.2f}%)")
    
    print("─"*70 + "\n")

def main():
    
    # Initialize predictor
    print("Loading models...", end=" ", flush=True)
    try:
        predictor = CipherPredictor()
        print("Ready!\n")
    except Exception as e:
        print(f"\nError loading models: {e}")
        sys.exit(1)
    
    # Main loop
    while True:
        try:
            # Get input
            user_input = input("Enter ciphertext (or 'help'/'exit'): ").strip()
            
            # Check for commands
            if user_input.lower() in ['exit', 'quit', 'q']:
                break
            
            if not user_input:
                continue
            
            # Check length warning
            if len(user_input) < 50:
                print(f"Warning: Text is very short ({len(user_input)} chars). Accuracy may be low.")
                print("Recommended: 150+ characters for reliable results.\n")
            elif len(user_input) < 150:
                print(f"Note: Text is short ({len(user_input)} chars). Optimal range is 200-400 chars.\n")
            
            # Predict
            result = predictor.predict(user_input)
            
            # Show results
            format_prediction(result)
            
        except KeyboardInterrupt:
            print("\n\Program Ended!\n")
            break
        except Exception as e:
            print(f"\nError: {e}\n")
            import traceback
            traceback.print_exc()
            continue

if __name__ == "__main__":
    main()