import pytesseract
from PIL import Image
from .image_preprocessor import ImagePreprocessor


class OCRService:
    def __init__(self, tesseract_path=None):
        if tesseract_path:
            pytesseract.pytesseract.tesseract_cmd = tesseract_path
        
        self.preprocessor = ImagePreprocessor()
    
    def extract_text(self, image_path, preprocess=True):
        try:
            # Load and preprocess image
            if preprocess:
                image = self.preprocessor.preprocess(image_path)
            else:
                image = Image.open(image_path)
            
            # Extract text using Tesseract
            # --psm 6: Assume uniform block of text
            text = pytesseract.image_to_string(image, config='--psm 6', lang='eng')
            
            # Get detailed OCR data including confidence scores
            data = pytesseract.image_to_data(image, output_type=pytesseract.Output.DICT)
            
            # Calculate average confidence (only for non-empty words)
            confidences = [int(conf) for conf in data['conf'] if int(conf) > 0]
            avg_confidence = sum(confidences) / len(confidences) if confidences else 0
            
            text = text.strip()
            
            return {
                'success': True,
                'text': text,
                'confidence': round(avg_confidence, 2),
                'char_count': len(text),
                'word_count': len(text.split()),
                'error': None
            }
            
        except Exception as e:
            return {
                'success': False,
                'text': '',
                'confidence': 0,
                'char_count': 0,
                'word_count': 0,
                'error': str(e)
            }
    
    def extract_text_multiple_methods(self, image_path):
        preprocessed_images = self.preprocessor.preprocess_with_multiple_methods(image_path)
        
        best_result = None
        best_confidence = 0
        best_method = None
        
        for method_name, pil_image in preprocessed_images:
            try:
                text = pytesseract.image_to_string(pil_image, config='--psm 6', lang='eng')
                data = pytesseract.image_to_data(pil_image, output_type=pytesseract.Output.DICT)
                
                confidences = [int(conf) for conf in data['conf'] if int(conf) > 0]
                avg_confidence = sum(confidences) / len(confidences) if confidences else 0
                
                if avg_confidence > best_confidence:
                    best_confidence = avg_confidence
                    best_method = method_name
                    best_result = {
                        'success': True,
                        'text': text.strip(),
                        'confidence': round(avg_confidence, 2),
                        'char_count': len(text.strip()),
                        'word_count': len(text.strip().split()),
                        'method': method_name,
                        'error': None
                    }
            
            except Exception as e:
                continue
        
        if best_result is None:
            return {
                'success': False,
                'text': '',
                'confidence': 0,
                'char_count': 0,
                'word_count': 0,
                'method': None,
                'error': 'All preprocessing methods failed'
            }
        
        return best_result
    
    def validate_for_cipher(self, result, min_chars=150):
        warnings = []
        is_valid = True

        if not result['success']:
            return {
                'is_valid': False,
                'warnings': ['OCR extraction failed'],
                'recommendation': 'Try uploading a clearer image'
            }

        # CharCount - WARNING only, not blocking
        if result['char_count'] < min_chars:
            warnings.append(f'Text is short ({result["char_count"]} chars). Classification works best with {min_chars}+ characters.')
            # Don't set is_valid = False anymore!

        # Confidence - WARNING only
        if result['confidence'] < 60:
            warnings.append(f'Low OCR confidence ({result["confidence"]}%). Please review text carefully.')
        elif result['confidence'] < 80:
            warnings.append(f'Moderate OCR confidence ({result["confidence"]}%). Please verify extracted text.')

        # Only block if extraction completely failed
        if not result['text'] or len(result['text'].strip()) == 0:
            is_valid = False
            warnings.append('No text detected in image')

        # Recommendation
        if not is_valid:
            recommendation = 'No text found. Try: clearer image, better lighting, or manual text entry'
        elif warnings:
            recommendation = 'Please review extracted text before proceeding. You can edit any mistakes.'
        else:
            recommendation = 'Text extraction looks good!'

        return {
            'is_valid': is_valid,
            'warnings': warnings,
            'recommendation': recommendation
    }