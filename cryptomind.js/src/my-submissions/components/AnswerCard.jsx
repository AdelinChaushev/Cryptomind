import React from 'react';

/* Props:
   cipherTitle   : string  (the cipher this answer was submitted for)
   status        : 'approved' | 'pending' | 'rejected'
   suggestedAt   : string  (e.g. "Dec 16, 2024")
   description   : string
   pointsEarned  : number  — only shown when status === 'approved'
   onViewCipher  : handler passed from parent
   onViewDetails : handler passed from parent
*/
const AnswerCard = ({
    cipherTitle,
    status,
    suggestedAt,
    description,
    pointsEarned,
    onViewCipher,
    onViewDetails,
}) => {
    const statusClass = `status-${status.toLowerCase()}`;
    const statusLabel =
        status === 'Approved' ? 'Correct' :
        status === 'Pending'  ? 'Pending Review' :
        'Incorrect';

    const descriptionBorderClass =
        status === 'Approved' ? 'border-emerald' :
        status === 'Rejected' ? 'border-rose' :
        'border-yellow';

    return (
        <div className="submission-card">
            {/* Header */}
            <div className="card-header">
                <span className="card-title">Answer for: "{cipherTitle}"</span>
                <span className={`status-badge ${statusClass}`}>{statusLabel}</span>
            </div>

            {/* Meta */}
            <div className="card-meta">
                <span className="meta-item">📅 {suggestedAt}</span>

                {status === 'Approved' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Verified Correct</span>
                    </>
                )}

                {status === 'Pending' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Awaiting Verification</span>
                    </>
                )}

                {status == 'Rejected' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Not Accepted</span>
                    </>
                )}
            </div>

            {/* Description */}
            <p className={`card-description ${descriptionBorderClass}`}>
                {description}
            </p>

            {/* Footer */}
            <div className="card-footer">
                <div className="card-footer-left">
                    {status === 'Approved' && pointsEarned !== undefined && (
                        <span className="points-badge">+{pointsEarned} pts</span>
                    )}
                </div>

                <div className="card-footer-right">
                    {status === 'Approved' && (
                        <button className="btn-card-action" onClick={onViewCipher}>
                            View Cipher
                        </button>
                    )}
                    {status !== 'Approved' && (
                        <button className="btn-card-action" onClick={onViewDetails}>
                            View Details
                        </button>
                    )}
                </div>
            </div>
        </div>
    );
};

export default AnswerCard;
