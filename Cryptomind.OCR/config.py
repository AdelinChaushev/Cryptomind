import os


class Config:
    """Configuration for OCR service."""
    
    # Server Configuration
    HOST = '127.0.0.1'  # localhost - only accessible from same machine
    PORT = 5001  # Different from your C# API port (usually 5000 or 7000)
    DEBUG = True  # Set to False in production
    
    # File Upload Configuration
    UPLOAD_FOLDER = os.path.join(os.path.dirname(__file__), 'uploads')
    MAX_CONTENT_LENGTH = 10 * 1024 * 1024  # 10MB max file size
    ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif', 'bmp'}
    
    # Tesseract Configuration
    # IMPORTANT: Update this path based on your system
    # Windows default:
    TESSERACT_PATH = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
    
    # Mac (if installed via Homebrew):
    # TESSERACT_PATH = '/usr/local/bin/tesseract'
    
    # Linux:
    # TESSERACT_PATH = '/usr/bin/tesseract'
    
    # Or set to None if tesseract is in system PATH:
    # TESSERACT_PATH = None
    
    # OCR Configuration
    MIN_CHARS_FOR_CIPHER = 150  # Minimum characters needed for cipher classification
    
    # CORS Configuration (allow your C# API to call this service)
    CORS_ORIGINS = [
        'http://localhost:5000',   # Your C# API
        'http://localhost:7000',   # Alternative C# API port
        'http://localhost:3000',   # Your React frontend (if direct calls needed)
    ]


class ProductionConfig(Config):
    """Production-specific configuration."""
    DEBUG = False
    HOST = '0.0.0.0'  # Accept connections from any IP in production
    # Update TESSERACT_PATH for production server


class DevelopmentConfig(Config):
    """Development-specific configuration."""
    DEBUG = True


class TestingConfig(Config):
    """Testing-specific configuration."""
    TESTING = True
    UPLOAD_FOLDER = os.path.join(os.path.dirname(__file__), 'test_uploads')