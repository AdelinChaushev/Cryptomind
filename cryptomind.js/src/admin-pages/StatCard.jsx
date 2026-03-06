import React from 'react';

const StatCard = ({ label, value, sub, accent = '' }) => {
    return (
        <div className={`stat-card${accent ? ` accent-${accent}` : ''}`}>
            <span className="stat-card-label">{label}</span>
            <span className="stat-card-value">{value}</span>
            {sub && <span className="stat-card-sub">{sub}</span>}
        </div>
    );
};

export default StatCard;
