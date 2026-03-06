const Footer = () => {
    return (
        <footer className="footer">
            <div className="footer-container">
                <div className="footer-brand">
                    <h3>CRYPTOMIND</h3>
                    <p>
                        Образователна криптографска платформа, съчетаваща машинно обучение с класическа
                        криптоанализа. Идентифицирайте шифри, решавайте предизвикателства и научете изкуството на криптоанализа.
                    </p>
                </div>
                <div className="footer-column">
                    <h4>Платформа</h4>
                    <ul className="footer-links">
                        <li><a href="/browse">Разгледай шифрите</a></li>
                        <li><a href="/leaderboard">Класация</a></li>
                        <li><a href="/cipher-tool">Инструмент за шифри</a></li>
                        <li><a href="/submit">Предложи шифър</a></li>
                    </ul>
                </div>
                <div className="footer-column">
                    <h4>Научи</h4>
                    <ul className="footer-links">
                        <li><a href="/about">За нас</a></li>
                    </ul>
                </div>
            </div>
            <div className="footer-bottom">
                &copy; 2026 Cryptomind. Само за образователни цели.
            </div>
        </footer>
    );
};

export default Footer;