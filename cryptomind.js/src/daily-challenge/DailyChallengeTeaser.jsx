import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import '../styles/daily-challenge-teaser.css';

const API = import.meta.env.VITE_API_URL;

export default function DailyChallengeTeaser() {
    const [challenge, setChallenge] = useState(null);

    useEffect(() => {
        axios.get(`${API}/api/daily-challenge`, { withCredentials: true })
            .then(res => setChallenge(res.data))
            .catch(() => {});
    }, []);

    return (
        <div className="dc-teaser">
            <div className="dc-teaser-left">
                <span className="dc-teaser-icon">🔐</span>
                <div>
                    <div className="dc-teaser-label">[ ДНЕВНО ]</div>
                    <div className="dc-teaser-title">ПРЕДИЗВИКАТЕЛСТВО</div>
                    {challenge && (
                        <div className="dc-teaser-streak">
                            {challenge.alreadySolvedToday
                                ? <>✓ Решено днес &mdash; серия: <span>🔥 {challenge.userCurrentStreak}</span></>
                                : challenge.userCurrentStreak > 0
                                    ? <>Серия: <span>🔥 {challenge.userCurrentStreak} дни поред</span></>
                                    : 'Реши шифъра и започни серия'}
                        </div>
                    )}
                </div>
            </div>

            {challenge?.alreadySolvedToday ? (
                <Link to="/daily-challenge" className="dc-teaser-solved">
                    ✓ Решено днес
                </Link>
            ) : (
                <Link to="/daily-challenge" className="dc-teaser-btn">
                    Реши сега →
                </Link>
            )}
        </div>
    );
}
