import React from 'react';

/* Props:
   title           : string
   status          : 'approved' | 'pending' | 'rejected' | 'deleted'
   submittedAt     : string
   deletedAt       : string | null    — only present when status === 'deleted'
   deletionReason  : string | null    — optional, shown in notice block
   cipherType      : string | null
   definition      : string | null
   description     : string
   rejectionReason : string | null
   onViewCipher    : () => void       — only called when status === 'approved'
   onViewDetails   : () => void       — called for pending / rejected
*/
const SubmissionCard = ({
    title,
    status,
    submittedAt,
    deletedAt,
    deletionReason,
    cipherType,
    definition,
    description,
    rejectionReason,
    onViewCipher,
    onViewDetails,
}) => {
    const statusClass =
        status === 'approved' ? 'badge-approved' :
        status === 'pending'  ? 'badge-pending'  :
        status === 'rejected' ? 'badge-rejected' :
        'badge-deleted';

    const statusLabel =
        status === 'approved' ? 'Approved'       :
        status === 'pending'  ? 'Pending Review' :
        status === 'rejected' ? 'Rejected'       :
        'Deleted';

    const descriptionBorderClass =
        status === 'approved' ? 'border-emerald' :
        status === 'rejected' ? 'border-rose'    :
        status === 'cipherdeleted'  ? 'border-deleted' :
        'border-yellow';

    const isDeleted = status === 'deleted';

    return (
        <div className={`submission-card ${isDeleted ? 'submission-card--deleted' : ''}`}>

            {/* Header */}
            <div className="card-header">
                <div className="card-title-group">
                    <span className={`card-title ${isDeleted ? 'card-title--deleted' : ''}`}>
                        {title}
                    </span>
                    {cipherType && (
                        <span className="card-type-tag">{cipherType}</span>
                    )}
                </div>
                <span className={`status-badge ${statusClass}`}>
                    <span className="status-dot" />
                    {statusLabel}
                </span>
            </div>

            {/* Meta */}
            <div className="card-meta">
                <span className="meta-item">Submitted: {submittedAt}</span>

                {isDeleted && deletedAt && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Deleted: {deletedAt}</span>
                    </>
                )}

                {definition && !isDeleted && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">{definition}</span>
                    </>
                )}

                {status === 'pending' && (
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
            {status === 'rejected' && rejectionReason && (
                <div className="rejection-notice">
                    <span className="rejection-icon">⚠</span>
                    <p className="rejection-text">
                        <strong>Rejection reason: </strong>{rejectionReason}
                    </p>
                </div>
            )}

            {/* Deletion notice */}
            {isDeleted && (
                <div className="deletion-notice">
                    <span className="deletion-notice__label">Removed by admin</span>
                    <p className="deletion-notice__reason">
                        {deletionReason
                            ? deletionReason
                            : 'This cipher was removed from the platform by an administrator.'}
                    </p>
                </div>
            )}

            {/* Footer — no actions for deleted cards */}
            {!isDeleted && (
                <div className="card-footer">
                    <div className="card-footer-left" />
                    <div className="card-footer-right">
                        {status === 'approved' && (
                            <button className="btn-card-action" onClick={onViewCipher}>
                                View Cipher
                            </button>
                        )}
                        {/* {(status === 'pending' || status === 'rejected') && (
                            <button className="btn-card-action" onClick={onViewDetails}>
                                View Details
                            </button>
                        )} */}
                    </div>
                </div>
            )}

        </div>
    );
};

export default SubmissionCard;