import React from 'react';

const LlmAssistantSection = ({
    result,
    isLoading,
    onRunAnalysis
}) => {
    const getConfidenceBadge = (confidence) => {
        const map = {
            high: 'badge-approved',
            medium: 'badge-pending',
            low: 'badge-rejected'
        };
        return map[confidence?.toLowerCase()] || 'badge-standard';
    };

    return (
        <div className="admin-card">
            <div className="admin-card-header">
                <span className="admin-card-title">LLM Assistant</span>
                {result && <span className="badge badge-approved">Analysis Complete</span>}
            </div>

            {/* Run Button */}
            {!result && (
                <div>
                    <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6', marginBottom: '12px' }}>
                        Run LLM analysis to verify the cipher type, check solution correctness, and get a recommendation.
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
                                Analyzing...
                            </>
                        ) : (
                            <>
                                <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="8" cy="8" r="6.5"/>
                                    <path d="M5 8l2 2 4-4"/>
                                </svg>
                                Run LLM Analysis
                            </>
                        )}
                    </button>
                </div>
            )}

            {/* LLM Result */}
            {result && (
                <div className="llm-result">
                    {/* Recommendation Banner */}
                    <div className={`llm-recommendation-banner recommendation-${result.solutionCorrect && result.isAppropriate? "approve" : "reject"}`}>
                        <div className="recommendation-icon">
                            {result.solutionCorrect && result.isAppropriate ? '✓' : 
                              '✕'}
                        </div>
                        <div>
                            <div className="recommendation-title">
                                Recommendation: {result.solutionCorrect && result.isAppropriate ? 'Approve' : 'Reject'}
                            </div>
                            <div className="recommendation-subtitle">
                                {result.reasoning}
                            </div>
                        </div>
                    </div>

                    {/* Analysis Grid */}
                    <div className="llm-analysis-grid">
                        <div className="llm-result-block">
                            <span className="llm-result-label">Predicted Type</span>
                            <div className="llm-type-row">
                                <span className="badge badge-standard">{result.predictedType || '—'}</span>
                            </div>
                        </div>

                        <div className="llm-result-block">
                            <span className="llm-result-label">Confidence</span>
                            <span className={`badge ${getConfidenceBadge(result.confidence)}`}>
                                {result.confidence?.toUpperCase() || '—'}
                            </span>
                        </div>

                        <div className="llm-result-block">
                            <span className="llm-result-label">Solution Correct</span>
                            <span className={`badge ${result.solutionCorrect ? 'badge-approved' : 'badge-rejected'}`}>
                                {result.solutionCorrect ? 'Yes' : 'No'}
                            </span>
                        </div>

                        <div className="llm-result-block">
                            <span className="llm-result-label">Content Appropriate</span>
                            <span className={`badge ${result.isAppropriate ? 'badge-approved' : 'badge-rejected'}`}>
                                {result.isAppropriate ? 'Yes' : 'No'}
                            </span>
                        </div>

                        {result.isSolvable !== null && result.isSolvable !== undefined && (
                            <div className="llm-result-block">
                                <span className="llm-result-label">Solvable</span>
                                <span className={`badge ${result.isSolvable ? 'badge-approved' : 'badge-rejected'}`}>
                                    {result.isSolvable ? 'Yes' : 'No'}
                                </span>
                            </div>
                        )}
                    </div>

                    {/* Issues */}
                    {result.issues && result.issues.length > 0 && (
                        <div className="llm-result-block">
                            <span className="llm-result-label">Issues Found</span>
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
{/* 
                    {/* Re-run button 
                    <button
                        onClick={onRunAnalysis}
                        className="btn btn-ghost btn-sm"
                        style={{ marginTop: '8px' }}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Re-analyzing...' : 'Re-run Analysis'}
                    </button> */}
                </div>
            )}
        </div>
    );
};

export default LlmAssistantSection;