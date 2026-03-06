import React from 'react';

const CipherInputToggle = ({ useImage, onToggle }) => {
    return (
        <div className="mode-toggle">
            <div className="mode-toggle-left">
                <span className="mode-toggle-label">
                    {useImage ? 'Качване на изображение' : 'Въвеждане на текст'}
                </span>
                <span className="mode-toggle-sub">
                    {useImage
                        ? 'Качете снимка или екранна снимка на шифъра'
                        : 'Поставете или въведете криптирания текст директно'}
                </span>
            </div>

            <div className="mode-toggle-right">
                <span className={`mode-label ${!useImage ? 'active' : ''}`}>ТЕКСТ</span>

                <label className="toggle">
                    <input
                        type="checkbox"
                        checked={useImage}
                        onChange={onToggle}
                    />
                    <span className="toggle-track"></span>
                </label>

                <span className={`mode-label ${useImage ? 'active' : ''}`}>ИЗОБРАЖЕНИЕ</span>
            </div>
        </div>
    );
};

export default CipherInputToggle;