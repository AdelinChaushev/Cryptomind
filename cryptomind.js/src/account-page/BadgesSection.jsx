import React from 'react';

const BADGE_ICONS = {
    'Cipher Creator':  '🔐',
    'Helpful Mind':    '💡',
    'First Solve':     '⚡',
    'Speed Runner':    '🏃',
    'Master Solver':   '🏆',
    'Code Breaker':    '🔓',
};

function BadgeItem({ badge }) {
    const icon = BADGE_ICONS[badge.title] || '🎖';

    return (
        <div className="badge-item">
            <div className="badge-icon-wrap">{icon}</div>
            <div className="badge-info">
                <div className="badge-title">{badge.title}</div>
                <div className="badge-desc">{badge.description}</div>
            </div>
            <div className="badge-earned">
                Earned by
                <span>{badge.earnedBy}</span>
                users
            </div>
        </div>
    );
}

function BadgesSection({ badges }) {
    return (
        <div className="section-panel">
            <div className="section-panel-header">
                <span className="section-panel-icon">🎖</span>
                <span className="section-panel-title">Badges ({badges?.length ?? 0})</span>
            </div>
            <div className="section-panel-body">
                {badges && badges.length > 0 ? (
                    <div className="badges-grid">
                        {badges.map((badge, i) => (
                            <BadgeItem key={i} badge={badge} />
                        ))}
                    </div>
                ) : (
                    <div className="no-badges">
                        No badges earned yet. Start solving ciphers!
                    </div>
                )}
            </div>
        </div>
    );
}

export default BadgesSection;
