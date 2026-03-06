import React, { useState, useRef } from 'react';

function BadgeItem({ badge }) {
    const [tooltipStyle, setTooltipStyle] = useState({});
    const wrapRef = useRef(null);

    const handleMouseEnter = () => {
        const rect = wrapRef.current.getBoundingClientRect();
        setTooltipStyle({
            left: rect.left + rect.width / 2 - 90,
            top: rect.top - 12,
            transform: 'translateY(-100%)',
        });
    };

    return (
        <div className="badge-item" onMouseEnter={handleMouseEnter} ref={wrapRef}>
            <div className="badge-icon-wrap">
                <img src={badge.badgeImage} alt={badge.title} />
            </div>
            <div className="badge-tooltip" style={tooltipStyle}>
                <div className="badge-tooltip-title">{badge.title}</div>
                <div className="badge-tooltip-desc">{badge.description}</div>
                <div className="badge-tooltip-earned">
                    Спечелена от <span>{badge.earnedBy}</span> потребители
                </div>
            </div>
        </div>
    );
}

function BadgesSection({ badges }) {
    return (
        <div className="section-panel">
            <div className="section-panel-header">
                <span className="section-panel-title">Значки ({badges?.length ?? 0})</span>
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
                        Все още няма спечелени значки. Започнете да решавате шифри!
                    </div>
                )}
            </div>
        </div>
    );
}

export default BadgesSection;