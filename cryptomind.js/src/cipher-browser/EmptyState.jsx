// Props:
//   onReset — () => void, called when the user clicks "Clear filters"

const EmptyState = ({ onReset }) => {
    return (
        <div className="empty-state">
            <div className="empty-icon">⊘</div>
            <h3 className="empty-title">No ciphers found</h3>
            <p className="empty-message">Try adjusting your filters or search term.</p>
            <button className="btn btn-ghost" onClick={onReset}>
                Clear filters
            </button>
        </div>
    );
};

export default EmptyState;
