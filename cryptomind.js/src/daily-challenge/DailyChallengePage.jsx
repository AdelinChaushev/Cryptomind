import DailyChallengeSection from './DailyChallengeSection';
import StreakLeaderboard from './StreakLeaderboard';
import './DailyChallengePage.css';

export default function DailyChallengePage() {
    return (
        <div className="dcp-page">
            <div className="dcp-container">
                <div className="dcp-header">
                    <div className="dcp-header-decoration">
                        <span className="dcp-bracket">[</span>
                        <span className="dcp-header-label">ДНЕВНО_ПРЕДИЗВИКАТЕЛСТВО</span>
                        <span className="dcp-bracket">]</span>
                    </div>
                    <h1 className="dcp-title">ДНЕВНО ПРЕДИЗВИКАТЕЛСТВО</h1>
                    <p className="dcp-subtitle">Един шифър на ден. Реши го и поддържай серията си.</p>
                </div>

                <div className="dcp-layout">
                    <DailyChallengeSection />
                    <StreakLeaderboard />
                </div>
            </div>
        </div>
    );
}
