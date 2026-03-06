import React from 'react';

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
        status === 'approved' ? 'Одобрен'         :
        status === 'pending'  ? 'Очаква преглед'   :
        status === 'rejected' ? 'Отхвърлен'        :
        'Изтрит';

    const descriptionBorderClass =
        status === 'approved'       ? 'border-emerald' :
        status === 'rejected'       ? 'border-rose'    :
        status === 'cipherdeleted'  ? 'border-deleted' :
        'border-yellow';

    const isDeleted = status === 'deleted';

    return (
        <div className={`submission-card ${isDeleted ? 'submission-card--deleted' : ''}`}>

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

            <div className="card-meta">
                <span className="meta-item">Предложен: {submittedAt}</span>

                {isDeleted && deletedAt && (
                    <>
                        <span className="meta-dot" />
                        <span className="meta-item">Изтрит: {deletedAt}</span>
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
                        <span className="meta-item">В процес на преглед</span>
                    </>
                )}
            </div>

            <p className={`card-description ${descriptionBorderClass}`}>
                {description}
            </p>

            {status === 'rejected' && rejectionReason && (
                <div className="rejection-notice">
                    <span className="rejection-icon">⚠</span>
                    <p className="rejection-text">
                        <strong>Причина за отхвърляне: </strong>{rejectionReason}
                    </p>
                </div>
            )}

            {isDeleted && (
                <div className="deletion-notice">
                    <span className="deletion-notice__label">Премахнат от администратор</span>
                    <p className="deletion-notice__reason">
                        {deletionReason
                            ? deletionReason
                            : 'Този шифър беше премахнат от платформата от администратор.'}
                    </p>
                </div>
            )}

            {!isDeleted && (
                <div className="card-footer">
                    <div className="card-footer-left" />
                    <div className="card-footer-right">
                        {status === 'approved' && (
                            <button className="btn-card-action" onClick={onViewCipher}>
                                Виж шифъра
                            </button>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};

export default SubmissionCard;