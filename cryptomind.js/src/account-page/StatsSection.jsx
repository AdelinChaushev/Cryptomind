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

function StreakPanel({ currentStreak, longestStreak }) {
    const hasStreak = currentStreak > 0;
    const progress = longestStreak > 0 ? Math.min(100, (currentStreak / longestStreak) * 100) : 0;
    const isRecord = currentStreak > 0 && currentStreak >= longestStreak;

    return (
        <div className={`streak-panel ${hasStreak ? 'streak-fire' : 'streak-ice'}`}>
            <div className="streak-panel-head">
                <span className="streak-panel-tag">[ СЕРИЯ ]</span>
                <span className="streak-record-pill">
                    <span>{longestStreak}</span> рекорд
                </span>
            </div>

            <div className="streak-panel-body">
                <div className="streak-big">{currentStreak}</div>
                <div className="streak-big-sub">поредни дни</div>
            </div>

            <div className="streak-panel-foot">
                <div className="streak-bar-track">
                    <div className="streak-bar-fill" style={{ width: `${progress}%` }} />
                </div>
                <div className={`streak-foot-label ${isRecord ? 'is-record' : ''}`}>
                    {isRecord
                        ? '↑ НОВ РЕКОРД'
                        : longestStreak > 0
                            ? `${currentStreak} / ${longestStreak} до рекорда`
                            : 'Реши дневното предизвикателство и започни серия'}
                </div>
            </div>
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
                    <StreakPanel
                        currentStreak={user.currentStreak ?? 0}
                        longestStreak={user.longestStreak ?? 0}
                    />
                </div>
            </div>
        </div>
    );
}

export default StatsSection;