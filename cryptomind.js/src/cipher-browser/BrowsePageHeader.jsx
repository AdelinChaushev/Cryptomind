const TICKER =
    'A · B · C · D · E · F · G · H · I · J · K · L · M · N · O · P · Q · R · S · T · U · V · W · X · Y · Z \u2003·\u2003 ' +
    'I · II · III · IV · V · VI · VII · VIII · IX · X \u2003·\u2003 ' +
    'α · β · γ · δ · ε · ζ · η · θ · ι · κ · λ · μ · ν · ξ · ο · π · ρ · σ · τ · υ · φ · χ · ψ · ω \u2003·\u2003 ' +
    '⊕ · ∇ · △ · ◇ · ■ · ▲ · ◆ · ◈ · ⊗ · ∑ · ∞ · ≠ · ≡ · ∂ · ∫ \u2003·\u2003 ';

const BrowsePageHeader = () => (
    <section className="page-header">
        <div className="ph-eyebrow">
            <span className="ph-rule" aria-hidden="true" />
            <span className="ph-tag">ТРЕЗОР НА ШИФРИ</span>
            <span className="ph-pulse" aria-hidden="true" />
        </div>

        <h1 className="page-title">
            Разгледай{' '}
            <em className="title-accent">Предизвикателствата</em>
        </h1>

        <p className="page-subtitle">
            Дешифрирайте, идентифицирайте и покорявайте класически шифри, предложени от общността.
        </p>

        <div className="cipher-ticker-wrap" aria-hidden="true">
            <div className="cipher-ticker">
                <span>{TICKER}</span>
                <span>{TICKER}</span>
            </div>
        </div>
    </section>
);

export default BrowsePageHeader;
