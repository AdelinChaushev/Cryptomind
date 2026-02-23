import React from 'react';

const CipherInputToggle = ({ useImage, onToggle }) => {
    return (
        <div className="mode-toggle">
            <div className="mode-toggle-left">
                <span className="mode-toggle-label">
                    {useImage ? 'Image Upload' : 'Text Input'}
                </span>
                <span className="mode-toggle-sub">
                    {useImage
                        ? 'Upload a photo or screenshot of the cipher'
                        : 'Paste or type the encrypted text directly'}
                </span>
            </div>

            <div className="mode-toggle-right">
                <span className={`mode-label ${!useImage ? 'active' : ''}`}>TEXT</span>

                <label className="toggle">
                    <input
                        type="checkbox"
                        checked={useImage}
                        onChange={onToggle}
                    />
                    <span className="toggle-track"></span>
                </label>

                <span className={`mode-label ${useImage ? 'active' : ''}`}>IMAGE</span>
            </div>
        </div>
    );
};

export default CipherInputToggle;
