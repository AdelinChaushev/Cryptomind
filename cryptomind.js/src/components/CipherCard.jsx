// Props for CipherCard:
//   cipher — object with the shape below (map from your API response):
//   {
//     id:               number
//     title:            string
//     cipherPreview:    string   — short snippet of the encrypted text
//     challengeType:    number   — 1 = Standard, 2 = Experimental
//     tags:             string[] — display names of the cipher type tags
//     solverCount:      number
//     createdAt:        string   — e.g. "3d ago", format on the backend or here
//   }

import { Link } from "react-router-dom";

const CHALLENGE_TYPE_BADGE = {
    1: { label: 'Standard',     className: 'badge-standard' },
    2: { label: 'Experimental', className: 'badge-experimental' },
};

const CipherCard = ({ cipher }) => {
    if (!cipher) return null;
    const typeBadge = CHALLENGE_TYPE_BADGE[cipher.challengeType];

    return (
        <article className="cipher-card" data-id={cipher.id}>
            <div className="card-header">
                <div className="card-badges">
                    {typeBadge && (
                        <span className={`badge ${typeBadge.className}`}>
                            {typeBadge.label}
                        </span>
                    )}
                </div>
            </div>

            <h3 className="card-title">{cipher.title}</h3>

            {cipher.cipherPreview && (
                <div className="card-cipher-preview">
                    <code className="cipher-text-preview">{cipher.cipherPreview}</code>
                </div>
            )}

            {cipher.tags && cipher.tags.length > 0 && (
                <div className="card-tags">
                    {cipher.tags.map((tag, index) => (
                        <span className="tag" key={index}>{tag}</span>
                    ))}
                </div>
            )}

            <div className="card-footer">
                <div className="card-meta">
                    <span className="meta-item">
                        <span className="meta-icon">✓</span>
                        {cipher.solverCount} solvers
                    </span>
                    <span className="meta-item">
                        <span className="meta-icon">◷</span>
                        {cipher.createdAt}
                    </span>
                </div>
                <Link to={`/cipher/${cipher.id}`} className="btn btn-card">
                    Solve →
                </Link>
            </div>
        </article>
    );
};

// Skeleton loader — shown while ciphers are being fetched
export const CipherCardSkeleton = () => (
    <div className="cipher-card skeleton" />
);

export default CipherCard;
