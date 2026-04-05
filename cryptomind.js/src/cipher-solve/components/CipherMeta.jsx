function CipherMeta({ cipher, timeAgo }) {
  return (
    <div className="cipher-meta-panel">
      <div className="meta-header">
        <span className="label-dot"></span>
        <p className="meta-header-title">Инфо за шифъра</p>
      </div>

      <div className="meta-body">
        <div className="points-display">
          <div className="points-value">{cipher.points ?? 100}</div>
          <div className="points-label">НАЛИЧНИ ТОЧКИ</div>
        </div>

        <div className="meta-stat-pills">
          <div className="meta-stat-pill">
            <span className="pill-num emerald">{cipher.successfulSubmissions ?? 0}</span>
            <span className="pill-label">Решени</span>
          </div>
          <div className="meta-stat-pill">
            <span className="pill-num yellow">{Math.round((cipher.successRate ?? 0) * 100) / 100}%</span>
            <span className="pill-label">Успеваемост</span>
          </div>
        </div>

        <div className="meta-stat">
          <span className="meta-stat-label">
            <span className="meta-clock-icon"></span>
            Добавен
          </span>
          <span className="meta-stat-value cyan">{timeAgo(cipher.dateSubmitted)}</span>
        </div>

        <div className="solve-cta">
          <div className="solve-progress-label">
            <span className="progress-text">Процент на решаване</span>
            <span className="progress-count">
              {cipher.successfulSubmissions ?? 0} / {cipher.totalAttempts ?? 0}
            </span>
          </div>
          <div className="progress-bar">
            <div
              className="progress-fill"
              style={{ width: `${Math.round((cipher.successRate ?? 0) * 100) / 100}%` }}
            />
          </div>
        </div>
      </div>
    </div>
  );
}

export default CipherMeta;