import "../styles/banned.css";
import { useContext } from "react";
import { AuthorizationContext } from "../App";

function BannedBackground() {
    return (
        <>
            <div className="banned-bg__grid" />
            <div className="banned-bg__glow" />
        </>
    );
}

function BannedIcon() {
    return (
        <div className="banned-icon__wrap">
            <span className="banned-icon__symbol">🔒</span>
        </div>
    );
}

function BannedStatusBadge() {
    return (
        <div className="banned-status-badge">
            <span className="banned-status-badge__dot" />
            АКАУНТЪТ Е БЛОКИРАН
        </div>
    );
}


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


function BannedInfoCard() {
    const { state,setState} = useContext(AuthorizationContext)
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
                    value={state.bannedMessage}
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
            </div>
        </div>
    );
}