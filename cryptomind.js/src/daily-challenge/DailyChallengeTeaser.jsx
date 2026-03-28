import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { getTodaysChallenge } from './dailyChallengeService';
import './DailyChallengeTeaser.css';

export default function DailyChallengeTeaser() {
    const [challenge, setChallenge] = useState(null);

    useEffect(() => {
        getTodaysChallenge()
            .then(setChallenge)
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
