import cv2
import numpy as np
from PIL import Image


class ImagePreprocessor:
    def __init__(self):
        self.debug_mode = False

    def preprocess(self, image_path):
        img = cv2.imread(image_path)

        if img is None:
            raise ValueError(f"Could not load image from {image_path}")

        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        deskewed = self._deskew(gray)
        denoised = self._denoise(deskewed)
        contrasted = self._enhance_contrast(denoised)
        binary = self._binarize(contrasted)

        return Image.fromarray(binary)

    def _deskew(self, image):
        edges = cv2.Canny(image, 50, 150, apertureSize=3)
        lines = cv2.HoughLines(edges, 1, np.pi / 180, 100)

        if lines is None:
            return image

        angles = []
        for rho, theta in lines[:, 0]:
            angle = np.degrees(theta) - 90
            angles.append(angle)

        median_angle = np.median(angles)

        if abs(median_angle) < 0.5:
            return image

        (h, w) = image.shape
        center = (w // 2, h // 2)
        M = cv2.getRotationMatrix2D(center, median_angle, 1.0)
        return cv2.warpAffine(image, M, (w, h),
                              flags=cv2.INTER_CUBIC,
                              borderMode=cv2.BORDER_REPLICATE)

    def _denoise(self, image):
        return cv2.fastNlMeansDenoising(image, None, h=10,
                                        templateWindowSize=7,
                                        searchWindowSize=21)

    def _enhance_contrast(self, image):
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        return clahe.apply(image)

    def _binarize(self, image):
        _, binary = cv2.threshold(image, 0, 255,
                                  cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        return binary

    def preprocess_with_multiple_methods(self, image_path):
        img = cv2.imread(image_path)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        results = []

        results.append(("full_pipeline", self.preprocess(image_path)))

        adaptive = cv2.adaptiveThreshold(gray, 255,
                                         cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
                                         cv2.THRESH_BINARY, 11, 2)
        results.append(("adaptive", Image.fromarray(adaptive)))

        _, simple_binary = cv2.threshold(gray, 0, 255,
                                         cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        results.append(("simple_binary", Image.fromarray(simple_binary)))

        _, inverted = cv2.threshold(gray, 0, 255,
                                    cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)
        results.append(("inverted", Image.fromarray(inverted)))

        return results