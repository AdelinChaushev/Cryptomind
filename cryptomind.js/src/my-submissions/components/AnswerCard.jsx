import React from 'react';

/* Props:
   cipherTitle     : string
   status          : 'approved' | 'pending' | 'rejected' | 'cipher_removed'
   suggestedAt     : string
   deletedAt       : string | null    — when status === 'cipher_removed'
   deletionReason  : string | null    — admin reason the cipher was removed
   description     : string           — the user's suggested answer text
   rejectionReason : string | null
   pointsEarned    : number | null    — only meaningful when status === 'approved'
   onViewCipher    : () => void
   onViewDetails   : () => void
*/

const AnswerCard = ({
    cipherTitle,
    status,
    suggestedAt,
    deletedAt,
    deletionReason,
    description,
    rejectionReason,
    pointsEarned,
    onViewCipher,
    onViewDetails,
}) => {
    const isCipherRemoved = status === 'cipher_removed';
     
    const statusClass =
        status === 'approved'        ? 'badge-approved'  :
        status === 'pending'         ? 'badge-pending'   :
        status === 'rejected'        ? 'badge-rejected'  :
        'badge-deleted';  // cipher_removed

    const statusLabel =
        status === 'approved'        ? 'Approved'        :
        status === 'pending'         ? 'Pending Review'  :
        status === 'rejected'        ? 'Rejected'        :
        'Cipher Removed';

    const descriptionBorderClass =
        status === 'approved'        ? 'border-emerald'  :
        status === 'rejected'        ? 'border-rose'     :
        isCipherRemoved              ? 'border-deleted'  :
        'border-yellow';

    return (
        <div className={`submission-card ${isCipherRemoved ? 'submission-card--deleted' : ''}`}>

            {/* Header */}
            <div className="card-header">
                <div className="card-title-group">
                    <span className={`card-title ${isCipherRemoved ? 'card-title--deleted' : ''}`}>
                        Answer for: {cipherTitle}
                    </span>
                </div>
                <span className={`status-badge ${statusClass}`}>
                    <span className="status-dot" />
                    {statusLabel}
                </span>
            </div>

            {/* Meta */}
            <div className="card-meta">
                <span className="meta-item">Suggested: {suggestedAt}</span>

                {isCipherRemoved && deletedAt && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Cipher deleted: {deletedAt}</span>
                    </>
                )}

                {status === 'pending' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Awaiting Review</span>
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

            {/* Cipher removed notice */}
            {isCipherRemoved && (
                <div className="deletion-notice">
                    <span className="deletion-notice__label">Cipher removed by admin</span>
                    <p className="deletion-notice__reason">
                        {deletionReason
                            ? deletionReason
                            : 'The cipher this answer was submitted for has been removed from the platform.'}
                    </p>
                </div>
            )}

            {/* Footer — no actions for removed cipher cards */}
            {!isCipherRemoved && (
                <div className="card-footer">
                    <div className="card-footer-left">
                        {status === 'approved' && pointsEarned != null && (
                            <span className="points-badge">+{pointsEarned} pts</span>
                        )}
                    </div>
                    <div className="card-footer-right">
                        {status === 'approved' && (
                            <button className="btn-card-action" onClick={onViewCipher}>
                                View Cipher
                            </button>
                        )}
                        {status !== 'approved' && (
                            <button className="btn-card-action" onClick={onViewDetails}>
                                View Details
                            </button>
                        )}
                    </div>
                </div>
            )}

        </div>
    );
};

export default AnswerCard;