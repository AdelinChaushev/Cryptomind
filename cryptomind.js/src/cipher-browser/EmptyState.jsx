const EmptyState = ({ onReset }) => {
    return (
        <div className="empty-state">
            <div className="empty-icon">⊘</div>
            <h3 className="empty-title">Няма намерени шифри</h3>
            <p className="empty-message">Опитайте да промените филтрите или търсения израз.</p>
            <button className="btn btn-ghost" onClick={onReset}>
                Изчисти филтрите
            </button>
        </div>
    );
};

export default EmptyState;