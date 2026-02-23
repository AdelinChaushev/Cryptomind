import React from 'react';

/* Props:
   title           : string
   status          : 'approved' | 'pending' | 'rejected'
   submittedAt     : string   (e.g. "Dec 15, 2024")
   cipherType      : string   (e.g. "Caesar", "Vigenere") — optional
   definition      : string   (e.g. "Standard", "Experimental") — optional
   description     : string
   rejectionReason : string   — only shown when status === 'rejected'
   onViewCipher    : handler passed from parent
   onViewDetails   : handler passed from parent
*/
const SubmissionCard = ({
    title,
    status,
    submittedAt,
    cipherType,
    definition,
    description,
    rejectionReason,
    onViewCipher,
    
}) => {
    status = status == "CipherDeleted" ? "Rejected" : status;
    console.log(status) // Treat deleted ciphers as rejected for display purposes
    const statusClass = `status-${status.toLowerCase()}`;
    const statusLabel =
        status === 'Approved' ? 'Approved' :
        status === 'Pending'  ? 'Pending Review' :
        'Rejected';

    const descriptionBorderClass =
        status === 'Approved' ? 'border-emerald' :
        status === 'Rejected' ? 'border-rose' :
        'border-yellow';

    return (
        <div className="submission-card">
            {/* Header */}
            <div className="card-header">
                <span className="card-title">{title}</span>
                <span className={`status-badge ${statusClass}`}>{statusLabel}</span>
            </div>

            {/* Meta */}
            <div className="card-meta">
                <span className="meta-item">📅 {submittedAt}</span>

                {cipherType && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-tag">{cipherType}</span>
                    </>
                )}

                {definition && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">{definition}</span>
                    </>
                )}

                {status === 'Pending' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Under Review</span>
                    </>
                )}
            </div>

            {/* Description */}
            <p className={`card-description ${descriptionBorderClass}`}>
                {description}
            </p>

            {/* Rejection notice */}
            {status === 'Rejected' && (
                <div className="rejection-notice">
                    <span className="rejection-icon">⚠</span>
                    <p className="rejection-text">
                        <strong>Rejection reason: </strong>{rejectionReason ? rejectionReason : "Not specified by admin."}
                    </p>
                </div>
            )}

            {/* Footer */}
            <div className="card-footer">
                <div className="card-footer-left">
                    {status === 'Approved' && (
                        <button className="btn-card-action" onClick={onViewCipher}>
                            View Cipher
                        </button>
                    )}
                    {/* {(status === 'Pending' || status === 'Rejected') && (
                        <button className="btn-card-action" onClick={onViewDetails}>
                            View Details
                        </button>
                    )} */}
                </div>
            </div>
        </div>
    );
};

export default SubmissionCard;
