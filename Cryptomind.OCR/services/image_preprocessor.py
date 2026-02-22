import cv2
import numpy as np
from PIL import Image


class ImagePreprocessor:
    def __init__(self):
        self.debug_mode = False  # Set to True to see intermediate processing steps
    
    def preprocess(self, image_path):
        # Load image using OpenCV
        img = cv2.imread(image_path)
        
        if img is None:
            raise ValueError(f"Could not load image from {image_path}")
        
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        deskewed = self._deskew(gray)
        
        denoised = self._denoise(deskewed)
        
        contrasted = self._enhance_contrast(denoised)
        
        binary = self._binarize(contrasted)
        
        pil_image = Image.fromarray(binary)
        
        return pil_image
    
    def _deskew(self, image):
        # Detect edges (where text is)
        edges = cv2.Canny(image, 50, 150, apertureSize=3)
        
        # Find lines using Hough transform
        lines = cv2.HoughLines(edges, 1, np.pi / 180, 100)
        
        if lines is None:
            return image  # No lines detected, return as-is
        
        # Calculate average angle of detected lines
        angles = []
        for rho, theta in lines[:, 0]:
            angle = np.degrees(theta) - 90
            angles.append(angle)
        
        median_angle = np.median(angles)
        
        # Only rotate if tilt is significant (more than 0.5 degrees)
        if abs(median_angle) < 0.5:
            return image
        
        # Rotate image to straighten it
        (h, w) = image.shape
        center = (w // 2, h // 2)
        M = cv2.getRotationMatrix2D(center, median_angle, 1.0)
        rotated = cv2.warpAffine(image, M, (w, h), 
                                 flags=cv2.INTER_CUBIC, 
                                 borderMode=cv2.BORDER_REPLICATE)
        
        return rotated
    
    def _denoise(self, image):
        # h=10: Filter strength (higher = more denoising but can blur text)
        # templateWindowSize=7: Size of patch to compare
        # searchWindowSize=21: Area to search for similar patches
        denoised = cv2.fastNlMeansDenoising(image, None, h=10, 
                                           templateWindowSize=7, 
                                           searchWindowSize=21)
        return denoised
    
    def _enhance_contrast(self, image):
        # clipLimit=2.0: Prevents over-enhancement
        # tileGridSize=(8,8): Size of tiles for local enhancement
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        enhanced = clahe.apply(image)
        return enhanced
    
    def _binarize(self, image):
        # THRESH_BINARY: Black text on white background
        # THRESH_OTSU: Automatically calculate best threshold
        _, binary = cv2.threshold(image, 0, 255, 
                                 cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        
        return binary
    
    def preprocess_with_multiple_methods(self, image_path):
        img = cv2.imread(image_path)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        results = []
        
        # Method 1: Full preprocessing pipeline
        results.append(("full_pipeline", self.preprocess(image_path)))
        
        # Method 2: Adaptive threshold (good for varying lighting)
        adaptive = cv2.adaptiveThreshold(gray, 255, 
                                        cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
                                        cv2.THRESH_BINARY, 11, 2)
        results.append(("adaptive", Image.fromarray(adaptive)))
        
        # Method 3: Simple binarization without other processing
        _, simple_binary = cv2.threshold(gray, 0, 255, 
                                        cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        results.append(("simple_binary", Image.fromarray(simple_binary)))
        
        # Method 4: Inverted (white text on black background)
        _, inverted = cv2.threshold(gray, 0, 255, 
                                   cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)
        results.append(("inverted", Image.fromarray(inverted)))
        
        return results