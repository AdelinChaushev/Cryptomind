import React, { useEffect, useRef } from 'react';
import StepNode from './StepNode';

const steps = [
    {
        number: 1,
        title: 'Разгледай шифрите',
        description: 'Разгледайте колекцията ни от класически шифри. Филтрирайте по трудност, вид или популярност, за да намерите идеалното предизвикателство.'
    },
    {
        number: 2,
        title: 'Реши или предложи',
        description: 'Разбийте съществуващи шифри или предложете свои криптирани послания за решаване от другите. Всеки шифър е нова загадка.'
    },
    {
        number: 3,
        title: 'Спечели точки',
        description: 'Натрупвайте точки за верни решения и се изкачвайте в класацията. Колкото по-трудни шифри решавате, толкова повече точки печелите.'
    },
    {
        number: 4,
        title: 'Учи с AI',
        description: 'Затруднихте се? Получете AI-подсказки или пълни решения.'
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

const HowItWorks = () => {
    const headerRef = useRef(null);
    useDecodeOnScroll(headerRef, 'Как работи');

    return (
        <section className="how-it-works">
            <div className="section-header">
                <h2 ref={headerRef}>Как работи</h2>
                <p>Започнете да решавате в четири прости стъпки</p>
            </div>
            <div className="steps-timeline">
                {steps.map((step, index) => (
                    <StepNode
                        key={step.number}
                        number={step.number}
                        title={step.title}
                        description={step.description}
                        isLast={index === steps.length - 1}
                    />
                ))}
            </div>
        </section>
    );
};

export default HowItWorks;