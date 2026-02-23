function ExperimentalCipherPanel({ onSubmit }) {
    return (
        <div className="solve-form-panel">
            <h3 className="panel-heading">Изпрати своя отговор</h3>

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
                    Това е <strong style={{ color: "var(--violet-400)" }}>експериментален шифър</strong>.
                    Изпрати разкодирания текст и обяснение — администратор ще прегледа отговора ти.
                </p>
            </div>

            <div style={{ marginBottom: "14px" }}>
                <p style={{ fontFamily: "var(--font-mono)", fontSize: "11px", color: "var(--text-dim)", textTransform: "uppercase", marginBottom: "8px" }}>
                    Разкодиран текст
                </p>
                <textarea
                    className="solve-input"
                    placeholder="Въведи това, което смяташ за декриптирано съобщение..."
                    value={onSubmit.decryptedText}
                    onChange={(e) => onSubmit.onDecryptedTextChange(e.target.value)}
                />
            </div>

            <div style={{ marginBottom: "20px" }}>
                <p style={{ fontFamily: "var(--font-mono)", fontSize: "11px", color: "var(--text-dim)", textTransform: "uppercase", marginBottom: "8px" }}>
                    Обяснение
                </p>
                <textarea
                    className="solve-input"
                    placeholder="Обясни как го реши — какъв е типът на шифъра, ключ, стъпки..."
                    value={onSubmit.description}
                    onChange={(e) => onSubmit.onDescriptionChange(e.target.value)}
                />
            </div>

            <div className="solve-form-footer">
                <p className="attempt-info" style={{ fontSize: "11px" }}>
                    Твоят отговор ще бъде прегледан от администратор
                </p>
                <button
                    className="btn-submit"
                    onClick={onSubmit.onSubmit}
                    disabled={!onSubmit.decryptedText.trim() || !onSubmit.description.trim()}
                >
                    <span>→</span>
                    Изпрати за преглед
                </button>
            </div>
        </div>
    );
}

export default ExperimentalCipherPanel;