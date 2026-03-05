from src.predictor import CipherPredictor
import sys

def format_prediction(result):
    top = result['top_prediction']

    print(f"\nCipher: {top['type']} ({top['family']})")
    print(f"  Overall confidence:  {top['confidence']*100:.2f}%")
    print(f"  Family confidence:   {top['family_confidence']*100:.2f}%")
    print(f"  Type confidence:     {top['type_confidence']*100:.2f}%")

    if len(result['all_predictions']) > 1:
        print("\nOther possibilities:")
        for i, pred in enumerate(result['all_predictions'][1:3], 2):
            print(f"  #{i}: {pred['type']} ({pred['confidence']*100:.2f}%)")

    print()

def main():
    try:
        predictor = CipherPredictor()
    except Exception as e:
        print(f"Error loading models: {e}")
        sys.exit(1)

    while True:
        try:
            user_input = input("Enter ciphertext (or 'exit'): ").strip()

            if user_input.lower() in ['exit', 'quit', 'q']:
                break

            if not user_input:
                continue

            if len(user_input) < 50:
                print(f"Text is very short ({len(user_input)} chars), accuracy may be low. Recommended: 150+ characters.\n")
            elif len(user_input) < 150:
                print(f"Text is short ({len(user_input)} chars), optimal range is 200-400 chars.\n")

            result = predictor.predict(user_input)
            format_prediction(result)

        except KeyboardInterrupt:
            break
        except Exception as e:
            print(f"Error: {e}\n")
            continue

if __name__ == "__main__":
    main()