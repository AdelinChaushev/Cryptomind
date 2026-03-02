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
            image = self.preprocessor.preprocess(image_path) if preprocess else Image.open(image_path)
    
            text = pytesseract.image_to_string(image, config="--psm 6", lang="eng").strip()
    
            data = pytesseract.image_to_data(image, output_type=pytesseract.Output.DICT)
    
            # Fixed confidence calculation
            confidences = []
            for conf in data["conf"]:
                try:
                    c = int(conf)
                    if c > 0:
                        confidences.append(c)
                except (ValueError, TypeError):
                    continue
                
            avg_confidence = sum(confidences) / len(confidences) if confidences else 0
    
            return {
                "success": True,
                "text": text,
                "confidence": round(avg_confidence, 2),
                "char_count": len(text),
                "error": None
            }
    
        except Exception as e:
            return {
                "success": False,
                "text": "",
                "confidence": 0,
                "char_count": 0,
                "error": str(e)
            }