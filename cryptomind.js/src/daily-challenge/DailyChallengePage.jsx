import DailyChallengeSection from './DailyChallengeSection';
import StreakLeaderboard from './StreakLeaderboard';
import '../styles/daily-challenge-page.css';

export default function DailyChallengePage() {
    return (
        <div className="dcp-page">
            <div className="dcp-grid-bg" />
            <div className="dcp-container">
                <div className="dcp-header">
                    <div className="dcp-eyebrow">
                        <span className="dcp-eyebrow-text">Дневно предизвикателство</span>
                    </div>
                    <h1 className="dcp-title">Един шифър, <em>всеки ден.</em></h1>
                    <p className="dcp-subtitle">Реши го и поддържай серията си.</p>
                </div>

                <div className="dcp-layout">
                    <DailyChallengeSection />
                    <StreakLeaderboard />
                </div>
            </div>
        </div>
    );
}
