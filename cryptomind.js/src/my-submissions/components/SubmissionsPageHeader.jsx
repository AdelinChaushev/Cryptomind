import React from 'react';

const SubmissionsPageHeader = () => {
    return (
        <div className="page-header">
            <nav className="breadcrumb">
                <a href="/">Home</a>
                <span className="breadcrumb-sep">/</span>
                <span className="breadcrumb-current">My Submissions</span>
            </nav>

            <h1 className="page-title">
                My <span>Submissions</span>
            </h1>
            <p className="page-subtitle">
                Track your cipher submissions and answer suggestions
            </p>
        </div>
    );
};

export default SubmissionsPageHeader;
