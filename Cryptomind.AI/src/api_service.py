from flask import Flask, request, jsonify
from flask_cors import CORS
from config.config import Config
from src.predictor import CipherPredictor
from src.english_scorer import EnglishScorer

app = Flask(__name__)
CORS(app)

predictor = None
english_scorer = None

@app.before_request
def initialize():
    global predictor, english_scorer
    if predictor is None:
        predictor = CipherPredictor()
    if english_scorer is None:
        english_scorer = EnglishScorer()

@app.route('/api/health', methods=['GET'])
def health():
    return jsonify({'status': 'healthy', 'service': 'Cryptomind ML'})

@app.route('/api/predict', methods=['POST'])
def predict():
    try:
        data = request.get_json()

        if not data or 'text' not in data:
            return jsonify({'error': 'Missing "text" field'}), 400

        ciphertext = data['text']

        if not ciphertext or len(ciphertext.strip()) == 0:
            return jsonify({'error': 'Empty text provided'}), 400

        return_top_k = data.get('return_top_k', True)
        result = predictor.predict(ciphertext, return_top_k=return_top_k)

        return jsonify(result)

    except Exception as e:
        import traceback
        traceback.print_exc()
        return jsonify({'error': str(e)}), 500

@app.route('/api/validate-english', methods=['POST'])
def validate_english():
    try:
        data = request.get_json()

        if not data or 'text' not in data:
            return jsonify({'error': 'Missing "text" field'}), 400

        plaintext = data['text']

        if not plaintext:
            return jsonify({'error': 'Empty text provided'}), 400

        result = english_scorer.score_text(plaintext)

        return jsonify(result)

    except Exception as e:
        import traceback
        traceback.print_exc()
        return jsonify({'error': str(e)}), 500

@app.route('/api/cipher-families', methods=['GET'])
def get_families():
    return jsonify({
        'families': Config.FAMILIES,
        'total_types': len(Config.get_all_types())
    })

def run_server():
    app.run(host=Config.API_HOST, port=Config.API_PORT, debug=False)

if __name__ == '__main__':
    run_server()