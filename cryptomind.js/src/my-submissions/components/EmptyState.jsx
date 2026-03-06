const EmptyState = ({ icon = '📭', title, subtitle, ctaLabel, ctaHref }) => {
    return (
        <div className="empty-state">
            <div className="empty-state-icon">{icon}</div>
            <p className="empty-state-title">{title}</p>
            <p className="empty-state-subtitle">{subtitle}</p>
            {ctaLabel && ctaHref && (
                <a href={ctaHref} className="btn-empty-cta">{ctaLabel}</a>
            )}
        </div>
    );
};

export default EmptyState;
