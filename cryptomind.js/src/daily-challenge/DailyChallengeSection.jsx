import { useState, useEffect, useCallback } from 'react';
import { getTodaysChallenge, submitAnswer } from './dailyChallengeService';
import './DailyChallengeSection.css';

function useCountdown() {
    const [timeLeft, setTimeLeft] = useState('');

    useEffect(() => {
        const tick = () => {
            const now = new Date();
            const midnight = new Date(Date.UTC(
                now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate() + 1
            ));
            const diff = midnight - now;
            const h = String(Math.floor(diff / 3600000)).padStart(2, '0');
            const m = String(Math.floor((diff % 3600000) / 60000)).padStart(2, '0');
            const s = String(Math.floor((diff % 60000) / 1000)).padStart(2, '0');
            setTimeLeft(`${h}:${m}:${s}`);
        };
        tick();
        const id = setInterval(tick, 1000);
        return () => clearInterval(id);
    }, []);

    return timeLeft;
}

function Skeleton() {
    return (
        <div className="dc-skeleton">
            <div className="dc-skeleton-line" style={{ width: '45%', height: '12px' }} />
            <div className="dc-skeleton-line" style={{ width: '100%', height: '80px', marginTop: '16px' }} />
            <div className="dc-skeleton-line" style={{ width: '70%', height: '36px', marginTop: '12px' }} />
        </div>
    );
}

export default function DailyChallengeSection() {
    const [challenge, setChallenge] = useState(null);
    const [status, setStatus] = useState('loading');
    const [answer, setAnswer] = useState('');
    const [lastResult, setLastResult] = useState(null);
    const [submitting, setSubmitting] = useState(false);
    const countdown = useCountdown();

    const load = useCallback(async () => {
        try {
            const data = await getTodaysChallenge();
            setChallenge(data);
            setStatus('ready');
        } catch {
            setStatus('error');
        }
    }, []);

    useEffect(() => { load(); }, [load]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!answer.trim() || submitting) return;

        setSubmitting(true);
        setLastResult(null);
        try {
            const result = await submitAnswer(answer.trim());
            setLastResult(result);
            if (result.isCorrect) {
                setChallenge(prev => ({
                    ...prev,
                    alreadySolvedToday: true,
                    userCurrentStreak: result.newStreak,
                }));
                setAnswer('');
            } else {
                setChallenge(prev => ({
                    ...prev,
                    attemptCount: (prev.attemptCount ?? 0) + 1,
                }));
            }
        } catch {
            setLastResult({ error: true });
        } finally {
            setSubmitting(false);
        }
    };

    if (status === 'loading') return <Skeleton />;
    if (status === 'error') return null;

    const solved = challenge.alreadySolvedToday;

    return (
        <div className="dc-section">
            <div className="dc-header">
                <div className="dc-title-row">
                    <span className="dc-label">[ ДНЕВНО ]</span>
                    <span className="dc-title">ПРЕДИЗВИКАТЕЛСТВО</span>
                </div>
                {challenge.userCurrentStreak > 0 && (
                    <div className="dc-streak-badge">
                        🔥 {challenge.userCurrentStreak} дни поред
                    </div>
                )}
            </div>

            <div className="dc-countdown">
                Следващо предизвикателство след: {countdown}
            </div>

            <div className="dc-cipher-box">
                {challenge.encryptedText}
            </div>

            {solved ? (
                <div className="dc-solved-banner">
                    <span className="dc-solved-icon">✓</span>
                    <div className="dc-solved-text">
                        Решено днес!
                        {lastResult?.correctAnswer && (
                            <strong>Отговор: {lastResult.correctAnswer}</strong>
                        )}
                        {!lastResult?.correctAnswer && (
                            <strong>Серия: 🔥 {challenge.userCurrentStreak}</strong>
                        )}
                    </div>
                </div>
            ) : (
                <>
                    {lastResult?.isCorrect === false && !lastResult?.error && (
                        <div className="dc-wrong-banner">
                            ✗ Грешен отговор — опитай пак
                        </div>
                    )}
                    <form className="dc-form" onSubmit={handleSubmit}>
                        <input
                            className="dc-input"
                            type="text"
                            placeholder="Въведи дешифрирания текст..."
                            value={answer}
                            onChange={e => setAnswer(e.target.value)}
                            disabled={submitting}
                        />
                        <button
                            className="dc-submit-btn"
                            type="submit"
                            disabled={submitting || !answer.trim()}
                        >
                            {submitting ? '...' : 'ПРОВЕРИ'}
                        </button>
                    </form>
                    <div className="dc-meta">
                        Опити днес: {challenge.attemptCount ?? 0}
                    </div>
                </>
            )}
        </div>
    );
}
