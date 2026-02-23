import React from 'react';

const PageHeader = () => {
    return (
        <header className="page-header">
            <nav className="breadcrumb">
                <a href="/">Home</a>
                <span className="breadcrumb-sep">›</span>
                <span>Submit Cipher</span>
            </nav>
            <h1 className="page-title">Submit a <span>Cipher</span></h1>
            <p className="page-subtitle">Contribute a challenge for the community to solve.</p>
        </header>
    );
};

export default PageHeader;
