function CipherMeta({ cipher, timeAgo }) {
  return (
    <div className="cipher-meta-panel">
      <div className="meta-header">
        <p className="meta-header-title">Инфо за шифъра</p>
      </div>

      <div className="meta-body">
        {cipher.challengeTypeDisplay === "Standard" && (
          <div className="points-display">
            <div className="points-value">{cipher.points ?? 100}</div>

            <div className="meta-stats">
              <div className="meta-stat">
                <span className="meta-stat-label">
                  <span className="meta-stat-icon">✅</span>
                  Решен пъти
                </span>
                <span className="meta-stat-value emerald">
                  {cipher.successfulSubmissions ?? 0}
                </span>
              </div>

              <div className="meta-stat">
                <span className="meta-stat-label">
                  <span className="meta-stat-icon">📊</span>
                  Успеваемост
                </span>
                <span className="meta-stat-value yellow">{cipher.successRate}%</span>
              </div>

              <div className="meta-stat">
                <span className="meta-stat-label">
                  <span className="meta-stat-icon">📅</span>
                  Добавен
                </span>
                <span className="meta-stat-value cyan">
                  {timeAgo(cipher.dateSubmitted)}
                </span>
              </div>
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
                  style={{ width: `${cipher.successRate ?? 0}%` }}
                />
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default CipherMeta;