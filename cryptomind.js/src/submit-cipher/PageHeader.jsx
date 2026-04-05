import React from 'react';

const PageHeader = () => {
    return (
        <header className="page-header">
            <nav className="breadcrumb">
                <a href="/">Начало</a>
                <span className="breadcrumb-sep">›</span>
                <span>Предложи шифър</span>
            </nav>
            <div className="page-eyebrow">
                <span className="page-eyebrow-text">Допринесете за общността</span>
            </div>
            <h1 className="page-title">Предложи <span>Шифър</span></h1>
            <p className="page-subtitle">Допринесете с предизвикателство за общността.</p>
            <div className="page-header-divider" />
        </header>
    );
};

export default PageHeader;