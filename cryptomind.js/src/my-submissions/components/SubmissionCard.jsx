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
            status === 'pending' ? 'badge-pending' :
                status === 'rejected' ? 'badge-rejected' :
                    'badge-deleted';

    const statusLabel =
        status === 'approved' ? 'Одобрен' :
            status === 'pending' ? 'Очаква преглед' :
                status === 'rejected' ? 'Отхвърлен' :
                    'Изтрит';

    const descriptionBorderClass =
        status === 'approved' ? 'border-emerald' :
            status === 'rejected' ? 'border-rose' :
                status === 'cipherdeleted' ? 'border-deleted' :
                    'border-yellow';

    const isDeleted = status === 'deleted';

    return (
        <div className={`submission-card ${isDeleted ? 'submission-card--deleted' : ''}`}>

            <div className="card-header">
                <div className="card-title-group" style={{ minWidth: 0, flex: 1, overflow: 'hidden' }}>
                    <span
                        className={`card-title ${isDeleted ? 'card-title--deleted' : ''}`}
                        style={{ display: 'block', wordBreak: 'break-all', overflowWrap: 'break-word' }}
                    >
                        {title}
                    </span>
                    {cipherType && (
                        <span className="card-type-tag">{cipherType}</span>
                    )}
                </div>
                <span className={`status-badge ${statusClass}`} style={{ flexShrink: 0 }}>
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
                    <span className="rejection-icon">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/>
                            <line x1="12" y1="9" x2="12" y2="13"/>
                            <line x1="12" y1="17" x2="12.01" y2="17"/>
                        </svg>
                    </span>
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