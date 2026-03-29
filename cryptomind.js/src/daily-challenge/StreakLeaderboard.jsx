import { useState, useEffect } from 'react';
import '../styles/streak-leaderboard.css';

const API = import.meta.env.VITE_API_URL;

function getInitial(username) {
    return username ? username.charAt(0).toUpperCase() : '?';
}

export default function StreakLeaderboard() {
    const [entries, setEntries] = useState([]);
    const [status, setStatus] = useState('loading');

    useEffect(() => {
        fetch(`${API}/api/leaderboard/streaks`)
            .then(r => {
                if (!r.ok) throw new Error();
                return r.json();
            })
            .then(data => { setEntries(data); setStatus('ready'); })
            .catch(() => setStatus('error'));
    }, []);

    if (status === 'loading' || status === 'error' || entries.length === 0) return null;

    const rankClass = (place) => {
        if (place === 1) return 'gold';
        if (place === 2) return 'silver';
        if (place === 3) return 'bronze';
        return '';
    };

    return (
        <div className="slb-section">
            <div className="slb-header">
                <span className="slb-label">[ СЕРИЯ ]</span>
                <span className="slb-title">КЛАСАЦИЯ НА СЕРИИ</span>
            </div>

            <div className="slb-table-header">
                <span>#</span>
                <span>ПОТРЕБИТЕЛ</span>
                <span className="slb-col-streak">🔥 СЕРИЯ</span>
            </div>

            <ul className="slb-list">
                {entries.map(entry => (
                    <li key={entry.place} className="slb-row">
                        <span className={`slb-rank ${rankClass(entry.place)}`}>
                            {entry.place <= 3
                                ? ['🥇', '🥈', '🥉'][entry.place - 1]
                                : `#${entry.place}`}
                        </span>
                        <div className="slb-user">
                            <div className="slb-avatar">{getInitial(entry.username)}</div>
                            <span className="slb-username">{entry.username}</span>
                        </div>
                        <span className="slb-streak">{entry.points}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
}
