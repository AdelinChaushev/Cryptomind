import os

class Config:

    # Server Configuration
    HOST = '0.0.0.0'
    PORT = 5001 
    
    UPLOAD_FOLDER = os.path.join(os.path.dirname(__file__), 'uploads')
    MAX_CONTENT_LENGTH = 10 * 1024 * 1024  # 10MB max file size
    ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif', 'bmp'}
    
    TESSERACT_PATH = '/usr/bin/tesseract' if os.name != 'nt' else r'C:\Program Files\Tesseract-OCR\tesseract.exe'

class DevelopmentConfig(Config):
    DEBUG = True


class TestingConfig(Config):
    TESTING = True
    UPLOAD_FOLDER = os.path.join(os.path.dirname(__file__), 'test_uploads')