function ActivityLog({ solvers, timeAgo }) {
    return (
        <div className="activity-panel">
            <div className="activity-header">
                <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                    <span className="label-dot"></span>
                    <p className="activity-header-title">Скорошни решения</p>
                </div>
                <div className="live-badge">
                    <span className="live-dot"></span>
                    На живо
                </div>
            </div>

            <div className="activity-list">
                {solvers.length === 0 ? (
                    <div className="activity-empty">
                        <p className="activity-empty-text">
                            Все още няма решения — бъди първият!
                        </p>
                    </div>
                ) : (
                    solvers.map((solver, i) => (
                        <div className="activity-item" key={i}>
                            <div className="activity-avatar">
                                {solver.userName?.charAt(0) ?? "?"}
                            </div>
                            <div className="activity-info">
                                <p className="activity-username">{solver.userName}</p>
                                <p className="activity-time">{timeAgo(solver.solvedSince)}</p>
                            </div>
                            <span className="activity-check">✓</span>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}

export default ActivityLog;
