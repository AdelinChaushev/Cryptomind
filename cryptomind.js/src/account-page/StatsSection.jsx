import React from 'react';

function StatCard({ value, label, accent, subtext }) {
    return (
        <div className={`stat-card ${accent}`}>
            <div className="stat-value">{value}</div>
            <div className="stat-label">{label}</div>
            {subtext && <div className="stat-subtext">{subtext}</div>}
        </div>
    );
}

function StatsSection({ user }) {
    const successRate = user.successRate != null
        ? `${(user.successRate).toFixed(1)}%`
        : '0%';

    return (
        <div className="section-panel">
            <div className="section-panel-header">
                <span className="section-panel-icon">📊</span>
                <span className="section-panel-title">Performance Stats</span>
            </div>
            <div className="section-panel-body">
                <div className="stats-grid">
                    <StatCard
                        value={user.score ?? 0}
                        label="Total Score"
                        accent="yellow"
                        subtext="points earned"
                    />
                    <StatCard
                        value={user.solvedCount ?? 0}
                        label="Ciphers Solved"
                        accent="emerald"
                    />
                    <StatCard
                        value={user.attemptedCiphers ?? 0}
                        label="Attempted"
                        accent="cyan"
                    />
                    <StatCard
                        value={successRate}
                        label="Success Rate"
                        accent="violet"
                    />
                    <StatCard
                        value={user.badges?.length ?? 0}
                        label="Badges"
                        accent="orange"
                    />
                    <StatCard
                        value={`#${user.leaderBoardPlace ?? '—'}`}
                        label="Global Rank"
                        accent="yellow"
                    />
                </div>
            </div>
        </div>
    );
}

export default StatsSection;
