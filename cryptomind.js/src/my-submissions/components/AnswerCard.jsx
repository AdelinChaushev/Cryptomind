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

}) => {
    const isCipherRemoved = status === 'cipherdeleted';
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

            
            {!isCipherRemoved && (
                <div className="card-footer">
                    <div className="card-footer-left">
                        {status === 'approved' && pointsEarned != null && (
                            <span className="points-badge">+{pointsEarned} т.</span>
                        )}
                    </div>

                    <div className="card-footer-right">

                            <button className="btn-card-action" onClick={ onViewCipher}>
                                Виж шифъра
                            </button>

                    </div>
                </div>
            )}

        </div>
    );
};

export default AnswerCard;