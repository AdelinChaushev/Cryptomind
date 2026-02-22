from flask import Flask, request, jsonify
from flask_cors import CORS
from werkzeug.utils import secure_filename
import os
import time
from services.ocr_service import OCRService
from config import Config

app = Flask(__name__)
CORS(app)  # Enable CORS for C# API to call this service

# Load configuration
app.config.from_object(Config)

# Initialize OCR service
ocr_service = OCRService(tesseract_path=app.config['TESSERACT_PATH'])


def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in app.config['ALLOWED_EXTENSIONS']


def ensure_upload_folder():
    if not os.path.exists(app.config['UPLOAD_FOLDER']):
        os.makedirs(app.config['UPLOAD_FOLDER'])


@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({
        'status': 'healthy',
        'service': 'Cryptomind OCR Service',
        'version': '1.0.0'
    }), 200


@app.route('/ocr/extract', methods=['POST'])
def extract_text():
    # Validate request has file
    if 'image' not in request.files:
        return jsonify({
            'success': False,
            'error': 'No image file provided in request'
        }), 400
    
    file = request.files['image']
    
    # Validate file was selected
    if file.filename == '':
        return jsonify({
            'success': False,
            'error': 'No file selected'
        }), 400
    
    # Validate file type
    if not allowed_file(file.filename):
        return jsonify({
            'success': False,
            'error': f'Invalid file type. Allowed: {", ".join(app.config["ALLOWED_EXTENSIONS"])}'
        }), 400
    
    try:
        # Ensure upload directory exists
        ensure_upload_folder()
        
        # Generate unique filename
        filename = secure_filename(file.filename)
        timestamp = str(int(time.time()))
        unique_filename = f"{timestamp}_{filename}"
        filepath = os.path.join(app.config['UPLOAD_FOLDER'], unique_filename)
        
        # Save uploaded file
        file.save(filepath)
        
        # Extract text using OCR
        result = ocr_service.extract_text(filepath, preprocess=True)
        
        # Clean up uploaded file after processing
        if os.path.exists(filepath):
            os.remove(filepath)
        
        # Check if OCR succeeded
        if not result['success']:
            return jsonify({
                'success': False,
                'error': f"OCR extraction failed: {result['error']}"
            }), 500
        
        # Validate text for cipher classification
        validation = ocr_service.validate_for_cipher(result, min_chars=150)
        
        # Return successful result
        return jsonify({
            'success': True,
            'text': result['text'],
            'confidence': result['confidence'],
            'char_count': result['char_count'],
            'word_count': result['word_count'],
            'validation': {
                'is_valid': validation['is_valid'],
                'warnings': validation['warnings'],
                'recommendation': validation['recommendation']
            }
        }), 200
    
    except Exception as e:
        # Clean up file on error
        if 'filepath' in locals() and os.path.exists(filepath):
            os.remove(filepath)
        
        app.logger.error(f"OCR extraction error: {str(e)}")
        return jsonify({
            'success': False,
            'error': f'Server error: {str(e)}'
        }), 500


@app.route('/ocr/extract-multiple', methods=['POST'])
def extract_text_multiple_methods():
    if 'image' not in request.files:
        return jsonify({
            'success': False,
            'error': 'No image file provided'
        }), 400
    
    file = request.files['image']
    
    if file.filename == '':
        return jsonify({
            'success': False,
            'error': 'No file selected'
        }), 400
    
    if not allowed_file(file.filename):
        return jsonify({
            'success': False,
            'error': f'Invalid file type. Allowed: {", ".join(app.config["ALLOWED_EXTENSIONS"])}'
        }), 400
    
    try:
        ensure_upload_folder()
        
        filename = secure_filename(file.filename)
        timestamp = str(int(time.time()))
        unique_filename = f"{timestamp}_{filename}"
        filepath = os.path.join(app.config['UPLOAD_FOLDER'], unique_filename)
        
        file.save(filepath)
        
        # Try multiple preprocessing methods
        result = ocr_service.extract_text_multiple_methods(filepath)
        
        # Clean up
        if os.path.exists(filepath):
            os.remove(filepath)
        
        if not result['success']:
            return jsonify({
                'success': False,
                'error': f"All OCR methods failed: {result['error']}"
            }), 500
        
        validation = ocr_service.validate_for_cipher(result, min_chars=150)
        
        return jsonify({
            'success': True,
            'text': result['text'],
            'confidence': result['confidence'],
            'char_count': result['char_count'],
            'word_count': result['word_count'],
            'method': result['method'],
            'validation': {
                'is_valid': validation['is_valid'],
                'warnings': validation['warnings'],
                'recommendation': validation['recommendation']
            }
        }), 200
    
    except Exception as e:
        if 'filepath' in locals() and os.path.exists(filepath):
            os.remove(filepath)
        
        app.logger.error(f"OCR multiple methods error: {str(e)}")
        return jsonify({
            'success': False,
            'error': f'Server error: {str(e)}'
        }), 500


@app.errorhandler(413)
def file_too_large(e):
    return jsonify({
        'success': False,
        'error': f'File too large. Maximum size: {app.config["MAX_CONTENT_LENGTH"] / (1024*1024)}MB'
    }), 413


@app.errorhandler(500)
def internal_error(e):
    app.logger.error(f"Internal server error: {str(e)}")
    return jsonify({
        'success': False,
        'error': 'Internal server error'
    }), 500


if __name__ == '__main__':
    # Create upload folder on startup
    ensure_upload_folder()
    
    # Run Flask server
    print("=" * 60)
    print("Cryptomind OCR Service Starting...")
    print(f"Upload folder: {app.config['UPLOAD_FOLDER']}")
    print(f"Tesseract path: {app.config['TESSERACT_PATH']}")
    print(f"Running on: http://{app.config['HOST']}:{app.config['PORT']}")
    print("=" * 60)
    
    app.run(
        host=app.config['HOST'],
        port=app.config['PORT'],
        debug=app.config['DEBUG']
    )