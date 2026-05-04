import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useError } from '../ErrorContext';
import '../styles/secret-reveal.css';

const API_BASE = import.meta.env.VITE_API_URL;
const REVEAL_ENDPOINT = `${API_BASE}/api/user/7f1a3b82-9e4d-4c5a-b2f1-6d8e9a0c3f4b`;

function SecretRevealPage() {
    const navigate = useNavigate();
    const { setError } = useError();
    const [status, setStatus] = useState('idle');
    const [errorMessage, setErrorMessage] = useState('');
    const [alreadyClaimed, setAlreadyClaimed] = useState(false);

    useEffect(() => {
        axios.get(`${API_BASE}/api/user/get-account-info`, { withCredentials: true })
            .then((res) => {
                const badges = res.data?.badges ?? [];
                const hasSecretBadge = badges.some(b => b.id === 16);
                if (hasSecretBadge) {
                    setAlreadyClaimed(true);
                }
            })
            .catch(() => {
            });
    }, []);

    const handleReveal = async () => {
        if (status === 'loading' || status === 'success' || alreadyClaimed) return;

        setStatus('loading');
        setErrorMessage('');

        try {
            await axios.put(REVEAL_ENDPOINT, {}, { withCredentials: true });
            setStatus('success');
        } catch (err) {
            const status = err.response?.status;
            const message = err.response?.data?.error || err.response?.data?.title;

            if (status === 409) {
                setAlreadyClaimed(true);
                setErrorMessage(message || 'Вече сте получили наградата от разкриването на тайната.');
                setStatus('already');
            } else if (status === 401) {
                setErrorMessage('Трябва да сте вписани, за да заявите наградата.');
                setStatus('error');
                setError('Трябва да сте вписани, за да заявите наградата.');
                navigate('/login');
            } else {
                const fallback = message || 'Възникна грешка при заявяване на наградата.';
                setErrorMessage(fallback);
                setStatus('error');
                setError(fallback);
            }
        }
    };

    const renderContent = () => {
        if (status === 'success') {
            return (
                <div className="secret-result secret-result--success">
                    <div className="secret-badge-glow">
                        <img
                            src="/Images/Badges/Badge_16.png"
                            alt="Светеща тайна"
                            className="secret-badge-image"
                            onError={(e) => { e.target.style.display = 'none'; }}
                        />
                    </div>
                    <h2 className="secret-success-title">Поздравления!</h2>
                    <p className="secret-success-text">
                        Открихте скритото послание чрез ултравиолетова светлина.
                        Значката <strong>„Светеща тайна“</strong> е добавена към вашия профил.
                    </p>
                    <button className="secret-cta" onClick={() => navigate('/account-info')}>
                        Към моя акаунт
                    </button>
                </div>
            );
        }

        if (alreadyClaimed || status === 'already') {
            return (
                <div className="secret-result secret-result--claimed">
                    <div className="secret-icon">✓</div>
                    <h2 className="secret-claimed-title">Вече сте получили наградата</h2>
                    <p className="secret-claimed-text">
                        {errorMessage || 'Значката „Светеща тайна“ вече присъства във вашия профил. Не можете да я заявите повторно.'}
                    </p>
                    <button className="secret-cta secret-cta--ghost" onClick={() => navigate('/account-info')}>
                        Виж значките си
                    </button>
                </div>
            );
        }

        return (
            <div className="secret-reveal-card">
                <div className="secret-reveal-glyph">⟁</div>
                <h1 className="secret-reveal-title">Разкриване на тайната</h1>
                <p className="secret-reveal-subtitle">
                    Стигнахте до скритата страница. Натиснете бутона по-долу, за да заявите ексклузивната значка
                    <strong> „Светеща тайна“</strong>.
                </p>

                {errorMessage && status === 'error' && (
                    <div className="secret-error">
                        ⚠ {errorMessage}
                    </div>
                )}

                <button
                    className="secret-cta"
                    onClick={handleReveal}
                    disabled={status === 'loading'}
                >
                    {status === 'loading' ? 'Заявяване...' : 'Заяви значката'}
                </button>

                <p className="secret-reveal-hint">
                    Тази награда може да бъде заявена само веднъж за всеки акаунт.
                </p>
            </div>
        );
    };

    return (
        <main className="secret-reveal-page">
            <div className="secret-reveal-shell">
                {renderContent()}
            </div>
        </main>
    );
}

export default SecretRevealPage;
