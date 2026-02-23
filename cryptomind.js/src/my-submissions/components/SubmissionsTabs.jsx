import React from 'react';

/* Props:
   activeTab         : 'ciphers' | 'answers'
   onTabChange       : (tab: string) => void
   cipherCount       : number
   answerCount       : number
*/
const SubmissionsTabs = ({ activeTab, onTabChange, cipherCount = 0, answerCount = 0 }) => {
    return (
        <div className="tabs-nav">
            <button
                className={`tab-btn ${activeTab === 'ciphers' ? 'active' : ''}`}
                onClick={() => onTabChange('ciphers')}
            >
                Cipher Submissions
                <span className="tab-count">{cipherCount}</span>
            </button>

            <button
                className={`tab-btn ${activeTab === 'answers' ? 'active' : ''}`}
                onClick={() => onTabChange('answers')}
            >
                Answer Suggestions
                <span className="tab-count">{answerCount}</span>
            </button>
        </div>
    );
};

export default SubmissionsTabs;
