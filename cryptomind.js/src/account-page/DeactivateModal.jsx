import React from 'react';

function DeactivateModal({ onConfirm, onCancel, loading }) {
    return (
        <div className="modal-overlay" onClick={onCancel}>
            <div className="modal-card" onClick={(e) => e.stopPropagation()}>
                <div className="modal-icon">⚠️</div>
                <div className="modal-title">Deactivate Account</div>
                <div className="modal-body">
                    Are you sure you want to deactivate your account?
                    <br />
                    <strong>This action cannot be undone.</strong>
                    <br /><br />
                    You will lose access to all your ciphers, submissions, and leaderboard progress.
                </div>
                <div className="modal-actions">
                    <button
                        className="btn-modal-cancel"
                        onClick={onCancel}
                        disabled={loading}
                    >
                        Cancel
                    </button>
                    <button
                        className="btn-modal-confirm"
                        onClick={onConfirm}
                        disabled={loading}
                    >
                        {loading ? 'Deactivating...' : 'Yes, Deactivate'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default DeactivateModal;
