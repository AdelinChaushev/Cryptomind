import React, { useEffect, useRef } from 'react';
import StepNode from './StepNode';

const steps = [
    {
        number: 1,
        title: 'Browse Ciphers',
        description: 'Explore our collection of classical ciphers. Filter by difficulty, type, or popularity to find your perfect challenge.'
    },
    {
        number: 2,
        title: 'Solve or Submit',
        description: 'Crack existing ciphers or submit your own encrypted messages for others to solve. Every cipher is a new puzzle.'
    },
    {
        number: 3,
        title: 'Earn Points',
        description: 'Gain points for correct solutions and climb the leaderboard. The more challenging the cipher, the more points you earn.'
    },
    {
        number: 4,
        title: 'Learn with AI',
        description: 'Stuck? Get AI-powered hints or full solutions. Our ML system identifies cipher types and guides your learning.'
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
    useDecodeOnScroll(headerRef, 'How It Works');

    return (
        <section className="how-it-works">
            <div className="section-header">
                <h2 ref={headerRef}>How It Works</h2>
                <p>Start solving in four simple steps</p>
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
