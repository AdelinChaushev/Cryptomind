import { useState, useEffect, useRef } from "react";
import "../styles/about.css";
import { useContext } from "react";
import { AuthorizationContext } from "../App.jsx";
import { Link } from "react-router-dom";

const SCRAMBLE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%";

// ── Cipher reveal on mount (hero headline) ───────────────────────
function useCipherReveal(text) {
    const [displayed, setDisplayed] = useState("");
    const { state, setState } = useContext(AuthorizationContext);
    useEffect(() => {
        let frame = 0;
        const totalFrames = text.length * 4;
        const id = setInterval(() => {
            frame++;
            const revealed = Math.floor((frame / totalFrames) * text.length);
            let result = "";
            for (let i = 0; i < text.length; i++) {
                if (i < revealed) result += text[i];
                else result += SCRAMBLE_CHARS[Math.floor(Math.random() * SCRAMBLE_CHARS.length)];
            }
            setDisplayed(result);
            if (frame >= totalFrames) clearInterval(id);
        }, 30);
        return () => clearInterval(id);
    }, [text]);
    return displayed || text;
}

// ── Cipher reveal triggered by scroll intersection ───────────────
function useScrollCipherReveal(text) {
    const [displayed, setDisplayed] = useState(text);
    const [triggered, setTriggered] = useState(false);
    const ref = useRef(null);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting && !triggered) {
                    setTriggered(true);
                    observer.disconnect();
                }
            },
            { threshold: 0.4 }
        );
        if (ref.current) observer.observe(ref.current);
        return () => observer.disconnect();
    }, [triggered]);

    useEffect(() => {
        if (!triggered) return;
        let frame = 0;
        const totalFrames = text.length * 1.4;
        const id = setInterval(() => {
            frame++;
            const revealed = Math.floor((frame / totalFrames) * text.length);
            let result = "";
            for (let i = 0; i < text.length; i++) {
                if (i < revealed) result += text[i];
                else result += SCRAMBLE_CHARS[Math.floor(Math.random() * SCRAMBLE_CHARS.length)];
            }
            setDisplayed(result);
            if (frame >= totalFrames) { setDisplayed(text); clearInterval(id); }
        }, 16);
        return () => clearInterval(id);
    }, [triggered, text]);

    return { displayed, ref };
}

// ── Hero background cipher stream ────────────────────────────────
const BG_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%÷×≠≈∑π";
const BG_COLS = 34;
const BG_ROWS = 20;
const BG_TOTAL = BG_COLS * BG_ROWS;

const HeroCipherBg = () => {
    const ref = useRef(null);

    useEffect(() => {
        const container = ref.current;
        if (!container) return;
        const spans = [];
        for (let i = 0; i < BG_TOTAL; i++) {
            const span = document.createElement("span");
            span.className = "about-hero-bg-char";
            span.textContent = BG_CHARS[Math.floor(Math.random() * BG_CHARS.length)];
            container.appendChild(span);
            spans.push(span);
        }
        const tick = setInterval(() => {
            const n = Math.floor(BG_TOTAL * 0.025);
            for (let i = 0; i < n; i++) {
                const idx = Math.floor(Math.random() * BG_TOTAL);
                spans[idx].textContent = BG_CHARS[Math.floor(Math.random() * BG_CHARS.length)];
                const roll = Math.random();
                if (roll > 0.93) {
                    spans[idx].classList.add("about-hero-bg-char--bright");
                    setTimeout(() => spans[idx].classList.remove("about-hero-bg-char--bright"), 280);
                } else if (roll > 0.76) {
                    spans[idx].classList.add("about-hero-bg-char--lit");
                    setTimeout(() => spans[idx].classList.remove("about-hero-bg-char--lit"), 480);
                }
            }
        }, 110);
        return () => { clearInterval(tick); container.innerHTML = ""; };
    }, []);

    return <div ref={ref} className="about-hero-bg-stream" aria-hidden="true" />;
};

// ── Typewriter terminal ───────────────────────────────────────────
const TERMINAL_LINES = [
    "> Анализ на текст...",
    "  Слой 1: Семейство — Полиазбучен  ✓",
    "  Увереност: 99.97%",
    "",
    "> Слой 2: Специализиран модел...",
    "  Тип: Шифър на Виженер (Vigenere)           ✓",
    "  Увереност: 95.56%",
    "",
    "> Резултат: VIGENERE",
    "  Времe за предсказване: 0.31 с    ■",
];
const FULL_TERMINAL_TEXT = TERMINAL_LINES.join("\n");

function TypewriterTerminal() {
    const [text, setText] = useState("");
    const [done, setDone] = useState(false);
    const [started, setStarted] = useState(false);
    const termRef = useRef(null);
    const timeoutRef = useRef(null);
    const idxRef = useRef(0);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => { if (entry.isIntersecting) { setStarted(true); observer.disconnect(); } },
            { threshold: 0.25 }
        );
        if (termRef.current) observer.observe(termRef.current);
        return () => observer.disconnect();
    }, []);

    useEffect(() => {
        if (!started) return;
        const type = () => {
            const idx = idxRef.current;
            if (idx > FULL_TERMINAL_TEXT.length) { setDone(true); return; }
            setText(FULL_TERMINAL_TEXT.slice(0, idx));
            idxRef.current = idx + 1;
            const ch = FULL_TERMINAL_TEXT[idx];
            let delay = 16;
            if (!ch || ch === "\n") delay = 90;
            else if (ch === " ") delay = 7;
            timeoutRef.current = setTimeout(type, delay);
        };
        timeoutRef.current = setTimeout(type, 420);
        return () => clearTimeout(timeoutRef.current);
    }, [started]);

    return (
        <div className="about-terminal" ref={termRef}>
            <div className="about-terminal__bar">
                <span className="about-terminal__dot about-terminal__dot--red" />
                <span className="about-terminal__dot about-terminal__dot--yellow" />
                <span className="about-terminal__dot about-terminal__dot--green" />
                <span className="about-terminal__filename">cryptomind_core.py</span>
            </div>
            <pre className="about-terminal__code">
                {text}{!done && <span className="about-terminal__cursor">▋</span>}
            </pre>
        </div>
    );
}


function StatCounter({ value, label, suffix = "" }) {
    const [count, setCount] = useState(0);
    const ref = useRef(null);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => {
                if (!entry.isIntersecting) return;
                let current = 0;
                const step = value / 60;
                const timer = setInterval(() => {
                    current += step;
                    if (current >= value) { setCount(value); clearInterval(timer); }
                    else setCount(Math.floor(current));
                }, 20);
                observer.disconnect();
            },
            { threshold: 0.5 }
        );
        if (ref.current) observer.observe(ref.current);
        return () => observer.disconnect();
    }, [value]);

    return (
        <div ref={ref} className="about-stat">
            <span className="about-stat__number">{count}{suffix}</span>
            <span className="about-stat__label">{label}</span>
        </div>
    );
}


function SectionHeader({ tag, title }) {
    const { displayed, ref } = useScrollCipherReveal(title);
    return (
        <>
            <div className="about-eyebrow">
                <span className="about-eyebrow-text">{tag}</span>
            </div>
            <h2 className="about-section-title" ref={ref}>{displayed}</h2>
            <div className="about-section-line" />
        </>
    );
}


function CipherCard({ family, types, accuracy, icon, partial }) {
    const [hovered, setHovered] = useState(false);

    return (
        <div
            className="about-cipher-card"
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
        >
            <div className="about-cipher-card__top">
                <span className="about-cipher-card__icon">{icon}</span>
            </div>
            <h3 className="about-cipher-card__family">{family}</h3>
            <div className="about-cipher-card__types">
                {types.map((t) => (
                    <span key={t} className="about-cipher-type-pill">{t}</span>
                ))}
            </div>
            <div className="about-cipher-card__bar-track">
                <div
                    className={`about-cipher-card__bar-fill${partial ? " about-cipher-card__bar-fill--partial" : ""}`}
                    style={{ width: hovered ? accuracy : "0%" }}
                />
            </div>
        </div>
    );
}


function StepCell({ number, title, desc, delay }) {
    const [visible, setVisible] = useState(false);
    const ref = useRef(null);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([e]) => { if (e.isIntersecting) { setVisible(true); observer.disconnect(); } },
            { threshold: 0.2 }
        );
        if (ref.current) observer.observe(ref.current);
        return () => observer.disconnect();
    }, []);

    return (
        <div
            ref={ref}
            className={`about-step ${visible ? "about-step--visible" : "about-step--hidden"}`}
            style={{ transitionDelay: `${delay}s` }}
        >
            <div className="about-step__number">0{number}</div>
            <div>
                <h3 className="about-step__title">{title}</h3>
                <p className="about-step__desc">{desc}</p>
            </div>
        </div>
    );
}


const IconGlobe = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <circle cx="12" cy="12" r="9" />
        <path d="M12 3c-2.3 3.7-2.3 14.3 0 18" />
        <path d="M12 3c2.3 3.7 2.3 14.3 0 18" />
        <path d="M3 12h18" />
    </svg>
);

const IconRuler = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M3 17L17 3l4 4L7 21z" />
        <path d="M7.5 12.5l1.5 1.5" />
        <path d="M11 9l1.5 1.5" />
        <path d="M14.5 5.5l1.5 1.5" />
    </svg>
);

const IconLock = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <rect x="5" y="11" width="14" height="10" rx="1" />
        <path d="M8 11V7a4 4 0 018 0v4" />
    </svg>
);

const IconScroll = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M4 6h16M4 10h12M4 14h10M4 18h7" />
    </svg>
);

const IconUser = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <circle cx="12" cy="8" r="4" />
        <path d="M4 20c0-4.4 3.6-8 8-8s8 3.6 8 8" />
    </svg>
);

const IconShield = () => (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 3L4 7v5c0 5.2 3.6 9.5 8 10.5C16.4 21.5 20 17.2 20 12V7z" />
    </svg>
);


function LimitCard({ iconEl, title, desc }) {
    return (
        <div className="about-limit-card">
            <div className="about-limit-card__icon-wrap">
                {iconEl}
            </div>
            <div>
                <div className="about-limit-card__title">{title}</div>
                <div className="about-limit-card__desc">{desc}</div>
            </div>
        </div>
    );
}


// ── Hero with cipher bg + mouse parallax ─────────────────────────
function Hero() {
    const headline = useCipherReveal("CRYPTOMIND");
    const heroRef = useRef(null);
    const glowRef = useRef(null);
    const gridRef = useRef(null);

    useEffect(() => {
        const hero = heroRef.current;
        if (!hero) return;

        const onMove = (e) => {
            const rect = hero.getBoundingClientRect();
            const dx = (e.clientX - (rect.left + rect.width  / 2)) / (rect.width  / 2);
            const dy = (e.clientY - (rect.top  + rect.height / 2)) / (rect.height / 2);
            if (glowRef.current)
                glowRef.current.style.transform = `translate(calc(-50% + ${dx * 28}px), calc(-50% + ${dy * 18}px))`;
            if (gridRef.current)
                gridRef.current.style.transform = `translate(${dx * -10}px, ${dy * -7}px)`;
        };

        const onLeave = () => {
            if (glowRef.current) glowRef.current.style.transform = "translate(-50%, -50%)";
            if (gridRef.current) gridRef.current.style.transform = "";
        };

        hero.addEventListener("mousemove", onMove);
        hero.addEventListener("mouseleave", onLeave);
        return () => {
            hero.removeEventListener("mousemove", onMove);
            hero.removeEventListener("mouseleave", onLeave);
        };
    }, []);

    return (
        <section className="about-hero" ref={heroRef}>
            <div className="about-hero__grid" ref={gridRef} />
            <HeroCipherBg />
            <div className="about-hero__glow" ref={glowRef} />

            <div className="about-hero__inner">
                <div className="about-hero__badge">
                    <span className="about-hero__badge-dot" />
                    ОБРАЗОВАТЕЛНА КРИПТОГРАФСКА ПЛАТФОРМА
                </div>

                <h1 className="about-hero__headline">{headline}</h1>

                <p className="about-hero__sub">
                    Образователна платформа за класическа криптография и криптоанализ,
                    поддържана от машинно обучение и изкуствен интелект.
                </p>

                <div className="about-hero__meta">
                    <span className="about-hero__meta-item">▸ 14 типа шифри</span>
                    <span className="about-hero__meta-divider">|</span>
                    <span className="about-hero__meta-item">▸ 90% ML точност</span>
                    <span className="about-hero__meta-divider">|</span>
                    <span className="about-hero__meta-item">▸ AI асистент</span>
                </div>
            </div>

            <div className="about-hero__scroll">SCROLL ↓</div>
        </section>
    );
}

function Mission() {
    return (
        <section className="about-section">
            <div className="about-container">
                <div className="about-mission-layout">
                    <div>
                        <SectionHeader tag="МИСИЯ" title="Какво е Cryptomind?" />
                        <p className="about-body-text">
                            Cryptomind е образователна уеб платформа, изградена около класическата
                            криптография. Потребителите взаимодействат с криптирани съобщения, опитват
                            се да ги разгадаят и научават как работят историческите шифри — всичко
                            подкрепено от ML класификатор и LLM асистент.
                        </p>
                        <p className="about-body-text">
                            Платформата е проектирана за начинаещи и ентусиасти, които искат да
                            разберат криптоанализа по практичен и ангажиращ начин. Тя не разбива
                            шифри — вместо това отговаря на по-фундаментален въпрос:
                        </p>
                        <blockquote className="about-mission-quote">
                            „Какъв тип шифър е вероятно използван?"
                        </blockquote>
                    </div>

                    <TypewriterTerminal />
                </div>
            </div>
        </section>
    );
}


function CipherFamilies() {
    const families = [
        { family: "ЗАМЕСТВАНЕ",   types: ["Цезар (Caesar)", "ROT13 (ROT13)", "Атбаш (Atbash)", "Просто замяна (Simple Substitution)"], icon: "⟳", partial: false },
        { family: "ПОЛИАЗБУЧНИ",  types: ["Виженер (Vigenere)", "Автоключ (Autokey)", "Тритемий (Trithemius)"],                        icon: "◈", partial: false },
        { family: "ТРАНСПОЗИЦИЯ", types: ["Железопътна ограда (RailFence)", "Колонна (Columnar)", "Маршрут (Route)"],                   icon: "⊞", partial: false },
        { family: "КОДИРАНЕ",     types: ["Base64 (Base64)", "Морзов (Morse)", "Двоичен (Binary)", "Шестнадесетичен (Hex)"],            icon: "⌬", partial: false },
    ];

    return (
        <section className="about-section about-section--alt">
            <div className="about-container">
                <SectionHeader tag="ШИФРИ" title="Поддържани типове шифри" />
                <p className="about-body-text" style={{ maxWidth: 580, marginBottom: 12 }}>
                    Системата покрива 14 типа класически шифри в 4 семейства.
                </p>
                <div className="about-cipher-grid">
                    {families.map((f) => <CipherCard key={f.family} {...f} />)}
                </div>
            </div>
        </section>
    );
}


function HowItWorks() {
    const steps = [
        { number: 1, title: "ПОДАЙ ТЕКСТ",     desc: "Въведи или качи криптиран текст (минимум 150 знака). Поддържат се текстов вход и изображения с автоматично OCR разпознаване.", delay: 0    },
        { number: 2, title: "ML КЛАСИФИКАЦИЯ", desc: "Двуслойната невронна мрежа анализира статистическите характеристики — честоти на букви, индекс на съвпадение, биграми и ентропия.", delay: 0.15 },
        { number: 3, title: "AI АСИСТЕНТ",     desc: "По избор поискай подсказка или пълно решение от AI. Асистентът обяснява методологията, без да разкрива директно отговора.", delay: 0.3  },
        { number: 4, title: "РЕШИ И СПЕЧЕЛИ", desc: "Изпрати своя отговор, натрупай точки и изкачи класацията. Стандартните шифри се проверяват автоматично за секунди.", delay: 0.45 },
    ];

    return (
        <section className="about-section">
            <div className="about-container">
                <SectionHeader tag="КАК РАБОТИ" title="Процесът стъпка по стъпка" />
                <div className="about-steps-grid">
                    {steps.map((s) => <StepCell key={s.number} {...s} />)}
                </div>
            </div>
        </section>
    );
}


function Limitations() {
    const limits = [
        { iconEl: <IconGlobe />,  title: "САМО АНГЛИЙСКИ ТЕКСТ",    desc: "ML моделът е трениран на английски корпус. Индексът на съвпадение приема английски честоти (~0.065). Друг език ще даде неверни резултати." },
        { iconEl: <IconRuler />,  title: "ОПТИМАЛНО 200–400 ЗНАКА", desc: "За най-добра точност на класификацията препоръчваме текст между 200 и 400 знака. Под 150 знака статистиката става ненадеждна и резултатите често са неточни." },
        { iconEl: <IconLock />,   title: "ЕДНО НИВО НА ШИФРОВАНЕ",  desc: "Приложено е само едно ниво на криптиране. Няма допълнителни слоеве или двойно шифроване." },
        { iconEl: <IconScroll />, title: "САМО КЛАСИЧЕСКИ ШИФРИ",   desc: "Не е предназначена за модерни алгоритми като AES или RSA. Платформата покрива единствено исторически и образователни шифри." },
    ];

    return (
        <section className="about-section about-section--alt">
            <div className="about-container">
                <SectionHeader tag="ОГРАНИЧЕНИЯ" title="Честност за ограниченията" />
                <p className="about-body-text about-limits-intro">
                    Ограниченията не са недостатъци — те са образователен инструмент. Всяко
                    отразява реално предизвикателство в криптоанализа, с което се сблъскват
                    дори специалистите.
                </p>
                <div className="about-limits-grid">
                    {limits.map((l) => <LimitCard key={l.title} {...l} />)}
                </div>
            </div>
        </section>
    );
}

function Roles() {
    const userPerms  = ["Разглежда и решава шифри", "Подава собствени предизвикателства", "Ползва AI подсказки и пълни решения", "Участва в класацията", "Преглежда своята история"];
    const adminPerms = ["Одобрява или отхвърля шифри", "Одобрява или отхвърля предложени отговори", "Преглежда ML и AI анализи при проверка", "Управлява и деактивира потребители", "Промотира до администратор"];

    return (
        <section className="about-section">
            <div className="about-container">
                <SectionHeader tag="РОЛИ" title="Кой може да прави какво?" />
                <div className="about-roles-grid">
                    <div className="about-role-card">
                        <div className="about-role-card__header">
                            <div className="about-role-card__icon-wrap">
                                <IconUser />
                            </div>
                            <h3 className="about-role-card__name">ПОТРЕБИТЕЛ</h3>
                        </div>
                        <ul className="about-role-card__list">
                            {userPerms.map((p) => (
                                <li key={p} className="about-role-card__item">
                                    <span className="about-role-card__bullet">▸</span> {p}
                                </li>
                            ))}
                        </ul>
                    </div>

                    <div className="about-role-card about-role-card--admin">
                        <div className="about-role-card__header">
                            <div className="about-role-card__icon-wrap about-role-card__icon-wrap--admin">
                                <IconShield />
                            </div>
                            <h3 className="about-role-card__name">АДМИНИСТРАТОР</h3>
                        </div>
                        <ul className="about-role-card__list">
                            {adminPerms.map((p) => (
                                <li key={p} className="about-role-card__item">
                                    <span className="about-role-card__bullet">▸</span> {p}
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>
            </div>
        </section>
    );
}


function CTA() {
    const { state, setState } = useContext(AuthorizationContext);
    return (
        <section className="about-cta">
            <div className="about-cta__glow" />
            <div className="about-container">
                <p className="about-cta__tag">ГОТОВ ЛИ СИ</p>
                <h2 className="about-cta__title">РАЗГАДАЙ ПЪРВИЯ СИ ШИФЪР</h2>
                <p className="about-cta__sub">Историята на тайните съобщения чака да бъде открита.</p>
                <div className="about-cta__buttons">
                    {!state.isLoggedIn
                        ? <Link to="/register" className="about-cta__btn-primary">Регистрирай се <span className="about-cta__btn-arrow">→</span></Link>
                        : <Link to="/" className="about-cta__btn-secondary">Разгледай шифрите</Link>
                    }
                </div>
            </div>
        </section>
    );
}

export default function AboutPage() {
    return (
        <div className="about-page">
            <Hero />
            <Mission />
            <CipherFamilies />
            <HowItWorks />
            <Limitations />
            <Roles />
            <CTA />
        </div>
    );
}
