function ExperimentalCipherPanel({ onSubmit }) {
    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Submit Your Answer</h3>

            <div style={{
                padding: "14px 16px",
                background: "var(--violet-dim)",
                border: "1px solid rgba(167, 139, 250, 0.3)",
                borderRadius: "8px",
                marginBottom: "20px",
                display: "flex",
                gap: "10px",
                alignItems: "flex-start"
            }}>
                <span style={{ color: "var(--violet-400)", flexShrink: 0 }}>⚗</span>
                <p style={{ fontSize: "12px", color: "var(--text-tertiary)", lineHeight: "1.6", fontFamily: "var(--font-mono)" }}>
                    This is an <strong style={{ color: "var(--violet-400)" }}>experimental cipher</strong>.
                    Submit your decrypted text and explanation — an admin will review your answer.
                </p>
            </div>

            <div style={{ marginBottom: "14px" }}>
                <p style={{
                    fontFamily: "var(--font-mono)",
                    fontSize: "11px",
                    letterSpacing: "0.1em",
                    color: "var(--text-dim)",
                    textTransform: "uppercase",
                    marginBottom: "8px"
                }}>
                    Decrypted Text
                </p>
                <textarea
                    className="solve-input"
                    placeholder="Enter what you think the decrypted message is..."
                    value={onSubmit.decryptedText}
                    onChange={(e) => onSubmit.onDecryptedTextChange(e.target.value)}
                />
            </div>

            <div style={{ marginBottom: "20px" }}>
                <p style={{
                    fontFamily: "var(--font-mono)",
                    fontSize: "11px",
                    letterSpacing: "0.1em",
                    color: "var(--text-dim)",
                    textTransform: "uppercase",
                    marginBottom: "8px"
                }}>
                    Explanation
                </p>
                <textarea
                    className="solve-input"
                    placeholder="Explain how you solved it — what cipher type, what key, what steps..."
                    value={onSubmit.description}
                    onChange={(e) => onSubmit.onDescriptionChange(e.target.value)}
                />
            </div>

            <div className="solve-form-footer">
                <p className="attempt-info" style={{ fontSize: "11px" }}>
                    Your submission will be reviewed by an admin
                </p>
                <button
                    className="btn-submit"
                    onClick={onSubmit.onSubmit}
                    disabled={!onSubmit.decryptedText.trim() || !onSubmit.description.trim()}
                >
                    <span>→</span>
                    Submit for Review
                </button>
            </div>
        </div>
    );
}

export default ExperimentalCipherPanel;