import { Link } from 'react-router-dom';
import { useContext } from 'react';
import { AuthorizationContext } from '../App.jsx';

const Footer = () => {
    const { state } = useContext(AuthorizationContext);

    return (
        <footer className="footer">
            <div className="footer-inner">
                <div className="footer-container">
                    <div className="footer-brand">
                        <div className="footer-logo-row">
                            <img src="/logo.png" alt="Cryptomind" className="footer-logo-img" />
                            <span className="footer-brand-name">Cryptomind</span>
                        </div>
                        <p className="footer-brand-desc">
                            Образователна криптографска платформа, съчетаваща машинно обучение с класическа
                            криптоанализа. Идентифицирайте шифри, решавайте предизвикателства и научете изкуството на криптоанализа.
                        </p>
                    </div>

                    <div className="footer-column">
                        <h4 className="footer-col-heading">Платформа</h4>
                        <ul className="footer-links">
                            <li><Link to={state.isLoggedIn ? "/" : "/login"} onClick={() => window.scrollTo(0, 0)}>Разгледай шифрите</Link></li>
                            <li><a href="/leaderboard">Класация</a></li>
                            <li><a href="/cipher-tool">Инструмент за шифри</a></li>
                            <li><a href="/submit">Предложи шифър</a></li>
                        </ul>
                    </div>

                    <div className="footer-column">
                        <h4 className="footer-col-heading">Научи</h4>
                        <ul className="footer-links">
                            <li><a href="/about">За нас</a></li>
                        </ul>
                    </div>

                    <div className="footer-column">
                        <h4 className="footer-col-heading">Контакти</h4>
                        <ul className="footer-links footer-contact-list">
                            <li>
                                <a href="tel:+359888000001" className="footer-contact-item">
                                    <span className="footer-contact-icon">✆</span>
                                    <span>+359 89 5686840</span>
                                </a>
                            </li>
                            <li>
                                <a href="tel:+359888000002" className="footer-contact-item">
                                    <span className="footer-contact-icon">✆</span>
                                    <span>+359 87 7377718</span>
                                </a>
                            </li>
                            <li className="footer-github-row">
                                <a href="https://github.com/AdelinChaushev" target="_blank" rel="noopener noreferrer" className="footer-github-btn" aria-label="GitHub profile 1">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0 0 24 12c0-6.63-5.37-12-12-12z"/></svg>
                                </a>
                                <a href="https://github.com/s4mKa7a" target="_blank" rel="noopener noreferrer" className="footer-github-btn" aria-label="GitHub profile 2">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0 0 24 12c0-6.63-5.37-12-12-12z"/></svg>
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>

                <div className="footer-bottom">
                    <span>&copy; 2026 Cryptomind. Само за образователни цели.</span>
                </div>
            </div>
        </footer>
    );
};

export default Footer;