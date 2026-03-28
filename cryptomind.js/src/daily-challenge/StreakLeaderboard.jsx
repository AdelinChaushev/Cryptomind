import { useState, useEffect } from 'react';
import { getStreakLeaderboard } from './dailyChallengeService';
import './StreakLeaderboard.css';

function getInitial(username) {
    return username ? username.charAt(0).toUpperCase() : '?';
}

export default function StreakLeaderboard() {
    const [entries, setEntries] = useState([]);
    const [status, setStatus] = useState('loading');

    useEffect(() => {
        getStreakLeaderboard()
            .then(data => { setEntries(data); setStatus('ready'); })
            .catch(() => setStatus('error'));
    }, []);

    if (status === 'loading' || status === 'error' || entries.length === 0) return null;

    return (
        <div className="slb-section">
            <div className="slb-header">
                <span className="slb-label">[ СЕРИЯ ]</span>
                <span className="slb-title">КЛАСАЦИЯ НА СЕРИИ</span>
            </div>

            <ul className="slb-list">
                {entries.map(entry => (
                    <li key={entry.place} className="slb-row">
                        <span className={`slb-rank ${entry.place <= 3 ? 'top' : ''}`}>
                            {entry.place <= 3
                                ? ['🥇', '🥈', '🥉'][entry.place - 1]
                                : `#${entry.place}`}
                        </span>
                        <div className="slb-user">
                            <div className="slb-avatar">{getInitial(entry.username)}</div>
                            <span className="slb-username">{entry.username}</span>
                        </div>
                        <span className="slb-streak">🔥 {entry.currentStreak}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
}
