const ImageIcon = () => (
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
        stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
        style={{ width: '11px', height: '11px' }}>
        <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
        <circle cx="8.5" cy="8.5" r="1.5" />
        <polyline points="21 15 16 10 5 21" />
    </svg>
);

const CheckIcon = () => (
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none"
        stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"
        style={{ width: '11px', height: '11px' }}>
        <polyline points="20 6 9 17 4 12" />
    </svg>
);

const CHALLENGE_TYPE_BADGE = {
    Standard:     'badge-standard',
    Experimental: 'badge-experimental',
};

const CipherCard = ({ cipher }) => {
    if (!cipher) return null;
    const typeBadgeClass = CHALLENGE_TYPE_BADGE[cipher.challengeTypeDisplay ?? ''] || null;

    return (
        <article className={`cipher-card ${cipher.alreadySolved ? 'cipher-card--solved' : ''}`} data-id={cipher.id}>
            <div className="card-header">
                <div className="card-badges">
                    {typeBadgeClass && (
                        <span className={`badge ${typeBadgeClass}`}>
                            {cipher.challengeTypeDisplay === "Standard" ? "Стандартен" : "Експериментален" }
                        </span>
                    )}

                    {cipher.isImage && (
                        <span className="badge badge-image">
                            <ImageIcon /> Изображение
                        </span>
                    )}

                    {cipher.alreadySolved && (
                        <span className="badge badge-solved">
                            <CheckIcon /> Решен
                        </span>
                    )}
                </div>
            </div>

            <h3 className="card-title">{cipher.title}</h3>

            <div className="card-footer">
                <a href={`/cipher/${cipher.id}`} className="btn btn-card">
                    {cipher.alreadySolved ? 'Виж' : 'Реши'}
                </a>
            </div>
        </article>
    );
};

export const CipherCardSkeleton = () => (
    <div className="cipher-card skeleton" />
);

export default CipherCard;