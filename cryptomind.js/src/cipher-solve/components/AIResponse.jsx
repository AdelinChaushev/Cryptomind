function AIResponse({ mode, text, isLoading }) {
    if (mode == -1 && !isLoading) return null;
    const hintType = mode === 0 ? "typeHint" : mode === 1 ? "solutionHint" : mode === 2 ? "solution" : null;
    const modeConfig = {
        typeHint: { label: "ТИП ПОДСКАЗКА", icon: "🔍" },
        solutionHint: { label: "ПОДСКАЗКА ЗА РЕШЕНИЕ", icon: "💡" },
        solution: { label: "ПЪЛНО РЕШЕНИЕ", icon: "🔓" },
    };

    const config = modeConfig[hintType] ?? { label: "AI ОТГОВОР", icon: "🤖" };

    return (
        <div className="ai-response">
            <div className="ai-response-header">
                <div className="ai-avatar">{config.icon}</div>
                <span className="ai-label">Cryptomind AI — {config.label}</span>
            </div>

            <div className="ai-response-body">
                {isLoading ? (
                    <div className="ai-loading">
                        <span className="ai-loading-text">Анализиране на шифъра</span>
                        <div className="ai-loading-bar-track">
                            <div className="ai-loading-bar-fill"></div>
                        </div>
                    </div>
                ) : (
                    <p>{text}</p>
                )}
            </div>
        </div>
    );
}

export default AIResponse;