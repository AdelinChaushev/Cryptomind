import React from 'react';

const SubmissionsPageHeader = () => {
    return (
        <div className="page-header">
            <nav className="breadcrumb">
                <a href="/">Начало</a>
                <span className="breadcrumb-sep">/</span>
                <span className="breadcrumb-current">Моите предложения</span>
            </nav>

            <h1 className="page-title">
                Моите <span>Предложения</span>
            </h1>
            <p className="page-subtitle">
                Следете вашите предложени шифри и отговори
            </p>
        </div>
    );
};

export default SubmissionsPageHeader;