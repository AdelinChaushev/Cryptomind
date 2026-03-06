import React from 'react';

const SubmissionsTabs = ({ activeTab, onTabChange, cipherCount = 0, answerCount = 0 }) => {
    return (
        <div className="tabs-nav">
            <button
                className={`tab-btn ${activeTab === 'ciphers' ? 'active' : ''}`}
                onClick={() => onTabChange('ciphers')}
            >
                Предложени шифри
                <span className="tab-count">{cipherCount}</span>
            </button>

            <button
                className={`tab-btn ${activeTab === 'answers' ? 'active' : ''}`}
                onClick={() => onTabChange('answers')}
            >
                Предложени отговори
                <span className="tab-count">{answerCount}</span>
            </button>
        </div>
    );
};

export default SubmissionsTabs;