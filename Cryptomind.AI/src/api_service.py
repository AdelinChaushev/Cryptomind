from flask import Flask, request, jsonify
from flask_cors import CORS
from config.config import Config
from src.predictor import CipherPredictor

app = Flask(__name__)
CORS(app)

# Initialize predictor
predictor = None

@app.before_request
def initialize():
    """Initialize predictor on first request"""
    global predictor
    if predictor is None:
        print("Initializing predictor...")
        predictor = CipherPredictor()

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