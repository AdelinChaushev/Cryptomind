import { useState, useEffect, useRef } from "react";
import "../styles/about.css";
import { useContext } from "react";
import { AuthorizationContext } from "../App.jsx";
import { Link } from "react-router-dom";
// ─── Cipher Reveal Hook ───────────────────────────────────────────────────────
function useCipherReveal(text) {
    const CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%";
    const [displayed, setDisplayed] = useState("");
    const { state ,setState} = useContext(AuthorizationContext);
    useEffect(() => {
        let frame = 0;
        const totalFrames = text.length * 4;
        const id = setInterval(() => {
            frame++;
            const revealed = Math.floor((frame / totalFrames) * text.length);
            let result = "";
            for (let i = 0; i < text.length; i++) {
                if (i < revealed) result += text[i];
                else result += CHARS[Math.floor(Math.random() * CHARS.length)];
            }
            setDisplayed(result);
            if (frame >= totalFrames) clearInterval(id);
        }, 30);
        return () => clearInterval(id);
    }, [text]);

    return displayed || text;
}

// ─── Stat Counter ─────────────────────────────────────────────────────────────
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

// ─── Section Header ───────────────────────────────────────────────────────────
function SectionHeader({ tag, title }) {
    return (
        <>
            <span className="about-section-tag">{tag}</span>
            <h2 className="about-section-title">{title}</h2>
            <div className="about-section-line" />
        </>
    );
}

// ─── Cipher Card ──────────────────────────────────────────────────────────────
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

// ─── Step Cell ────────────────────────────────────────────────────────────────
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

// ─── Limit Card ───────────────────────────────────────────────────────────────
function LimitCard({ icon, title, desc }) {
    return (
        <div className="about-limit-card">
            <div className="about-limit-card__icon">{icon}</div>
            <div>
                <div className="about-limit-card__title">{title}</div>
                <div className="about-limit-card__desc">{desc}</div>
            </div>
        </div>
    );
}

// ─── Hero ─────────────────────────────────────────────────────────────────────
function Hero() {
    const headline = useCipherReveal("CRYPTOMIND");

    return (
        <section className="about-hero">
            <div className="about-hero__grid" />
            <div className="about-hero__glow" />

            <div className="about-hero__inner">
                <div className="about-hero__badge">// ОБРАЗОВАТЕЛНА КРИПТОГРАФСКА ПЛАТФОРМА</div>

                <h1 className="about-hero__headline">{headline}</h1>

                <p className="about-hero__sub">
                    Образователна платформа за класическа криптография и криптоанализ,
                    поддържана от машинно обучение и изкуствен интелект.
                </p>

                <div className="about-hero__meta">
                    <span className="about-hero__meta-item">▸ 14 типа шифри</span>
                    <span className="about-hero__meta-divider">|</span>
                    <span className="about-hero__meta-item">▸ 90% точност</span>
                    <span className="about-hero__meta-divider">|</span>
                    <span className="about-hero__meta-item">▸ &lt;1 сек. предсказване</span>
                </div>
            </div>

            <div className="about-hero__scroll">SCROLL ↓</div>
        </section>
    );
}

// ─── Mission ──────────────────────────────────────────────────────────────────
function Mission() {
    return (
        <section className="about-section">
            <div className="about-container">
                <div className="about-mission-layout">
                    <div>
                        <SectionHeader tag="// МИСИЯ" title="Какво е CryptoMind?" />
                        <p className="about-body-text">
                            CryptoMind е образователна уеб платформа, изградена около класическата
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

                    <div className="about-terminal">
                        <div className="about-terminal__bar">
                            <span className="about-terminal__dot about-terminal__dot--red" />
                            <span className="about-terminal__dot about-terminal__dot--yellow" />
                            <span className="about-terminal__dot about-terminal__dot--green" />
                            <span className="about-terminal__filename">cryptomind_core.py</span>
                        </div>
                        <pre className="about-terminal__code">{`> Анализ на текст...
  Слой 1: Семейство — Полиазбучен  ✓
  Увереност: 99.97%

> Слой 2: Специализиран модел...
  Тип: Шифър на Виженер            ✓
  Увереност: 95.56%

> Резултат: VIGENERE
  Времe за предсказване: 0.31 с    ■`}</pre>
                    </div>
                </div>
            </div>
        </section>
    );
}

// ─── Stats ────────────────────────────────────────────────────────────────────
function Stats() {
    return (
        <section className="about-stats-section">
            <div className="about-stats-grid">
                <StatCounter value={90}    label="Обща точност"            suffix="%" />
                <StatCounter value={14}    label="Поддържани шифри"                  />
                <StatCounter value={99}    label="Точност — Слой 1"        suffix="%" />
                <StatCounter value={47668} label="Тренировъчни изречения"            />
            </div>
        </section>
    );
}

// ─── Cipher Families ──────────────────────────────────────────────────────────
function CipherFamilies() {
    const families = [
        { family: "ЗАМЕСТВАНЕ",   types: ["Цезар", "ROT13", "Atbash", "Просто заместване"], icon: "⟳", partial: false },
        { family: "ПОЛИАЗБУЧНИ",  types: ["Виженер", "Autokey", "Трифемий"],                icon: "◈", partial: false },
        { family: "ТРАНСПОЗИЦИЯ", types: ["Rail Fence", "Колонна", "Маршрутна"],            icon: "⊞", partial: true  },
        { family: "КОДИРАНЕ",     types: ["Base64", "Morse", "Binary", "Hex"],              icon: "⌬", partial: false },
    ];

    return (
        <section className="about-section about-section--alt">
            <div className="about-container">
                <SectionHeader tag="// ШИФРИ" title="Поддържани типове шифри" />
                <p className="about-body-text" style={{ maxWidth: 580, marginBottom: 12 }}>
                    Системата покрива 14 типа класически шифри в 4 семейства.
                    Задръжте курсора върху карта, за да видите точността на разпознаване.
                </p>
                <div className="about-cipher-grid">
                    {families.map((f) => <CipherCard key={f.family} {...f} />)}
                </div>
            </div>
        </section>
    );
}

// ─── How It Works ─────────────────────────────────────────────────────────────
function HowItWorks() {
    const steps = [
        { number: 1, title: "ПОДАЙ ТЕКСТ",      desc: "Въведи или качи криптиран текст (минимум 150 знака). Поддържат се текстов вход и изображения с автоматично OCR разпознаване.", delay: 0    },
        { number: 2, title: "ML КЛАСИФИКАЦИЯ",  desc: "Двуслойната невронна мрежа анализира статистическите характеристики — честоти на букви, индекс на съвпадение, биграми и ентропия.", delay: 0.15 },
        { number: 3, title: "AI АСИСТЕНТ",      desc: "По избор поискай подсказка или пълно решение от AI. Асистентът обяснява методологията, без да разкрива директно отговора.", delay: 0.3  },
        { number: 4, title: "РЕШИ И СПЕЧЕЛИ",  desc: "Изпрати своя отговор, натрупай точки и изкачи класацията. Стандартните шифри се проверяват автоматично за секунди.", delay: 0.45 },
    ];

    return (
        <section className="about-section">
            <div className="about-container">
                <SectionHeader tag="// КАК РАБОТИ" title="Процесът стъпка по стъпка" />
                <div className="about-steps-grid">
                    {steps.map((s) => <StepCell key={s.number} {...s} />)}
                </div>
            </div>
        </section>
    );
}

// ─── Limitations ──────────────────────────────────────────────────────────────
function Limitations() {
    const limits = [
        { icon: "🇬🇧", title: "САМО АНГЛИЙСКИ ТЕКСТ",   desc: "ML моделът е трениран на английски корпус. Индексът на съвпадение приема английски честоти (~0.065). Друг език ще даде неверни резултати." },
        { icon: "📏",   title: "МИНИМУМ 150 ЗНАКА",      desc: "Статистическите характеристики стават ненадеждни под тази граница. Оптималната дължина за класификация е 200–400 знака." },
        { icon: "1️⃣",  title: "САМО ЕДИН ШИФЪР",        desc: "Системата предполага, че текстът е криптиран с точно един тип шифър. Наслоеното или смесено криптиране не се поддържа." },
        { icon: "📜",   title: "САМО КЛАСИЧЕСКИ ШИФРИ",  desc: "Не е предназначена за модерни алгоритми като AES или RSA. Платформата покрива единствено исторически и образователни шифри." },
    ];

    return (
        <section className="about-section about-section--alt">
            <div className="about-container">
                <SectionHeader tag="// ОГРАНИЧЕНИЯ" title="Честност за ограниченията" />
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

// ─── Roles ────────────────────────────────────────────────────────────────────
function Roles() {
    const userPerms  = ["Разглежда и решава шифри", "Подава собствени предизвикателства", "Ползва AI подсказки и пълни решения", "Участва в класацията", "Преглежда своята история"];
    const adminPerms = ["Одобрява или отхвърля шифри", "Преглежда ML и AI анализи при проверка", "Задава тагове, трудност и тип", "Управлява и деактивира потребители", "Промотира до администратор"];

    return (
        <section className="about-section">
            <div className="about-container">
                <SectionHeader tag="// РОЛИ" title="Кой може да прави какво?" />
                <div className="about-roles-grid">
                    <div className="about-role-card">
                        <div className="about-role-card__header">
                            <span className="about-role-card__icon">👤</span>
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
                            <span className="about-role-card__icon">🛡</span>
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

// ─── CTA ──────────────────────────────────────────────────────────────────────
function CTA() {
     const { state ,setState} = useContext(AuthorizationContext);
    return (
        <section className="about-cta">
            <div className="about-cta__glow" />
            <div className="about-container">
                <p className="about-cta__tag">// ГОТОВ ЛИ СИ?</p>
                <h2 className="about-cta__title">РАЗГАДАЙ ПЪРВИЯ СИ ШИФЪР</h2>
                <p className="about-cta__sub">Историята на тайните съобщения чака да бъде открита.</p>
                <div className="about-cta__buttons">
                    {!state.isLoggedIn ? <Link to="/register" className="about-cta__btn-primary">Регистрирай се</Link>
                   : <Link to="/"  className="about-cta__btn-secondary">Разгледай шифрите</Link>}
                </div>
            </div>
        </section>
    );
}

// ─── Page Root ────────────────────────────────────────────────────────────────
export default function AboutPage() {
    return (
        <div className="about-page">
            <Hero />
            <Mission />
            {/* <Stats /> */}
            <CipherFamilies />
            <HowItWorks />
            <Limitations />
            <Roles />
            <CTA />
        </div>
    );
}