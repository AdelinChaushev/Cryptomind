import { useEffect } from "react";
import "../styles/legal.css";

const SECTIONS = [
    { id: "acceptance", tag: "01", eyebrow: "ПРИЕМАНЕ", title: "Приемане на условията" },
    { id: "service", tag: "02", eyebrow: "УСЛУГА", title: "Описание на услугата" },
    { id: "account", tag: "03", eyebrow: "АКАУНТ", title: "Регистрация и акаунт" },
    { id: "conduct", tag: "04", eyebrow: "ПОВЕДЕНИЕ", title: "Допустимо поведение" },
    { id: "content", tag: "05", eyebrow: "СЪДЪРЖАНИЕ", title: "Подадено от потребителите съдържание" },
    { id: "ai-use", tag: "06", eyebrow: "AI И ML", title: "Използване на AI и ML функции" },
    { id: "ip", tag: "07", eyebrow: "ПРАВА", title: "Интелектуална собственост" },
    { id: "termination", tag: "08", eyebrow: "ПРЕКРАТЯВАНЕ", title: "Прекратяване и блокиране" },
    { id: "disclaimer", tag: "09", eyebrow: "ОТКАЗ", title: "Отказ от отговорност" },
    { id: "changes", tag: "10", eyebrow: "ПРОМЕНИ", title: "Промени в условията" },
];

export default function TermsOfService() {
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
                    <h1 className="legal-hero__title">УСЛОВИЯ ЗА ПОЛЗВАНЕ</h1>
                    <p className="legal-hero__sub">
                        Правилата, които уреждат отношенията между вас и Cryptomind
                        при използване на платформата за криптоанализ и образование.
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

                    <article id="acceptance" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[0].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[0].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Чрез достъпа или използването на Cryptomind вие се съгласявате да спазвате
                            настоящите Условия за ползване. Ако не сте съгласни с някоя част от тях,
                            моля, не използвайте платформата.
                        </p>
                        <p className="legal-text">
                            За да използвате платформата, трябва да сте на възраст от поне
                            <strong> 13 години</strong>. Лица под тази възраст могат да я използват
                            само под надзора на родител или законен настойник.
                        </p>
                    </article>

                    <article id="service" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[1].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[1].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Cryptomind е <strong>образователна криптографска платформа</strong>, която
                            предоставя интерактивни предизвикателства, инструменти за анализ на класически
                            шифри и AI асистент за обучение. Услугата се предоставя „както е", за
                            образователни и нетърговски цели.
                        </p>
                        <div className="legal-callout">
                            <strong>Важно</strong>
                            Платформата покрива единствено класически шифри. Тя не е предназначена
                            за криптиране на чувствителна информация и не предоставя средства за
                            модерна криптографска защита (AES, RSA и др.).
                        </div>
                    </article>

                    <article id="account" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[2].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[2].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">При регистрация се задължавате да:</p>
                        <ul className="legal-list">
                            <li className="legal-list__item">предоставяте вярна и актуална информация;</li>
                            <li className="legal-list__item">пазите паролата си в тайна и да не я споделяте с трети лица;</li>
                            <li className="legal-list__item">носите отговорност за всички действия, извършени през вашия акаунт;</li>
                            <li className="legal-list__item">ни уведомявате незабавно при подозрение за неоторизиран достъп.</li>
                        </ul>
                    </article>

                    <article id="conduct" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[3].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[3].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Когато използвате Cryptomind, се <strong>задължавате да не</strong>:
                        </p>
                        <ul className="legal-list">
                            <li className="legal-list__item">подавате незаконно, обидно, заплашително или дискриминационно съдържание;</li>
                            <li className="legal-list__item">опитвате да получите неоторизиран достъп до системи, акаунти или данни;</li>
                            <li className="legal-list__item">използвате автоматизирани средства (ботове, скриптове) за манипулиране на класацията;</li>
                            <li className="legal-list__item">претоварвате услугата или нарушавате нейното функциониране;</li>
                            <li className="legal-list__item">използвате платформата за разпространение на зловреден софтуер или фишинг;</li>
                            <li className="legal-list__item">нарушавате правата на интелектуална собственост на трети страни.</li>
                        </ul>
                    </article>

                    <article id="content" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[4].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[4].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Когато подавате шифри, отговори или други материали, гарантирате, че имате
                            право да ги споделите и че те не нарушават настоящите условия.
                        </p>
                        <p className="legal-text">
                            Запазвате собствеността върху съдържанието си, но ни предоставяте
                            <strong> неизключителен, безвъзмезден лиценз</strong> да го показваме,
                            съхраняваме и обработваме в рамките на платформата с образователна цел.
                        </p>
                        <p className="legal-text">
                            Запазваме си правото да премахваме или редактираме подадено съдържание,
                            което нарушава тези условия или което счетем за неподходящо.
                        </p>
                    </article>

                    <article id="ai-use" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[5].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[5].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            ML класификаторът и AI асистентът са образователни инструменти.
                            Резултатите им са <strong>вероятностни</strong> и могат да бъдат неточни,
                            особено при кратки текстове или нестандартни шифри.
                        </p>
                        <p className="legal-text">
                            Не подавайте чувствителна, лична или поверителна информация към инструментите.
                            Не носим отговорност за решения, взети въз основа на резултати от AI или ML.
                        </p>
                    </article>

                    <article id="ip" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[6].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[6].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Дизайнът, кодът, логата и материалите на Cryptomind са защитени от закона
                            за авторско право и са собственост на екипа на платформата. Не се разрешава
                            копиране, разпространение или модификация без писмено съгласие.
                        </p>
                    </article>

                    <article id="termination" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[7].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[7].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Запазваме си правото да <strong>спрем или прекратим</strong> достъпа ви до
                            платформата при нарушаване на настоящите условия, включително без предизвестие
                            при сериозни нарушения. Можете по всяко време да поискате изтриване на
                            акаунта си.
                        </p>
                    </article>

                    <article id="disclaimer" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[8].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[8].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Услугата се предоставя „<strong>както е</strong>" и „както е налична", без
                            каквито и да било гаранции — изрични или подразбиращи се. В максимално
                            допустимата от закона степен не носим отговорност за непреки, случайни или
                            последващи вреди, произтичащи от използването на платформата.
                        </p>
                    </article>

                    <article id="changes" className="legal-section">
                        <div className="legal-eyebrow">
                            <span className="legal-eyebrow-text">{SECTIONS[9].eyebrow}</span>
                        </div>
                        <h2 className="legal-section__title">{SECTIONS[9].title}</h2>
                        <div className="legal-section__line" />
                        <p className="legal-text">
                            Можем да актуализираме настоящите условия по всяко време. Промените влизат в
                            сила от датата на публикуване. Продължаващото използване на платформата след
                            промяна означава съгласие с новата версия.
                        </p>
                    </article>
                </div>
            </section>
        </div>
    );
}
