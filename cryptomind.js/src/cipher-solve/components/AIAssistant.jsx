import AIResponse from "./AIResponse";

function AIAssistant({ onTypeHint, onSolutionHint, onSolution, aiMode, aiText, aiLoading , allowSolution,allowTypeHint,allowSolutionHint})
{
    return (
        <div className="ai-assistant-panel">
            <h3 className="panel-heading">AI Асистент</h3>
            
            <div className="ai-buttons">
             {allowTypeHint &&  <button
                    className="btn-ai btn-type-hint"
                    onClick={onTypeHint}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        
                        Тип подсказка
                    </span>
                    <span className="btn-ai-right">
                        <span className="btn-ai-penalty">-30%</span>
                        <span className="btn-ai-arrow">→</span>
                    </span>
                </button>}

               {allowSolutionHint && <button
                    className="btn-ai btn-solution-hint"
                    onClick={onSolutionHint}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        Подсказка за решение
                    </span>
                    <span className="btn-ai-right">
                        <span className="btn-ai-penalty">-50%</span>
                        <span className="btn-ai-arrow">→</span>
                    </span>
                </button>}

               {allowSolution && <button
                    className="btn-ai btn-solution"
                    onClick={onSolution}
                    disabled={aiLoading}>
                    <span className="btn-ai-left">
                        Разкрий решението
                    </span>
                    <span className="btn-ai-right">
                        <span className="btn-ai-penalty">-95%</span>
                        <span className="btn-ai-arrow">→</span>
                    </span>
                </button>}
            </div>

            <div className="penalty-warning">
                <p className="warning-text">
                    Използването на AI асистент ще <strong>намали спечелените точки</strong> за този шифър.
                    Подсказките за типа струват най-малко, тези за решението повече, а пълното решение – най-много.
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