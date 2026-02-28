import { useState } from 'react';
import '../styles/experimental-cipher-panel.css';

function ExperimentalCipherPanel({ onSubmit }) {
    const [submitted, setSubmitted] = useState(false);

    const handleSubmit = async () => {
       const isSuccessful = await onSubmit.onSubmit(); 
    
   
    if (isSuccessful) {
        setSubmitted(true);
        setTimeout(() => setSubmitted(false), 4000);
    }
    };

    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Изпрати своя отговор</h3>

            <div className="experimental-notice">
                <span className="experimental-notice__icon">⚗</span>
                <p className="experimental-notice__text">
                    Това е <strong className="experimental-notice__highlight">експериментален шифър</strong>.
                    Изпратете декриптирания текст и обяснение — администратор ще прегледа вашия отговор.
                </p>
            </div>

            <div className="experimental-field">
                <p className="experimental-field__label">Декриптиран текст</p>
                <textarea
                    className="solve-input"
                    placeholder="Въведи това, което смяташ за декриптирано съобщение..."
                    value={onSubmit.decryptedText}
                    onChange={(e) => onSubmit.onDecryptedTextChange(e.target.value)}
                />
            </div>

            <div className="experimental-field">
                <p className="experimental-field__label">Обяснение</p>
                <textarea
                    className="solve-input"
                    placeholder="Обясни как го реши — какъв е типът на шифъра, ключ, стъпки..."
                    value={onSubmit.description}
                    onChange={(e) => onSubmit.onDescriptionChange(e.target.value)}
                />
            </div>

            {submitted && (
                <p className="experimental-success">
                    ✓ Отговорът е изпратен. Администратор ще прегледа вашето решение.
                </p>
            )}

            <div className="solve-form-footer">
                <p className="attempt-info" style={{ fontSize: "11px" }}>
                    Твоят отговор ще бъде прегледан от администратор
                </p>
                <button
                    className="btn-submit"
                    onClick={handleSubmit}
                >
                    <span>→</span>
                    {submitted ? 'Изпратено' : 'Изпрати за преглед'}
                </button>
            </div>
        </div>
    );
}

export default ExperimentalCipherPanel;