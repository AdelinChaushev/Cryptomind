"""
Flask API Service for Cryptomind ML
Now includes English text validation endpoint
"""
from flask import Flask, request, jsonify
from flask_cors import CORS
from config.config import Config
from src.predictor import CipherPredictor
from src.english_scorer import EnglishScorer  # NEW IMPORT

app = Flask(__name__)
CORS(app)

# Initialize predictor and scorer
predictor = None
english_scorer = None  # NEW GLOBAL VARIABLE

@app.before_request
def initialize():
    """Initialize predictor and English scorer on first request"""
    global predictor, english_scorer
    if predictor is None:
        print("Initializing predictor...")
        predictor = CipherPredictor()
    if english_scorer is None:  # NEW INITIALIZATION
        print("Initializing English scorer...")
        english_scorer = EnglishScorer()

@app.route('/api/health', methods=['GET'])
def health():
    """Health check endpoint"""
    return jsonify({'status': 'healthy', 'service': 'Cryptomind ML'})

@app.route('/api/predict', methods=['POST'])
def predict():
    """Predict cipher type from ciphertext"""
    try:
        data = request.get_json()
        
        if not data or 'text' not in data:
            return jsonify({'error': 'Missing "text" field'}), 400
        
        ciphertext = data['text']
        
        if not ciphertext or len(ciphertext.strip()) == 0:
            return jsonify({'error': 'Empty text provided'}), 400
        
        # Get prediction
        return_top_k = data.get('return_top_k', True)
        result = predictor.predict(ciphertext, return_top_k=return_top_k)
        
        return jsonify(result)
    
    except Exception as e:
        return jsonify({'error': str(e)}), 500
    
@app.route('/api/validate-english', methods=['POST'])
def validate_english():
    """
    Validate if plaintext is legitimate English text.
    
    Request Body:
    {
        "text": "The text to validate"
    }
    
    Response:
    {
        "is_english": true,
        "confidence": 0.753,
        "metrics": {
            "frequency": 0.531,
            "bigrams": 1.0,
            "trigrams": 0.714,
            "words": 0.688,
            "ic": 1.0
        },
        "details": {
            "text_length": 47,
            "letter_count": 39,
            "word_count": 8,
            "ic_value": 0.0624
        }
    }
    """
    try:
        data = request.get_json()
        
        if not data or 'text' not in data:
            return jsonify({'error': 'Missing "text" field'}), 400
        
        plaintext = data['text']
        
        if not plaintext:
            return jsonify({'error': 'Empty text provided'}), 400
        
        # Score the text using English scorer
        result = english_scorer.score_text(plaintext)
        
        return jsonify(result)
    
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/cipher-families', methods=['GET'])
def get_families():
    """Get all supported cipher families and types"""
    return jsonify({
        'families': Config.FAMILIES,
        'total_types': len(Config.get_all_types())
    })

def run_server():
    """Run Flask server"""
    print(f"Starting ML API server on {Config.API_HOST}:{Config.API_PORT}")
    app.run(host=Config.API_HOST, port=Config.API_PORT, debug=False)

if __name__ == '__main__':
    run_server()