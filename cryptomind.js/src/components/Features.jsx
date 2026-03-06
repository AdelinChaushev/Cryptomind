import { useEffect, useRef } from 'react';
import FeatureCard from './FeatureCard';

const features = [
    {
        icon: '📷',
        title: 'Изпрати свой шифър',
        description: 'Качи снимка или въведи текст — системата автоматично разпознава текста и го добавя към общността за решаване.'
    },
    {
        icon: '💡',
        title: 'AI подсказки и решения',
        description: 'Получете интелигентни подсказки или пълни стъпка по стъпка решения, задвижвани от напреднали езикови модели.'
    },
    {
        icon: '📚',
        title: 'Образователни инструменти',
        description: 'Научете как работи всеки шифър с интерактивни примери и подробни обяснения.'
    },
    {
        icon: '🏆',
        title: 'Конкурентна класация',
        description: 'Съревновавайте се с други криптоанализатори. Следете класирането, точките и процента си на успех.'
    }
];

const useDecodeOnScroll = (ref, originalText) => {
    useEffect(() => {
        const header = ref.current;
        if (!header) return;

        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*';

        const scrambleText = (text) => {
            return text.split('').map(char => {
                if (char === ' ') return ' ';
                return chars[Math.floor(Math.random() * chars.length)];
            }).join('');
        };

        const decodeText = (element, original, duration = 800) => {
            const totalSteps = 20;
            const stepDuration = duration / totalSteps;
            let currentStep = 0;

            element.classList.remove('encoded');
            element.classList.add('decoding');

            const interval = setInterval(() => {
                if (currentStep >= totalSteps) {
                    element.textContent = original;
                    element.classList.remove('decoding');
                    clearInterval(interval);
                    return;
                }

                const revealIndex = Math.floor((currentStep / totalSteps) * original.length);
                let decodedText = '';

                for (let i = 0; i < original.length; i++) {
                    if (i < revealIndex) {
                        decodedText += original[i];
                    } else if (original[i] === ' ') {
                        decodedText += ' ';
                    } else {
                        decodedText += chars[Math.floor(Math.random() * chars.length)];
                    }
                }

                element.textContent = decodedText;
                currentStep++;
            }, stepDuration);
        };

        const observer = new IntersectionObserver(
            (entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        decodeText(entry.target, originalText);
                        observer.unobserve(entry.target);
                    }
                });
            },
            { threshold: 0.3, rootMargin: '0px 0px -100px 0px' }
        );

        header.textContent = scrambleText(originalText);
        header.classList.add('encoded');
        observer.observe(header);

        return () => observer.disconnect();
    }, []);
};

const Features = () => {
    const headerRef = useRef(null);
    useDecodeOnScroll(headerRef, 'Функции');

    return (
        <section className="features-section">
            <div className="section-header">
                <h2 ref={headerRef}>Функции</h2>
                <p>Всичко необходимо, за да овладеете криптографията</p>
            </div>
            <div className="features-grid">
                {features.map((feature, index) => (
                    <FeatureCard key={index} {...feature} />
                ))}
            </div>
        </section>
    );
};

export default Features;