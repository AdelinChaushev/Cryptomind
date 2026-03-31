import React, { useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';

const CHARS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#@$%&+=÷×≠≈∞∑π';
const COLS = 18;
const ROWS = 14;
const TOTAL = COLS * ROWS;

const CipherMatrix = () => {
    const containerRef = useRef(null);

    useEffect(() => {
        const container = containerRef.current;
        if (!container) return;

        const spans = [];
        for (let i = 0; i < TOTAL; i++) {
            const span = document.createElement('span');
            span.className = 'cm-char';
            span.textContent = CHARS[Math.floor(Math.random() * CHARS.length)];
            container.appendChild(span);
            spans.push(span);
        }

        const tick = setInterval(() => {
            const updates = Math.floor(TOTAL * 0.04);
            for (let i = 0; i < updates; i++) {
                const idx = Math.floor(Math.random() * TOTAL);
                const span = spans[idx];
                span.textContent = CHARS[Math.floor(Math.random() * CHARS.length)];

                const roll = Math.random();
                if (roll > 0.85) {
                    span.classList.add('cm-bright');
                    setTimeout(() => span.classList.remove('cm-bright'), 220);
                } else if (roll > 0.6) {
                    span.classList.add('cm-lit');
                    setTimeout(() => span.classList.remove('cm-lit'), 400);
                }
            }
        }, 90);

        return () => {
            clearInterval(tick);
            container.innerHTML = '';
        };
    }, []);

    return <div ref={containerRef} className="cipher-matrix" aria-hidden="true" />;
};

const Hero = () => {
    return (
        <section className="hero">
            <div className="hero-badge">
                <span className="hero-badge-dot" />
                Cryptomind&nbsp;/&nbsp;Криптография
            </div>

            <div className="hero-body">
                <div className="hero-text">
                    <h1 className="hero-headline">
                        Декодирайте<br />
                        <em>миналото,</em><br />
                        овладейте<br />
                        криптографията
                    </h1>

                    <p className="hero-sub">
                        Предизвикайте себе си с класически шифри, учете се от
                        AI&#8209;подсказки и се състезавайте с&nbsp;други
                        криптографи. 14&nbsp;вида шифри, разкрийте тайните
                        на&nbsp;криптирането.
                    </p>

                    <div className="hero-buttons">
                        <Link to="/register" className="btn btn-primary">
                            Започни да&nbsp;решаваш
                            <span className="btn-arrow">→</span>
                        </Link>
                        <Link to="/cipher-library" className="btn btn-secondary">
                            Научи повече
                        </Link>
                    </div>
                </div>

                <div className="hero-cipher-panel">
                    <CipherMatrix />
                </div>
            </div>
            
            <div className="hero-stats">
                <div className="hero-stat">
                    <span className="hero-stat-num">14</span>
                    <span className="hero-stat-label">Вида шифри</span>
                </div>
                <div className="hero-stat-sep" />
                <div className="hero-stat">
                    <span className="hero-stat-num">AI</span>
                    <span className="hero-stat-label">Подсказки & решения</span>
                </div>
                <div className="hero-stat-sep" />
                <div className="hero-stat">
                    <span className="hero-stat-num">Daily</span>
                    <span className="hero-stat-label">Ежедневно предизвикателство</span>
                </div>
                <div className="hero-stat-sep" />
                <div className="hero-stat">
                    <span className="hero-stat-num">1v1</span>
                    <span className="hero-stat-label">Реално състезание</span>
                </div>
            </div>
        </section>
    );
};

export default Hero;
