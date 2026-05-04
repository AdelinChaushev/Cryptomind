import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useError } from '../ErrorContext';
import '../styles/secret-reveal.css';
import badgeImg from '../../../Images/Badges/Badge_16.png';

const API_BASE = import.meta.env.VITE_API_URL;
const REVEAL_ENDPOINT = `${API_BASE}/api/user/7f1a3b82-9e4d-4c5a-b2f1-6d8e9a0c3f4b`;

function SecretRevealPage() {
    const navigate = useNavigate();
    const { setError } = useError();
    const [status, setStatus] = useState('success');
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
            .catch(() => { });
    }, []);

    const handleReveal = async () => {
        if (status === 'loading' || status === 'success' || alreadyClaimed) return;

        setStatus('loading');
        setErrorMessage('');

        try {
            await axios.put(REVEAL_ENDPOINT, {}, { withCredentials: true });
            setStatus('success');
        } catch (err) {
            const code = err.response?.status;
            const message = err.response?.data?.error || err.response?.data?.title;

            if (code === 409) {
                setAlreadyClaimed(true);
                setErrorMessage(message || 'Вече сте получили наградата от разкриването на тайната.');
                setStatus('already');
            } else if (code === 401) {
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

    /* Shared floating badge scene used in success & already-claimed states */
    const FloatingBadge = () => (
        <div className="secret-badge-scene">
            {/* Ambient sparks orbiting the badge */}
            <span className="badge-spark" />
            <span className="badge-spark" />
            <span className="badge-spark" />
            <span className="badge-spark" />
            <span className="badge-spark" />
            <span className="badge-spark" />

            {/* Spinning conic rings */}
            <div className="badge-ring" />
            <div className="badge-ring-inner" />

            {/* The actual badge image */}
            <img
                src="../../../Images/Badges/Badge_16.png"
                alt="UV Uncovered – Secrets Found"
                className="secret-badge-image"
                onError={(e) => { e.target.style.display = 'none'; }}
            />

            {/* Ground shadow */}
            <div className="badge-shadow" />
        </div>
    );

    const renderPanelBody = () => {
        if (status === 'success') {
            return (
                <div className="secret-state secret-state--success">
                    <FloatingBadge />
                    <h2 className="secret-state-title">Поздравления!</h2>
                    <p className="secret-state-text">
                        Открихте скритото послание чрез ултравиолетова светлина.
                        Значката <strong>„Светеща тайна"</strong> вече е във вашия профил.
                    </p>
                    <button className="btn-secret-primary" onClick={() => navigate('/account-info')}>
                        Към моя акаунт
                    </button>
                </div>
            );
        }

        if (alreadyClaimed || status === 'already') {
            return (
                <div className="secret-state secret-state--claimed">
                    <FloatingBadge />
                    <h2 className="secret-state-title">Вече сте получили наградата</h2>
                    <p className="secret-state-text">
                        {errorMessage || 'Значката „Светеща тайна" вече присъства във вашия профил. Не можете да я заявите повторно.'}
                    </p>
                    <button className="btn-secret-ghost" onClick={() => navigate('/account-info')}>
                        Виж значките си
                    </button>
                </div>
            );
        }

        return (
            <div className="secret-state">
                <div className="secret-glyph">
                    <span className="secret-glyph-mark">✶</span>
                </div>
                <h2 className="secret-state-title">Заяви ексклузивната значка</h2>
                <p className="secret-state-text">
                    Натиснете бутона по-долу, за да добавите значката
                    <strong> „Светеща тайна"</strong> към своя профил.
                </p>

                {errorMessage && status === 'error' && (
                    <div className="secret-error-box">
                        <span className="secret-error-icon">⚠</span>
                        <span>{errorMessage}</span>
                    </div>
                )}

                <button
                    className="btn-secret-primary"
                    onClick={handleReveal}
                    disabled={status === 'loading'}
                >
                    {status === 'loading' ? 'Заявяване...' : 'Заяви значката'}
                </button>

                <p className="secret-hint">
                    Тази награда може да бъде заявена само веднъж за всеки акаунт.
                </p>
            </div>
        );
    };

    return (
        <div className="secret-page">
            <div className="secret-page-header">
                <div className="secret-breadcrumb">
                    <span>Начало</span>
                    <span className="breadcrumb-sep">/</span>
                    <span className="breadcrumb-current">Разкрита тайна</span>
                </div>
                <h1 className="secret-page-title">
                    Разкриване на <span>тайната</span>
                </h1>
                <p className="secret-page-subtitle">
                    Стигнахте до скритата страница. Заявете ексклузивната значка „Luminous Secret" за своя профил.
                </p>
            </div>

            <div className="secret-layout">
                <section className="secret-panel">
                    <header className="secret-panel-header">
                        <span className="secret-panel-icon">⟁</span>
                        <span className="secret-panel-title">Скрита награда</span>
                    </header>
                    <div className="secret-panel-body">
                        {renderPanelBody()}
                    </div>
                </section>
            </div>
        </div>
    );
}

export default SecretRevealPage;