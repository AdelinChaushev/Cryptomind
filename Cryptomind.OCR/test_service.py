import requests
import sys


def test_health():
    print("Testing health check endpoint...")
    try:
        response = requests.get('http://localhost:5001/health')
        if response.status_code == 200:
            data = response.json()
            print(f" Health check passed: {data['status']}")
            print(f"  Service: {data['service']}")
            print(f"  Version: {data['version']}")
            return True
        else:
            print(f" Health check failed: Status {response.status_code}")
            return False
    except requests.exceptions.ConnectionError:
        print(" Cannot connect to service. Is it running?")
        print("  Start service with: python app.py")
        return False
    except Exception as e:
        print(f" Health check error: {e}")
        return False


def test_ocr_with_file(image_path):
    print(f"\nTesting OCR extraction with: {image_path}")
    try:
        with open(image_path, 'rb') as f:
            files = {'image': f}
            response = requests.post('http://localhost:5001/ocr/extract', files=files)
        
        if response.status_code == 200:
            data = response.json()
            if data['success']:
                print(f" OCR extraction successful!")
                print(f"  Confidence: {data['confidence']}%")
                print(f"  Characters: {data['char_count']}")
                print(f"  Words: {data['word_count']}")
                print(f"  Validation: {'Valid' if data['validation']['is_valid'] else 'Invalid'}")
                
                if data['validation']['warnings']:
                    print(f"  Warnings:")
                    for warning in data['validation']['warnings']:
                        print(f"    - {warning}")
                
                print(f"\n  Text preview:")
                print(f"  {'-' * 50}")
                preview = data['text'][:200]
                print(f"  {preview}")
                if len(data['text']) > 200:
                    print(f"  ... (truncated, total {len(data['text'])} chars)")
                print(f"  {'-' * 50}")
                return True
            else:
                print(f" OCR failed: {data.get('error', 'Unknown error')}")
                return False
        else:
            print(f" Request failed: Status {response.status_code}")
            print(f"  Response: {response.text}")
            return False
    except FileNotFoundError:
        print(f" Image file not found: {image_path}")
        print("  Create a test image or provide path to existing image")
        return False
    except Exception as e:
        print(f" OCR test error: {e}")
        return False


def main():
    print("Cryptomind OCR Service Test")
    
    # Test 1: Health check
    health_ok = test_health()
    if not health_ok:
        print("Tests failed. Fix health check issues first.")
        sys.exit(1)
    
    # Test 2: OCR extraction (if image provided)
    if len(sys.argv) > 1:
        image_path = sys.argv[1]
        ocr_ok = test_ocr_with_file(image_path)
    else:
        print("To test OCR extraction, run:")
        print("  python test_service.py path/to/test/image.png")
        ocr_ok = None
    
    # Summary
    print("\n" + "=" * 60)
    print("Test Summary:")
    print(f"  Health Check: {' PASS' if health_ok else ' FAIL'}")
    if ocr_ok is not None:
        print(f"  OCR Extraction: {' PASS' if ocr_ok else ' FAIL'}")
    print("=" * 60)
    
    if health_ok and (ocr_ok is None or ocr_ok):
        print("\n All tests passed! OCR service is ready.")
        sys.exit(0)
    else:
        print("\n Some tests failed. Please fix issues.")
        sys.exit(1)


if __name__ == '__main__':
    main()