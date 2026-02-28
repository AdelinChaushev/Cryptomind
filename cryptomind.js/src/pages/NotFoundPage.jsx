import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/not-found.css";
import { useContext } from "react";
import { AuthorizationContext } from "../App.jsx";
// ─── Typing Effect ────────────────────────────────────────────────────────────
function useTyping(text, delay = 0) {
    const [displayed, setDisplayed] = useState("");

    useEffect(() => {
        let timeout;
        let index = 0;

        const start = () => {
            const tick = () => {
                setDisplayed(text.slice(0, index + 1));
                index++;
                if (index < text.length) timeout = setTimeout(tick, 35);
            };
            timeout = setTimeout(tick, delay);
        };

        start();
        return () => clearTimeout(timeout);
    }, [text, delay]);

    return displayed;
}

// ─── Background ───────────────────────────────────────────────────────────────
function NfBackground() {
    return (
        <>
            <div className="nf-bg__grid" />
            <div className="nf-bg__glow-center" />
            <div className="nf-bg__glow-left" />
        </>
    );
}

// ─── Terminal Line ────────────────────────────────────────────────────────────
function NfTerminalLine({ prompt = "$", text, type = "cmd", show }) {
    if (!show) return null;
    return (
        <div className="nf-terminal__line">
            <span className="nf-terminal__prompt">{prompt}</span>
            <span className={`nf-terminal__${type}`}>{text}</span>
        </div>
    );
}

// ─── Terminal ─────────────────────────────────────────────────────────────────
function NfTerminal() {
    const [step, setStep] = useState(0);

    useEffect(() => {
        const timers = [
            setTimeout(() => setStep(1), 400),
            setTimeout(() => setStep(2), 1100),
            setTimeout(() => setStep(3), 1900),
            setTimeout(() => setStep(4), 2600),
            setTimeout(() => setStep(5), 3400),
        ];
        return () => timers.forEach(clearTimeout);
    }, []);

    const line1 = useTyping("GET /mystery-page HTTP/1.1",      400);
    const line2 = useTyping("Търсене на маршрут...",           1100);
    const line3 = useTyping("ГРЕШКА: Маршрутът не е намерен", 1900);
    const line4 = useTyping("Препоръка: Върни се на началото", 2600);

    return (
        <div className="nf-terminal">
            <div className="nf-terminal__bar">
                <span className="nf-terminal__dot nf-terminal__dot--red" />
                <span className="nf-terminal__dot nf-terminal__dot--yellow" />
                <span className="nf-terminal__dot nf-terminal__dot--green" />
                <span className="nf-terminal__label">cryptomind_router.sh</span>
            </div>
            <div className="nf-terminal__body">
                <NfTerminalLine prompt="$" text={line1} type="cmd"   show={step >= 1} />
                <NfTerminalLine prompt=">" text={line2} type="warn"  show={step >= 2} />
                <NfTerminalLine prompt="!" text={line3} type="error" show={step >= 3} />
                <NfTerminalLine prompt=">" text={line4} type="ok"    show={step >= 4} />
                {step >= 5 && (
                    <div className="nf-terminal__line">
                        <span className="nf-terminal__prompt">$</span>
                        <span className="nf-terminal__cmd">
                            _<span className="nf-terminal__cursor" />
                        </span>
                    </div>
                )}
            </div>
        </div>
    );
}

// ─── Actions ──────────────────────────────────────────────────────────────────
function NfActions() {
    const navigate = useNavigate();
    const { state,setState } = useContext(AuthorizationContext);
    return (
        <div className="nf-actions">
            <button className="nf-actions__btn-primary" onClick={() => navigate("/")}>
                Начална страница
            </button>
            <button className="nf-actions__btn-ghost" onClick={() => navigate(-1)}>
                Назад
            </button>
                {state.isLoggedIn && !state.roles.includes("Admin") && (
            <Link to="/" className="nf-actions__btn-ghost">
                Разгледай шифрите
            </Link>)}
        </div>
    );
}

// ─── Page Root ────────────────────────────────────────────────────────────────
export default function NotFoundPage() {
    return (
        <div className="not-found-page">
            <NfBackground />
            <div className="nf-inner">
                <div className="nf-badge">// ГРЕШКА — СТРАНИЦАТА НЕ Е НАМЕРЕНА</div>
                <div className="nf-code">404</div>
                <div className="nf-glitch-line">
                    0x45 52 52 4F 52 — PAGE_NOT_FOUND — 0x45 52 52 4F 52
                </div>
                <h1 className="nf-title">СТРАНИЦАТА Е КРИПТИРАНА… ИЛИ НЕСЪЩЕСТВУВАЩА</h1>
                <p className="nf-subtitle">
                    Маршрутът, който търсиш, не съществува в системата.
                    Може би URL адресът е сбъркан или страницата е преместена.
                </p>
                <NfTerminal />
                <NfActions />
            </div>
        </div>
    );
}