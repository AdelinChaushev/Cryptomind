import React from 'react';

const AVAILABLE_TAGS = [
    { id: 1, label: 'Image' },
    { id: 2, label: 'Puzzle' },
    { id: 3, label: 'Historical' },
    { id: 4, label: 'Short' },
    { id: 5, label: 'Long' },
    { id: 6, label: 'Beginner Friendly' },
    { id: 7, label: 'Tricky' }
];

const AdminActions = ({
    cipher,
    title,
    onTitleChange,
    selectedTags,
    onTagToggle,
    allowHint,
    onAllowHintChange,
    allowSolution,
    onAllowSolutionChange,
    challengeType,
    onChallengeTypeChange,
    rejectReason,
    onRejectReasonChange,
    showRejectForm,
    onRejectToggle,
    onApproveStandard,
    onApproveExperimental,
    onConfirmReject,
    onCancelReject
}) => {
    return (
        <div className="actions-column">
            {/* ─── Edit Details ─── */}
            <div className="admin-card">
                <div className="admin-card-header">
                    <span className="admin-card-title">Cipher Details</span>
                </div>

                <div className="form-group">
                    <label className="form-label">Title</label>
                    <input
                        type="text"
                        className="form-input"
                        value={title}
                        onChange={(e) => onTitleChange(e.target.value)}
                        placeholder="Enter cipher title..."
                    />
                </div>
            </div>

            {/* ─── Permissions ─── */}
            <div className="admin-card">
                <div className="admin-card-header">
                    <span className="admin-card-title">AI Assistance Permissions</span>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                    <label className="permission-toggle">
                        <input
                            type="checkbox"
                            checked={allowHint}
                            onChange={(e) => onAllowHintChange(e.target.checked)}
                        />
                        <span>Allow Hints</span>
                    </label>

                    <label className="permission-toggle">
                        <input
                            type="checkbox"
                            checked={allowSolution}
                            onChange={(e) => onAllowSolutionChange(e.target.checked)}
                        />
                        <span>Allow Full Solution</span>
                    </label>
                </div>
            </div>

            {/* ─── Tags ─── */}
            <div className="admin-card">
                <div className="admin-card-header">
                    <span className="admin-card-title">Tags</span>
                </div>

                <div className="tag-cloud">
                    {AVAILABLE_TAGS.map((tag) => (
                        <button
                            key={tag.id}
                            className={`tag-chip${selectedTags.includes(tag.id) ? ' tag-selected' : ''}`}
                            onClick={() => onTagToggle(tag.id)}
                        >
                            {tag.label}
                        </button>
                    ))}
                </div>
            </div>

            {/* ─── Challenge Type ─── */}
            <div className="admin-card">
                <div className="admin-card-header">
                    <span className="admin-card-title">Challenge Type</span>
                </div>

                <div className="type-toggle">
                    <button
                        className={`type-toggle-btn${challengeType === 'Standard' ? ' active-standard' : ''}`}
                        onClick={() => onChallengeTypeChange('Standard')}
                    >
                        <span className="type-toggle-dot dot-sky" />
                        Standard
                        <span className="type-toggle-note">HAS SOLUTION</span>
                    </button>
                    <button
                        className={`type-toggle-btn${challengeType === 'Experimental' ? ' active-experimental' : ''}`}
                        onClick={() => onChallengeTypeChange('Experimental')}
                    >
                        <span className="type-toggle-dot dot-violet" />
                        Experimental
                        <span className="type-toggle-note">NO SOLUTION YET</span>
                    </button>
                </div>
            </div>

            {/* ─── Actions ─── */}
            <div className="admin-card">
                <div className="admin-card-header">
                    <span className="admin-card-title">Review Decision</span>
                </div>

                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                    <button
                        onClick={onApproveStandard}
                        className="btn btn-success"
                        style={{ justifyContent: 'center' }}
                    >
                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                            <path d="M2 8l4 4 8-8"/>
                        </svg>
                        Approve as Standard
                    </button>

                    <button
                        onClick={onApproveExperimental}
                        className="btn btn-sky"
                        style={{ justifyContent: 'center' }}
                    >
                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                            <circle cx="8" cy="8" r="6.5"/><path d="M8 5v3l2 2"/>
                        </svg>
                        Approve as Experimental
                    </button>

                    <div className="reject-section">
                        <button
                            onClick={onRejectToggle}
                            className="btn btn-danger"
                            style={{ justifyContent: 'center', width: '100%' }}
                        >
                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                <path d="M3 3l10 10M13 3L3 13"/>
                            </svg>
                            Reject Submission
                        </button>

                        {showRejectForm && (
                            <div className="reject-form" style={{ marginTop: '10px' }}>
                                <label className="form-label">
                                    Rejection Reason <span style={{ color: 'var(--rose-500)' }}>*</span>
                                </label>
                                <textarea
                                    className="form-textarea"
                                    placeholder="Explain why this is being rejected..."
                                    rows="3"
                                    value={rejectReason}
                                    onChange={(e) => onRejectReasonChange(e.target.value)}
                                />
                                <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                    <button onClick={onConfirmReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>
                                        Confirm Reject
                                    </button>
                                    <button onClick={onCancelReject} className="btn btn-ghost btn-sm">
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminActions;