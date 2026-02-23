function CipherMeta({ cipher , timeAgo }) {
    const successPercent = cipher.successRate || 0;

    return (
        <div className="cipher-meta-panel">
            <div className="meta-header">
                <p className="meta-header-title">Cipher Info</p>
            </div>

            <div className="meta-body">
                <div className="points-display">
                    <div className="points-value">{cipher.points ?? 100}</div>
                    <div className="points-label">Points Available</div>
                </div>

                <div className="meta-stats">
                    

                    <div className="meta-stat">
                        <span className="meta-stat-label">
                            <span className="meta-stat-icon">✅</span>
                            Times Solved
                        </span>
                        <span className="meta-stat-value emerald">{cipher.successfulSubmissions ?? 0}</span>
                    </div>

                    <div className="meta-stat">
                        <span className="meta-stat-label">
                            <span className="meta-stat-icon">📊</span>
                            Success Rate
                        </span>
                        <span className="meta-stat-value yellow">{cipher.successRate}%</span>
                    </div>

                    <div className="meta-stat">
                        <span className="meta-stat-label">
                            <span className="meta-stat-icon">📅</span>
                            Added
                        </span>
                        <span className="meta-stat-value cyan">{timeAgo(cipher.dateSubmitted)}</span>
                    </div>
                </div>

                <div className="solve-cta">
                    <div className="solve-progress-label">
                        <span className="progress-text">Solve rate</span>
                        <span className="progress-count">{cipher.successfulSubmissions ?? 0} / {cipher.totalAttempts ?? 0}</span>
                    </div>
                    <div className="progress-bar">
                        <div
                            className="progress-fill"
                            style={{ width: `${cipher.successRate}%` }}
                        ></div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CipherMeta;
