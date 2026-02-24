import React from 'react';

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
        'badge-deleted';

    const statusLabel =
        status === 'approved'        ? 'Одобрен'              :
        status === 'pending'         ? 'Очаква преглед'        :
        status === 'rejected'        ? 'Отхвърлен'            :
        'Шифърът е премахнат';

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
                        Отговор за: {cipherTitle}
                    </span>
                </div>
                <span className={`status-badge ${statusClass}`}>
                    <span className="status-dot" />
                    {statusLabel}
                </span>
            </div>

            {/* Meta */}
            <div className="card-meta">
                <span className="meta-item">Предложен: {suggestedAt}</span>

                {isCipherRemoved && deletedAt && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Шифърът е изтрит: {deletedAt}</span>
                    </>
                )}

                {status === 'pending' && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Очаква преглед</span>
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
                        <strong>Причина за отхвърляне: </strong>{rejectionReason}
                    </p>
                </div>
            )}

            {/* Cipher removed notice */}
            {isCipherRemoved && (
                <div className="deletion-notice">
                    <span className="deletion-notice__label">Шифърът е премахнат от администратор</span>
                    <p className="deletion-notice__reason">
                        {deletionReason
                            ? deletionReason
                            : 'Шифърът, за който е бил изпратен този отговор, е премахнат от платформата.'}
                    </p>
                </div>
            )}

            {/* Footer */}
            {!isCipherRemoved && (
                <div className="card-footer">
                    <div className="card-footer-left">
                        {status === 'approved' && pointsEarned != null && (
                            <span className="points-badge">+{pointsEarned} т.</span>
                        )}
                    </div>
                    <div className="card-footer-right">
                        {status === 'approved' && (
                            <button className="btn-card-action" onClick={onViewCipher}>
                                Виж шифъра
                            </button>
                        )}
                        {status !== 'approved' && (
                            <button className="btn-card-action" onClick={onViewDetails}>
                                Виж детайли
                            </button>
                        )}
                    </div>
                </div>
            )}

        </div>
    );
};

export default AnswerCard;