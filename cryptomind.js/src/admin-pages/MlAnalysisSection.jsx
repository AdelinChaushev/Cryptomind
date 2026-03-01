import React from 'react';



const MlAnalysisSection = ({
    mlFamily,
    mlPrediction,
    mlConfidence,
    llmRecommended,
    userProvidedType,
    typeMismatch
}) => {
    const getConfColor = (pct) => {
        if (pct >= 85) return 'var(--emerald-500)';
        if (pct >= 65) return 'var(--yellow-500)';
        return 'var(--rose-500)';
    };

    const confColor = mlConfidence !== null ? getConfColor(mlConfidence) : null;

    return (
        <div className="admin-card review-card">
            <div className="admin-card-header">
                <div className="review-card-title-row">
                    <span className="review-section-tag">ML</span>
                    <span className="admin-card-title">ML Analysis</span>
                </div>
                {llmRecommended !== undefined && (
                    <span className={`badge ${llmRecommended ? 'badge-pending' : 'badge-approved'}`}>
                        {llmRecommended ? 'LLM Review Required' : 'LLM Optional'}
                    </span>
                )}
            </div>

            {mlPrediction ? (
                <div className="ml-result-grid">
                  
                    <div className="ml-metric">
                        <span className="ml-metric-label">Cipher Family</span>
                        <span className="ml-metric-value">{mlFamily ?? '—'}</span>
                    </div>

                  
                    <div className="ml-metric">
                        <span className="ml-metric-label">Predicted Type</span>
                        <span className="ml-metric-value text-yellow">{mlPrediction}</span>
                    </div>

                   
                    <div className="ml-metric ml-metric-wide">
                        <span className="ml-metric-label">Confidence</span>
                        <div className="ml-confidence-row">
                            <div className="ml-confidence-bar">
                                <div
                                    className="ml-confidence-fill"
                                    style={{
                                        width: `${mlConfidence}%`,
                                        background: confColor
                                    }}
                                />
                            </div>
                            <span className="ml-confidence-pct" style={{ color: confColor }}>
                                {mlConfidence}%
                            </span>
                        </div>
                    </div>

                   
                    <div className="ml-metric">
                        <span className="ml-metric-label">Layer 1 (Family)</span>
                        <span className="ml-metric-value text-xs text-dim">99.97% accuracy</span>
                    </div>
                </div>
            ) : (
                <div className="empty-state" style={{ padding: '30px' }}>
                    <span className="empty-state-title">ML prediction not available</span>
                    <span className="empty-state-text">Text may be too short (&lt;150 chars) or classification failed</span>
                </div>
            )}

            
            {typeMismatch && userProvidedType && mlPrediction && (
                <div className="admin-notice notice-warning" style={{ marginTop: '14px' }}>
                    <svg className="admin-notice-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                        <path d="M8 1L15 14H1L8 1z"/><path d="M8 6v4M8 11.5h.01"/>
                    </svg>
                    <span>
                        User claimed <strong>{userProvidedType}</strong> but ML predicts <strong>{mlPrediction}</strong>.
                        LLM verification is recommended.
                    </span>
                </div>
            )}

            {(mlPrediction === 'Columnar' || mlPrediction === 'RailFence') && (
                <div className="admin-notice notice-info" style={{ marginTop: '14px' }}>
                    <svg className="admin-notice-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                        <circle cx="8" cy="8" r="6.5"/><path d="M8 7v4M8 5h.01"/>
                    </svg>
                    <span>
                        Note: Columnar and RailFence transposition ciphers share near-identical statistical signatures.
                        Columnar is often misclassified as RailFence (81% wrong confidence). LLM review is strongly advised.
                    </span>
                </div>
            )}
        </div>
    );
};

export default MlAnalysisSection;
