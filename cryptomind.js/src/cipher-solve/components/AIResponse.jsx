function AIResponse({ mode, text, isLoading }) {
    if (mode == -1 && !isLoading) return null;
    const hintType = mode === 0 ? "typeHint" : mode === 1 ? "solutionHint" : mode === 2 ? "solution" : null;
    const modeConfig = {
        typeHint:     { label: "TYPE HINT",     icon: "🔍" },
        solutionHint: { label: "SOLUTION HINT", icon: "💡" },
        solution:     { label: "FULL SOLUTION", icon: "🔓" },
    };

    const config = modeConfig[hintType] ?? { label: "AI RESPONSE", icon: "🤖" };
    console.log("AIResponse config:", config);
    return (
        <div className="ai-response">
            <div className="ai-response-header">
                <div className="ai-avatar">{config.icon}</div>
                <span className="ai-label">CryptoMind AI — {config.label}</span>
            </div>

            <div className="ai-response-body">
                {isLoading ? (
                    <div className="ai-loading">
                        <div className="loading-dots">
                            <div className="loading-dot"></div>
                            <div className="loading-dot"></div>
                            <div className="loading-dot"></div>
                        </div>
                        Analyzing cipher...
                    </div>
                ) : (
                    <p>{text}</p>
                )}
            </div>
        </div>
    );
}

export default AIResponse;