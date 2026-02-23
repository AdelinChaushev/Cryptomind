import React from 'react';

const SubmitActions = ({ onSubmit, onCancel }) => {
    return (
        <div className="submit-card">
            <button className="btn-submit" onClick={onSubmit}>
                SUBMIT FOR REVIEW
            </button>
            <button className="btn-cancel" onClick={onCancel}>
                Cancel
            </button>
            <p className="submit-note">
                Your submission enters a pending queue and is only visible to admins until approved.
            </p>
        </div>
    );
};

export default SubmitActions;
