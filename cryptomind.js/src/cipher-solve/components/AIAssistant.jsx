import AIResponse from "./AIResponse";

function AIAssistant({ onTypeHint, onSolutionHint, onSolution, aiMode, aiText, aiLoading , allowSolution,allowTypeHint,allowSolutionHint})
 {
    return (
        <div className="ai-assistant-panel">
            <h3 className="panel-heading">AI Assistance</h3>
            
            <div className="ai-buttons">
             {allowTypeHint &&  <button
                    className="btn-ai btn-type-hint"
                    onClick={onTypeHint}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        <span className="btn-ai-icon">🔍</span>
                        Type Hint
                    </span>
                    <span className="btn-ai-arrow">→</span>
                </button>}

               {allowSolutionHint && <button
                    className="btn-ai btn-solution-hint"
                    onClick={onSolutionHint}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        
                        Solution Hint
                    </span>
                    <span className="btn-ai-arrow">→</span>
                </button>}

               {allowSolution && <button
                    className="btn-ai btn-solution"
                    onClick={onSolution}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        
                        Reveal Solution
                    </span>
                    <span className="btn-ai-arrow">→</span>
                </button>}
            </div>

            <div className="penalty-warning">
              
                <p className="warning-text">
                    Using AI assistance will <strong>reduce points earned</strong> for this cipher.
                    Type hints cost the least, solution hints cost more, and a full solution costs the most.
                </p>
            </div>

            <AIResponse
                mode={aiMode}
                text={aiText}
                isLoading={aiLoading}
            />
        </div>
    );
}

export default AIAssistant;