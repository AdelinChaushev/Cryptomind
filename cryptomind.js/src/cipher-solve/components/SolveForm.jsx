function SolveForm({ answer, onAnswerChange, onSubmit, attempts, maxAttempts, result }) {
    const attemptsLeft = maxAttempts - attempts;

    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Твоят отговор</h3>

            <div className="solve-input-wrap">
                <textarea
                    className="solve-input"
                    placeholder="Въведи разкодирания текст тук..."
                    value={answer}
                    onChange={(e) => onAnswerChange(e.target.value)}
                    disabled={result === "correct"}
                />
            </div>

            <div className="solve-form-footer">
                <button
                    className="btn-submit"
                    onClick={onSubmit}
                    disabled={!answer.trim() || result === "correct" || attemptsLeft <= 0}
                >
                    <span>→</span>
                    Изпрати отговор
                </button>
            </div>

            {result === "correct" && (
                <div className="answer-result correct">
                    <span className="result-icon">✓</span>
                    Вярно! Поздравления.
                </div>
            )}
            {result === "incorrect" && (
                <div className="answer-result incorrect">
                    <span className="result-icon">✗</span>
                    Грешно — опитай пак.
                </div>
            )}
        </div>
    );
}

export default SolveForm;
