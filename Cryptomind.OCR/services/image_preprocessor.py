import cv2
import numpy as np
from PIL import Image


class ImagePreprocessor:
    """
    Prepares images for OCR by cleaning and enhancing them.
    
    Main tasks:
    - Convert to grayscale (removes color distractions)
    - Increase contrast (makes text stand out)
    - Remove noise (speckles, artifacts)
    - Straighten tilted images (deskewing)
    - Convert to pure black/white (binarization)
    """
    
    def __init__(self):
        self.debug_mode = False  # Set to True to see intermediate processing steps
    
    def preprocess(self, image_path):
        """
        Main preprocessing pipeline.
        
        Args:
            image_path: Path to image file
            
        Returns:
            Preprocessed PIL Image ready for OCR
        """
        # Load image using OpenCV
        img = cv2.imread(image_path)
        
        if img is None:
            raise ValueError(f"Could not load image from {image_path}")
        
        # Step 1: Convert to grayscale
        # Why? Color doesn't help OCR and can confuse it
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # Step 2: Deskew (straighten tilted images)
        # Why? OCR works best with straight horizontal text
        deskewed = self._deskew(gray)
        
        # Step 3: Remove noise
        # Why? Speckles and artifacts confuse OCR
        denoised = self._denoise(deskewed)
        
        # Step 4: Increase contrast
        # Why? Makes text stand out from background
        contrasted = self._enhance_contrast(denoised)
        
        # Step 5: Binarization (pure black text on white background)
        # Why? Simplifies image for OCR - just black text, white background
        binary = self._binarize(contrasted)
        
        # Convert back to PIL Image (Tesseract expects PIL format)
        pil_image = Image.fromarray(binary)
        
        return pil_image
    
    def _deskew(self, image):
        """
        Straighten tilted images.
        
        How it works:
        - Detects edges in image
        - Finds angle of text lines
        - Rotates image to make text horizontal
        """
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
        """
        Remove noise (speckles, artifacts).
        
        Uses Non-Local Means Denoising:
        - Looks at similar patches across image
        - Averages them to reduce noise while preserving edges
        """
        # h=10: Filter strength (higher = more denoising but can blur text)
        # templateWindowSize=7: Size of patch to compare
        # searchWindowSize=21: Area to search for similar patches
        denoised = cv2.fastNlMeansDenoising(image, None, h=10, 
                                           templateWindowSize=7, 
                                           searchWindowSize=21)
        return denoised
    
    def _enhance_contrast(self, image):
        """
        Make text stand out more from background.
        
        Uses CLAHE (Contrast Limited Adaptive Histogram Equalization):
        - Divides image into tiles
        - Enhances contrast in each tile separately
        - Better than global contrast adjustment
        """
        # clipLimit=2.0: Prevents over-enhancement
        # tileGridSize=(8,8): Size of tiles for local enhancement
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        enhanced = clahe.apply(image)
        return enhanced
    
    def _binarize(self, image):
        """
        Convert to pure black text on white background.
        
        Uses Otsu's method:
        - Automatically finds optimal threshold
        - Pixels darker than threshold become black
        - Pixels lighter than threshold become white
        """
        # THRESH_BINARY: Black text on white background
        # THRESH_OTSU: Automatically calculate best threshold
        _, binary = cv2.threshold(image, 0, 255, 
                                 cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        
        return binary
    
    def preprocess_with_multiple_methods(self, image_path):
        """
        Try different preprocessing approaches and return all of them.
        Useful when one method doesn't work well.
        
        Returns:
            List of preprocessed images using different strategies
        """
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