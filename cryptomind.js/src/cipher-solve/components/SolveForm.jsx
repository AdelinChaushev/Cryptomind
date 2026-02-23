function SolveForm({ answer, onAnswerChange, onSubmit, attempts, maxAttempts, result }) {
    const attemptsLeft = maxAttempts - attempts;

    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Your Answer</h3>

            <div className="solve-input-wrap">
                <textarea
                    className="solve-input"
                    placeholder="Type the decrypted plaintext here..."
                    value={answer}
                    onChange={(e) => onAnswerChange(e.target.value)}
                    disabled={result === "correct"}
                />
            </div>

            <div className="solve-form-footer">
                {/* <p className="attempt-info">
                    <span>{attemptsLeft}</span> attempt{attemptsLeft !== 1 ? "s" : ""} remaining
                </p> */}
                <button
                    className="btn-submit"
                    onClick={onSubmit}
                    disabled={!answer.trim() || result === "correct" || attemptsLeft <= 0}
                >
                    <span>→</span>
                    Submit Answer
                </button>
            </div>

            {result === "correct" && (
                <div className="answer-result correct">
                    <span className="result-icon">✓</span>
                    Correct! Well done.
                </div>
            )}
            {result === "incorrect" && (
                <div className="answer-result incorrect">
                    <span className="result-icon">✗</span>
                    Incorrect — try again.
                </div>
            )}
        </div>
    );
}

export default SolveForm;
