import React from 'react';

const LlmAssistantSection = ({
    result,
    isLoading,
    onRunAnalysis
}) => {
    const getConfidenceBadge = (confidence) => {
        const map = {
            'висока': 'badge-approved',
            'средна': 'badge-pending',
            'ниска': 'badge-rejected'
        };
        return map[confidence?.toLowerCase()] || 'badge-standard';
    };

    const isApproved = result?.recommendation === 'approve';

    return (
        <div className="admin-card">
            <div className="admin-card-header">
                <span className="admin-card-title">LLM Асистент</span>
                {result && <span className="badge badge-approved">Анализът е завършен</span>}
            </div>

            
            {!result && (
                <div>
                    <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6', marginBottom: '12px' }}>
                        Стартирайте LLM анализ, за да проверите вида на шифъра, верността на решението и да получите препоръка.
                    </p>

                    <button
                        onClick={onRunAnalysis}
                        className="btn btn-primary"
                        disabled={isLoading}
                        style={{ width: '100%', justifyContent: 'center' }}
                    >
                        {isLoading ? (
                            <>
                                <span className="spinner" />
                                Анализиране...
                            </>
                        ) : (
                            <>
                                <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="8" cy="8" r="6.5"/>
                                    <path d="M5 8l2 2 4-4"/>
                                </svg>
                                Стартирай LLM анализ
                            </>
                        )}
                    </button>
                </div>
            )}

            
            {result && (
                <div className="llm-result">
                    
                    <div className={`llm-recommendation-banner recommendation-${isApproved ? 'approve' : 'reject'}`}>
                        <div className="recommendation-icon">
                            {isApproved ? '✓' : '✕'}
                        </div>
                        <div>
                            <div className="recommendation-title">
                                Препоръка: {isApproved ? 'Одобри' : 'Отхвърли'}
                            </div>
                            <div className="recommendation-subtitle">
                                {result.reasoning}
                            </div>
                        </div>
                    </div>

                    
                    <div className="llm-analysis-grid">
                        <div className="llm-result-block">
                            <span className="llm-result-label">Предвиден вид</span>
                            <div className="llm-type-row">
                                <span className="badge badge-standard">{result.predictedType || '—'}</span>
                            </div>
                        </div>

                        <div className="llm-result-block">
                            <span className="llm-result-label">Увереност</span>
                            <span className={`badge ${getConfidenceBadge(result.confidence)}`}>
                                {result.confidence?.toUpperCase() || '—'}
                            </span>
                        </div>

                        {result.solutionCorrect !== null && result.solutionCorrect !== undefined && (
                            <div className="llm-result-block">
                                <span className="llm-result-label">Решението е вярно</span>
                                <span className={`badge ${result.solutionCorrect ? 'badge-approved' : 'badge-rejected'}`}>
                                    {result.solutionCorrect ? 'Да' : 'Не'}
                                </span>
                            </div>
                        )}

                        <div className="llm-result-block">
                            <span className="llm-result-label">Съдържанието е подходящо</span>
                            <span className={`badge ${result.isAppropriate ? 'badge-approved' : 'badge-rejected'}`}>
                                {result.isAppropriate ? 'Да' : 'Не'}
                            </span>
                        </div>

                        {result.isSolvable !== null && result.isSolvable !== undefined && (
                            <div className="llm-result-block">
                                <span className="llm-result-label">Решаем</span>
                                <span className={`badge ${result.isSolvable ? 'badge-approved' : 'badge-rejected'}`}>
                                    {result.isSolvable ? 'Да' : 'Не'}
                                </span>
                            </div>
                        )}
                    </div>

                   
                    {result.issues && result.issues.length > 0 && (
                        <div className="llm-result-block">
                            <span className="llm-result-label">Открити проблеми</span>
                            <div className="llm-issues-list">
                                {result.issues.map((issue, index) => (
                                    <div key={index} className="llm-issue-item">
                                        <span className="issue-bullet">•</span>
                                        <span>{issue}</span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default LlmAssistantSection;