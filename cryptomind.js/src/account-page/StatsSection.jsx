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
                <span className="section-panel-title">Статистика</span>
            </div>
            <div className="section-panel-body">
                <div className="stats-grid">
                    <StatCard
                        value={user.score ?? 0}
                        label="Общ резултат"
                        accent="yellow"
                        subtext="спечелени точки"
                    />
                    <StatCard
                        value={user.solvedCount ?? 0}
                        label="Решени шифри"
                        accent="emerald"
                    />
                    <StatCard
                        value={user.attemptedCiphers ?? 0}
                        label="Опитани"
                        accent="cyan"
                    />
                    <StatCard
                        value={successRate}
                        label="Процент на успех"
                        accent="violet"
                    />
                    <StatCard
                        value={user.badges?.length ?? 0}
                        label="Значки"
                        accent="orange"
                    />
                    <StatCard
                        value={`#${user.leaderBoardPlace ?? '—'}`}
                        label="Глобална класация"
                        accent="yellow"
                    />
                    <StatCard
                        value={`🔥 ${user.currentStreak ?? 0}`}
                        label="Текуща серия"
                        accent="cyan"
                        subtext="поредни дни"
                    />
                    <StatCard
                        value={user.longestStreak ?? 0}
                        label="Най-дълга серия"
                        accent="orange"
                        subtext="дни"
                    />
                </div>
            </div>
        </div>
    );
}

export default StatsSection;