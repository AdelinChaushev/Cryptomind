import { useState } from 'react';
import '../styles/experimental-cipher-panel.css';

function ExperimentalCipherPanel({ onSubmit }) {
    const [submitted, setSubmitted] = useState(false);

    const handleSubmit = () => {
        onSubmit.onSubmit();
        setSubmitted(true);
    };

    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Изпрати своя отговор</h3>

            <div className="experimental-notice">
                <span className="experimental-notice__icon">⚗</span>
                <p className="experimental-notice__text">
                    This is an <strong className="experimental-notice__highlight">experimental cipher</strong>.
                    Submit your decrypted text and explanation — an admin will review your answer.
                </p>
            </div>

            <div className="experimental-field">
                <p className="experimental-field__label">Decrypted Text</p>
                <textarea
                    className="solve-input"
                    placeholder="Въведи това, което смяташ за декриптирано съобщение..."
                    value={onSubmit.decryptedText}
                    onChange={(e) => onSubmit.onDecryptedTextChange(e.target.value)}
                />
            </div>

            <div className="experimental-field">
                <p className="experimental-field__label">Explanation</p>
                <textarea
                    className="solve-input"
                    placeholder="Обясни как го реши — какъв е типът на шифъра, ключ, стъпки..."
                    value={onSubmit.description}
                    onChange={(e) => onSubmit.onDescriptionChange(e.target.value)}
                />
            </div>

            {submitted && (
                <p className="experimental-success">
                    ✓ Answer submitted. An admin will review your solution.
                </p>
            )}

            <div className="solve-form-footer">
                <p className="attempt-info" style={{ fontSize: "11px" }}>
                    Твоят отговор ще бъде прегледан от администратор
                </p>
                <button
                    className="btn-submit"
                    onClick={handleSubmit}
                    // disabled={!onSubmit.decryptedText.trim() || !onSubmit.description.trim() || submitted}
                >
                    <span>→</span>
                    {submitted ? 'Submitted' : 'Submit for Review'}
                </button>
            </div>
        </div>
    );
}

export default ExperimentalCipherPanel;