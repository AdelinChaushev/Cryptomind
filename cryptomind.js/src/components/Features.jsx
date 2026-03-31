import React, { useRef, useEffect } from 'react';
import FeatureCard from './FeatureCard';

const features = [
    {
        index: '01',
        title: 'Изпрати свой шифър',
        description: 'Качи снимка или въведи текст — системата автоматично разпознава текста и го добавя към общността за решаване.'
    },
    {
        index: '02',
        title: 'AI подсказки',
        description: 'Получете интелигентни подсказки или пълни стъпка по стъпка решения, задвижвани от напреднали езикови модели.'
    },
    {
        index: '03',
        title: 'Образователни инструменти',
        description: 'Научете как работи всеки шифър с интерактивни примери и подробни обяснения за всяка техника.'
    },
    {
        index: '04',
        title: 'Конкурентна класация',
        description: 'Съревновавайте се с криптоанализатори. Следете класирането, точките и процента си на успех.'
    }
];

const Features = () => {
    const eyebrowRef = useRef(null);

    useEffect(() => {
        const el = eyebrowRef.current;
        if (!el) return;

        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting) {
                    el.style.opacity = '1';
                    el.style.transform = 'translateY(0)';
                    observer.disconnect();
                }
            },
            { threshold: 0.4 }
        );

        el.style.opacity = '0';
        el.style.transform = 'translateY(16px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);

        return () => observer.disconnect();
    }, []);

    return (
        <section className="home-section features-section">
            <div className="home-wrap">
                <div className="eyebrow" ref={eyebrowRef}>
                    <span className="eyebrow-text">Функции</span>
                </div>

                <div className="features-grid">
                    {features.map((feature) => (
                        <FeatureCard key={feature.index} {...feature} />
                    ))}
                </div>
            </div>
        </section>
    );
};

export default Features;
