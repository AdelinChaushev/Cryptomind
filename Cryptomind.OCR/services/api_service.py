from flask import Flask, request, jsonify
from flask_cors import CORS
from werkzeug.utils import secure_filename
import os
import time
from services.ocr_service import OCRService
from config import Config

app = Flask(__name__)
CORS(app)

app.config.from_object(Config)

ocr_service = OCRService()


def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in app.config['ALLOWED_EXTENSIONS']


def ensure_upload_folder():
    if not os.path.exists(app.config['UPLOAD_FOLDER']):
        os.makedirs(app.config['UPLOAD_FOLDER'])


@app.route('/health', methods=['GET'], strict_slashes=False)
def health_check():
    return jsonify({
        'status': 'healthy',
        'service': 'Cryptomind OCR Service',
        'version': '1.0.0'
    }), 200


@app.route('/ocr/extract', methods=['POST'], strict_slashes=False)
def extract_text():
    if 'image' not in request.files:
        return jsonify({'success': False, 'error': 'No image file provided'}), 400

    file = request.files['image']

    if file.filename == '':
        return jsonify({'success': False, 'error': 'No file selected'}), 400

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

        result = ocr_service.extract_text(filepath, preprocess=True)

        if os.path.exists(filepath):
            os.remove(filepath)

        if not result['success']:
            return jsonify({
                'success': False,
                'error': f"OCR extraction failed: {result['error']}"
            }), 500

        return jsonify({
            'success': True,
            'text': result['text'],
            'confidence': result['confidence'],
            'char_count': result['char_count']
        }), 200

    except Exception as e:
        if 'filepath' in locals() and os.path.exists(filepath):
            os.remove(filepath)
        app.logger.error(f"OCR extraction error: {str(e)}")
        return jsonify({'success': False, 'error': 'Server error occurred'}), 500


@app.errorhandler(413)
def file_too_large(e):
    max_mb = app.config["MAX_CONTENT_LENGTH"] / (1024 * 1024)
    return jsonify({
        'success': False,
        'error': f'File too large. Maximum size: {max_mb} MB'
    }), 413


@app.errorhandler(500)
def internal_error(e):
    app.logger.error(f"Internal server error: {str(e)}")
    return jsonify({'success': False, 'error': 'Internal server error'}), 500


def run_server():
    ensure_upload_folder()
    app.run(
        host=app.config['HOST'],
        port=app.config['PORT'],
        debug=app.config['DEBUG']
    )