const Footer = () => {
    return (
        <footer className="footer">
            <div className="footer-container">
                <div className="footer-brand">
                    <h3>CRYPTOMIND</h3>
                    <p>
                        An educational cryptography platform combining machine learning with classical
                        cryptanalysis. Identify ciphers, solve challenges, and learn the art of cryptanalysis.
                    </p>
                </div>
                <div className="footer-column">
                    <h4>Platform</h4>
                    <ul className="footer-links">
                        <li><a href="/browse">Browse Ciphers</a></li>
                        <li><a href="/leaderboard">Leaderboard</a></li>
                        <li><a href="/tools">Tools</a></li>
                        <li><a href="/submit">Submit Cipher</a></li>
                    </ul>
                </div>
                <div className="footer-column">
                    <h4>Learn</h4>
                    <ul className="footer-links">
                        <li><a href="/about">About</a></li>
                        <li><a href="/how-it-works">How It Works</a></li>
                    </ul>
                </div>               
            </div>
            <div className="footer-bottom">
                &copy; 2025 CryptoMind. Educational use only.
            </div>
        </footer>
    );
};

export default Footer;
