import React from 'react';

/* Props:
   totalSubmissions  : number
   approved          : number
   pending           : number
   rejected          : number
*/
const SubmissionsStatsStrip = ({ totalSubmissions = 0, approved = 0, pending = 0, rejected = 0 , deleted = 0 }) => {
    return (
        <div className="stats-strip">
            <div className="stat-chip">
                <span className="stat-chip-label">Total</span>
                <div className="stat-divider" />
                <span className="stat-chip-value">{totalSubmissions}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Approved</span>
                <div className="stat-divider" />
                <span className="stat-chip-value emerald">{approved}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Pending</span>
                <div className="stat-divider" />
                <span className="stat-chip-value yellow">{pending}</span>
            </div>

            <div className="stat-chip">
                <span className="stat-chip-label">Rejected</span>
                <div className="stat-divider" />
                <span className="stat-chip-value rose">{rejected}</span>
            </div>
             <div className="stat-chip">
                <span className="stat-chip-label">Deleted</span>
                <div className="stat-divider" />
                <span className="stat-chip-value grey">{deleted}</span>
            </div>
        </div>
    );
};

export default SubmissionsStatsStrip;
