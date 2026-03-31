import React, { useRef, useEffect } from 'react';

const steps = [
    {
        num: '01',
        title: 'Разгледай шифрите',
        description: 'Разгледайте колекцията ни от класически шифри. Филтрирайте по трудност, вид или популярност, за да намерите идеалното предизвикателство.'
    },
    {
        num: '02',
        title: 'Реши или предложи',
        description: 'Разбийте съществуващи шифри или предложете свои криптирани послания за решаване от другите. Всеки шифър е нова загадка.'
    },
    {
        num: '03',
        title: 'Спечели точки',
        description: 'Натрупвайте точки за верни решения и се изкачвайте в класацията. Колкото по-трудни шифри решавате, толкова повече точки печелите.'
    },
    {
        num: '04',
        title: 'Учи с AI',
        description: 'Затруднихте се? Получете AI-подсказки или пълни решения стъпка по стъпка, задвижвани от напреднали езикови модели.'
    }
];

const HowItWorks = () => {
    const threadRef = useRef(null);

    useEffect(() => {
        const items = threadRef.current?.querySelectorAll('.step-item');
        if (!items) return;

        const observer = new IntersectionObserver(
            (entries) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('step-item--visible');
                        observer.unobserve(entry.target);
                    }
                });
            },
            { threshold: 0.2 }
        );

        items.forEach((item) => observer.observe(item));
        return () => observer.disconnect();
    }, []);

    return (
        <section className="home-section">
            <div className="home-wrap">
                <div className="eyebrow">
                    <span className="eyebrow-text">Как работи</span>
                </div>

                <div className="steps-thread" ref={threadRef}>
                    {steps.map((step, i) => (
                        <div
                            className="step-item"
                            key={step.num}
                            style={{ '--delay': `${i * 0.12}s` }}
                        >
                            <div className="step-item-body">
                                <span className="step-big-num" aria-hidden="true">
                                    {step.num}
                                </span>
                                <div className="step-item-content">
                                    <h3 className="step-item-title">{step.title}</h3>
                                    <p className="step-item-desc">{step.description}</p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </section>
    );
};

export default HowItWorks;
