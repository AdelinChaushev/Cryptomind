import React, { useState, useEffect } from 'react';

const useCountUp = (target, duration = 900, delay = 0) => {
    const [value, setValue] = useState(0);

    useEffect(() => {
        let startId;
        let rafId;

        startId = setTimeout(() => {
            if (target === 0) { setValue(0); return; }
            const start = performance.now();

            const update = (now) => {
                const elapsed = now - start;
                const progress = Math.min(elapsed / duration, 1);
                const eased = 1 - Math.pow(1 - progress, 3);
                setValue(Math.round(eased * target));
                if (progress < 1) rafId = requestAnimationFrame(update);
            };

            rafId = requestAnimationFrame(update);
        }, delay);

        return () => {
            clearTimeout(startId);
            cancelAnimationFrame(rafId);
        };
    }, [target, duration, delay]);

    return value;
};

const SubmissionsStatsStrip = ({ totalSubmissions = 0, approved = 0, pending = 0, rejected = 0, deleted = 0 }) => {
    const animTotal    = useCountUp(totalSubmissions, 800, 200);
    const animApproved = useCountUp(approved,         750, 280);
    const animPending  = useCountUp(pending,          750, 340);
    const animRejected = useCountUp(rejected,         750, 400);
    const animDeleted  = useCountUp(deleted,          750, 460);

    return (
        <div className="stats-strip">
            <div className="stat-chip">
                <span className="stat-chip-label">Общо</span>
                <div className="stat-divider" />
                <span className="stat-chip-value">{animTotal}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Одобрени</span>
                <div className="stat-divider" />
                <span className="stat-chip-value emerald">{animApproved}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Изчакващи</span>
                <div className="stat-divider" />
                <span className="stat-chip-value yellow">{animPending}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Отхвърлени</span>
                <div className="stat-divider" />
                <span className="stat-chip-value rose">{animRejected}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Изтрити</span>
                <div className="stat-divider" />
                <span className="stat-chip-value grey">{animDeleted}</span>
            </div>
        </div>
    );
};

export default SubmissionsStatsStrip;
