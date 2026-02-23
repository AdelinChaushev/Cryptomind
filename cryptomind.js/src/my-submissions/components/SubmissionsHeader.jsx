import React from 'react';

// stats: { total, approved, pending, rejected }
const SubmissionsHeader = ({ stats = {} }) => {
    const { total = 0, approved = 0, pending = 0, rejected = 0 } = stats;

    return (
        <header className="page-header">
            <nav className="breadcrumb" aria-label="Breadcrumb">
                <a href="/">Home</a>
                <span className="breadcrumb-sep">›</span>
                <span className="breadcrumb-current">My Submissions</span>
            </nav>

            <div className="header-label">
                <span className="label-dot"></span>
                USER DASHBOARD
            </div>

            <h1 className="page-title">My <span>Submissions</span></h1>
            <p className="page-subtitle">
                Track your cipher submissions and answer suggestions across all challenges.
            </p>

            <div className="header-stats">
                <div className="hstat">
                    <span className="hstat-value yellow">{total}</span>
                    <span className="hstat-label">Total</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value emerald">{approved}</span>
                    <span className="hstat-label">Approved</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value">{pending}</span>
                    <span className="hstat-label">Pending</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value rose">{rejected}</span>
                    <span className="hstat-label">Rejected</span>
                </div>
            </div>
        </header>
    );
};

export default SubmissionsHeader;
