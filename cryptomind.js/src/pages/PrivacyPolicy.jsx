import { useEffect } from "react";
import "../styles/legal.css";

const SECTIONS = [
    { id: "intro", tag: "01", eyebrow: "ВЪВЕДЕНИЕ", title: "Кои сме ние" },
    { id: "data-we-collect", tag: "02", eyebrow: "ДАННИ", title: "Какви данни събираме" },
    { id: "how-we-use", tag: "03", eyebrow: "ЦЕЛИ", title: "Как използваме данните" },
    { id: "cookies", tag: "04", eyebrow: "БИСКВИТКИ", title: "Бисквитки и сесии" },
    { id: "ai-content", tag: "05", eyebrow: "AI И ML", title: "Обработка от ML и AI асистента" },
    { id: "sharing", tag: "06", eyebrow: "СПОДЕЛЯНЕ", title: "Споделяне с трети страни" },
    { id: "retention", tag: "07", eyebrow: "СЪХРАНЕНИЕ", title: "Срок на съхранение" },
    { id: "rights", tag: "08", eyebrow: "ПРАВА", title: "Вашите права" },
    { id: "security", tag: "09", eyebrow: "СИГУРНОСТ", title: "Сигурност на данните" },
    { id: "changes", tag: "10", eyebrow: "ПРОМЕНИ", title: "Промени в политиката" },
];

export default function PrivacyPolicy() {
    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    return (
        <div className="legal-page">
            <section className="legal-hero">
                <div className="legal-hero__grid" />
                <div className="legal-hero__glow" />
                <div className="legal-hero__inner">
                    <div className="legal-hero__badge">
                        <span className="legal-hero__badge-dot" />
                        ПРАВНА ИНФОРМАЦИЯ
                    </div>
                    <h1 className="legal-hero__title">ПОЛИТИКА ЗА ПОВЕРИТЕЛНОСТ</h1>
                    <p className="legal-hero__sub">
                        Описваме какви данни събираме, защо ги обработваме и какъв контрол
                        имате върху своята информация в Cryptomind.
                    </p>
                    <div className="legal-hero__meta">
                        <span className="legal-hero__meta-dim">ПОСЛЕДНО АКТУАЛИЗИРАНЕ</span>
                        <span>25 АПРИЛ 2026</span>
                    </div>
                </div>
            </section>

            <section className="legal-body">
                <div className="legal-container">
                    <nav className="legal-toc" aria-label="Съдържание">
                        <h2 className="legal-toc__heading">// Съдържание</h2>
                        <ul className="legal-toc__list">
                            {SECTIONS.map((s) => (
                                <li key={s.id} className="legal-toc__item">
                                    <a href={`#${s.id}`}>
                                        <span className="legal-toc__num">{s.tag}</span>
                                        <span>{s.title}</span>
                                    </a>
                                </li>
                            ))}
                        </ul>
                    </nav>

                    <article id="intro" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[0].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[0].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Cryptomind е <strong>образователна платформа</strong> за класическа криптография
                            и криптоанализ. Тази политика обяснява как обработваме лични данни на
                            нашите потребители в съответствие с приложимото законодателство, включително
                            Общия регламент относно защитата на данните (GDPR).
                        </p>
                        <p className="legal-text">
                            Платформата е създадена с образователна цел и не предлага комерсиални услуги.
                            Събираме минимално необходимия обем данни, за да осигурим функционалността на
                            акаунта, класацията и инструментите за анализ на шифри.
                        </p>
                    </article>

                    <article id="data-we-collect" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[1].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[1].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            При използване на Cryptomind можем да обработваме следните категории данни:
                        </p>
                        <ul className="legal-list">
                            <li className="legal-list__item">
                                <strong>Данни за акаунт:</strong> потребителско име, имейл адрес и
                                криптирана (хеширана) парола.
                            </li>
                            <li className="legal-list__item">
                                <strong>Данни за активност:</strong> решени шифри, подадени отговори,
                                спечелени точки, серии (streaks) и постижения.
                            </li>
                            <li className="legal-list__item">
                                <strong>Подаден от вас текст:</strong> криптирани съобщения, които
                                изпращате към инструмента или подавате за одобрение.
                            </li>
                            <li className="legal-list__item">
                                <strong>Технически данни:</strong> IP адрес, тип браузър и сесийни
                                идентификатори, необходими за функционирането на услугата.
                            </li>
                            <li className="legal-list__item">
                                <strong>Известия:</strong> история на нотификациите, свързани с вашата
                                активност в платформата.
                            </li>
                        </ul>
                    </article>

                    <article id="how-we-use" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[2].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[2].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">Обработваме данните ви, за да:</p>
                        <ul className="legal-list">
                            <li className="legal-list__item">осигурим достъп до вашия акаунт и поддържаме сесията ви;</li>
                            <li className="legal-list__item">записваме напредъка ви и поддържаме класацията;</li>
                            <li className="legal-list__item">проверяваме подадените отговори и подадените от вас шифри;</li>
                            <li className="legal-list__item">предоставяме ML класификация и AI подсказки върху текста, който подавате;</li>
                            <li className="legal-list__item">изпращаме известия в платформата за свързани с акаунта събития;</li>
                            <li className="legal-list__item">защитаваме платформата срещу злоупотреба и нарушения на правилата.</li>
                        </ul>
                        <div className="legal-callout">
                            <strong>Правно основание</strong>
                            Обработваме данните ви на основание сключения с вас потребителски договор
                            (Условия за ползване), вашето съгласие при регистрация и нашия легитимен
                            интерес да поддържаме сигурна и работеща платформа.
                        </div>
                    </article>

                    <article id="cookies" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[3].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[3].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Използваме <strong>сесийни бисквитки</strong> за поддържане на влизането ви
                            в системата. Тези бисквитки са технически необходими и не се използват за
                            рекламни или маркетингови цели.
                        </p>
                        <p className="legal-text">
                            Не използваме рекламни мрежи и не споделяме данни за вашето поведение
                            с трети страни за маркетинг.
                        </p>
                    </article>

                    <article id="ai-content" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[4].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[4].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Когато подавате текст към инструмента за анализ, той се изпраща към нашите
                            ML модели и (при изрична заявка) към LLM асистент. Препоръчваме ви да
                            <strong> не подавате чувствителна или поверителна информация</strong> —
                            платформата е предназначена за класически образователни шифри.
                        </p>
                        <p className="legal-text">
                            Анонимизирани метаданни от анализа (дължина на текста, тип на предсказан
                            шифър, увереност) могат да бъдат запазвани с цел подобряване на моделите.
                        </p>
                    </article>

                    <article id="sharing" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[5].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[5].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            <strong>Не продаваме</strong> вашите лични данни. Споделяне се случва само
                            в следните ограничени случаи:
                        </p>
                        <ul className="legal-list">
                            <li className="legal-list__item">с доставчици на инфраструктура (хостинг, бази данни), които действат по наши инструкции;</li>
                            <li className="legal-list__item">с доставчик на LLM услуга, когато вие изрично заявите AI подсказка или решение;</li>
                            <li className="legal-list__item">когато това се изисква от закон, съдебно решение или регулаторен орган.</li>
                        </ul>
                    </article>

                    <article id="retention" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[6].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[6].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Данните за акаунта ви се съхраняват, докато съществува акаунтът. При изтриване
                            на акаунта премахваме личните ви идентификатори, но можем да запазим
                            анонимизирани записи (например резултати в класацията или одобрени шифри),
                            които вече са станали част от публичното съдържание на платформата.
                        </p>
                    </article>

                    <article id="rights" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[7].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[7].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">Според приложимото законодателство имате право да:</p>
                        <ul className="legal-list">
                            <li className="legal-list__item">поискате достъп до личните си данни;</li>
                            <li className="legal-list__item">коригирате неточна или непълна информация;</li>
                            <li className="legal-list__item">поискате изтриване на акаунта си („право да бъдете забравен");</li>
                            <li className="legal-list__item">възразите срещу определени видове обработка;</li>
                            <li className="legal-list__item">подадете жалба до съответния надзорен орган за защита на данните.</li>
                        </ul>
                    </article>

                    <article id="security" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[8].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[8].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Прилагаме технически и организационни мерки за защита на данните ви —
                            хеширане на паролите, криптирани сесии и контрол на достъпа. Въпреки това,
                            нито една система не е напълно неуязвима; препоръчваме използване на силна
                            и уникална парола.
                        </p>
                    </article>

                    <article id="changes" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[9].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[9].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Можем да актуализираме тази политика, за да отразява промени в платформата
                            или законодателството. Датата на последна актуализация е посочена в горната
                            част на страницата. Препоръчваме периодично да я преглеждате.
                        </p>
                    </article>
                </div>
            </section>
        </div>
    );
}
