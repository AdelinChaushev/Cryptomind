export default function CipherCard({ cipher, familyColor, isOpen, onToggle }) {
  const dc = diffColor(cipher.difficulty);
  const fc = cipher.color;
  const fb = fc + "15";
  function diffColor(d) {
    if (d === "Начинаещ")  return "#10b981";
    if (d === "Среден")    return "#fbbf24";
    if (d === "Напреднал") return "#f43f5e";
    return "#ffffff";
  }
  return (
    <div className="cl-cipher" id={cipher.id}
      style={{ "--fc": fc, "--fb": fb, "--dc": dc, "--db": dc + "15" }}>
      {/* HEAD — always visible, click to toggle */}
      <div className="cl-cipher-head" onClick={onToggle}>
        <div className="cl-cipher-icon">
          {cipher.label.slice(0,2).toUpperCase()}
        </div>
        <div className="cl-cipher-titles">
          <div className="cl-cipher-name">{cipher.label}</div>
          <div className="cl-cipher-sub">{cipher.how.split(".")[0]}.</div>
          <div className="cl-cipher-badges">
            <span className="cl-badge cl-badge-era">{cipher.era}</span>
            <span className="cl-badge cl-badge-diff">{cipher.difficulty}</span>
            {cipher.tags.map(t=>(
              <span key={t} className="cl-badge cl-badge-tag">{t}</span>
            ))}
          </div>
        </div>
        <span className={`cl-chevron ${isOpen?"open":""}`}>▼</span>
      </div>

      {/* BODY — expanded content */}
      <div className={`cl-cipher-body ${isOpen?"open":""}`}>
        <div className="cl-divider"/>

        {/* key type */}
        <div className="cl-key-info">
          <span className="cl-key-label">KEY TYPE:</span>
          <span className="cl-key-val">{cipher.keyType}</span>
        </div>

        {/* example */}
        <div className="cl-example">
          <div className="cl-example-tag">// quick example</div>
          <div className="cl-example-row">
            <span className="cl-ex-label">PLAIN</span>
            <span className="cl-ex-val cl-ex-plain">{cipher.example.plain}</span>
          </div>
          {cipher.example.key !== "—" && (
            <div className="cl-example-row" style={{marginTop:6}}>
              <span className="cl-ex-label">KEY</span>
              <span className="cl-ex-val cl-ex-key">{cipher.example.key}</span>
            </div>
          )}
          <div className="cl-example-row" style={{marginTop:6}}>
            <span className="cl-ex-label">CIPHER</span>
            <span className="cl-ex-val cl-ex-cipher">{cipher.example.cipher}</span>
          </div>
        </div>

        {/* how it works + formula */}
        <div className="cl-grid2">
          <div className="cl-block">
            <div className="cl-block-tag">// how it works</div>
            <div className="cl-block-text">{cipher.how}</div>
            {cipher.formula && (
              <div className="cl-formula">{cipher.formula}</div>
            )}
          </div>
          <div>
            {/* weakness */}
            <div className="cl-weakness" style={{marginBottom:12}}>
              <div className="cl-weakness-tag">// cryptographic weakness</div>
              <div className="cl-weakness-text">{cipher.weakness}</div>
            </div>
          </div>
        </div>

        {/* fun fact */}
        <div className="cl-funfact">
          <div className="cl-funfact-tag">// historical note</div>
          <div className="cl-funfact-text">{cipher.funFact}</div>
        </div>

        {/* CTAs */}
        <div className="cl-cta-row">
          <a className="cl-cta cl-cta-primary"
            href={`/cipher-tool#${cipher.id}`}>
            ▶ See it in action
          </a>
        </div>
      </div>
    </div>
  );
}
