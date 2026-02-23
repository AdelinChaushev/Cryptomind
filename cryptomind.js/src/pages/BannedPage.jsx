import "../styles/banned.css";

// ─── Background ───────────────────────────────────────────────────────────────
function BannedBackground() {
    return (
        <>
            <div className="banned-bg__grid" />
            <div className="banned-bg__glow" />
        </>
    );
}

// ─── Icon ─────────────────────────────────────────────────────────────────────
function BannedIcon() {
    return (
        <div className="banned-icon__wrap">
            <span className="banned-icon__symbol">🔒</span>
        </div>
    );
}

// ─── Status Badge ─────────────────────────────────────────────────────────────
function BannedStatusBadge() {
    return (
        <div className="banned-status-badge">
            <span className="banned-status-badge__dot" />
            АКАУНТЪТ Е БЛОКИРАН
        </div>
    );
}

// ─── Heading ──────────────────────────────────────────────────────────────────
function BannedHeading() {
    return (
        <>
            <h1 className="banned-heading">
                ДОСТЪПЪТ Е{" "}
                <span className="banned-heading__accent">ОТКАЗАН</span>
            </h1>
            <p className="banned-subheading">
                Твоят акаунт е блокиран от администратор поради нарушение на
                правилата на платформата. Ако смяташ, че това е грешка, свържи
                се с нас за преглед на случая.
            </p>
        </>
    );
}

// ─── Info Row ─────────────────────────────────────────────────────────────────
// Renders a single key → value row inside the info card.
function BannedInfoRow({ label, value, valueStyle }) {
    return (
        <div className="banned-info-row">
            <span className="banned-info-row__key">{label}</span>
            <span className={`banned-info-row__value${valueStyle ? ` banned-info-row__value--${valueStyle}` : ""}`}>
                {value}
            </span>
        </div>
    );
}

// ─── Info Card ────────────────────────────────────────────────────────────────
// Shows account status details in a terminal-style card.
function BannedInfoCard() {
    return (
        <div className="banned-card">
            <div className="banned-card__header">
                <span className="banned-card__header-dot banned-card__header-dot--red" />
                <span className="banned-card__header-dot banned-card__header-dot--yellow" />
                <span className="banned-card__header-dot banned-card__header-dot--green" />
                <span className="banned-card__header-label">account_status.log</span>
            </div>
            <div>
                <BannedInfoRow
                    label="СТАТУС"
                    value="БЛОКИРАН"
                    valueStyle="rose"
                />
                <BannedInfoRow
                    label="ДОСТЪП ДО ПЛАТФОРМАТА"
                    value="ЗАБРАНЕН"
                    valueStyle="rose"
                />
                <BannedInfoRow
                    label="ПРИЧИНА"
                    value="Нарушение на правилата"
                />
                <BannedInfoRow
                    label="ОБЖАЛВАНЕ"
                    value="support@cryptomind.bg"
                />
                <BannedInfoRow
                    label="РЕШЕНИЕ"
                    value="Само от администратор"
                    valueStyle="dim"
                />
            </div>
        </div>
    );
}

// ─── Actions ──────────────────────────────────────────────────────────────────
function BannedActions() {
    return (
        <></>
        // <div className="banned-actions">
        //     <a href="mailto:support@cryptomind.bg" className="banned-actions__btn-primary">
        //         Свържи се с нас
        //     </a>
        //     <a href="/" className="banned-actions__btn-ghost">
        //         Начална страница
        //     </a>
        // </div>
    );
}

// ─── Footer Note ──────────────────────────────────────────────────────────────
function BannedFooterNote() {
    return (
        <></>
        // <p className="banned-footer-note">
        //     Смяташ, че е грешка? Изпрати имейл на{" "}
        //     <a href="mailto:support@cryptomind.bg" className="banned-footer-note__link">
        //         support@cryptomind.bg
        //     </a>
        //     {" "}с темата „Обжалване на блокиране".
        // </p>
    );
}

// ─── Page Root ────────────────────────────────────────────────────────────────
export default function BannedPage() {
    return (
        <div className="banned-page">
            <BannedBackground />
            <div className="banned-inner">
                <BannedIcon />
                <BannedStatusBadge />
                <BannedHeading />
                <BannedInfoCard />
                <BannedActions />
                <BannedFooterNote />
            </div>
        </div>
    );
}