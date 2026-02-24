import React from 'react';

const SubmitActions = ({ onSubmit, onCancel }) => {
    return (
        <div className="submit-card">
            <button className="btn-submit" onClick={onSubmit}>
                ИЗПРАТИ ЗА ПРЕГЛЕД
            </button>
            <button className="btn-cancel" onClick={onCancel}>
                Отказ
            </button>
            <p className="submit-note">
                Вашето предложение влиза в опашка за изчакване и е видимо само за администратори до одобрението му.
            </p>
        </div>
    );
};

export default SubmitActions;