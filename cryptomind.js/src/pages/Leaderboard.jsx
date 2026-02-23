import { useEffect, useState } from "react";
import "../styles/leaderboard.css";

const API_URL = "http://localhost:5115/api/leaderboard";

const MEDALS = ["🥇", "🥈", "🥉"];
const RANK_CLASSES = ["rank-1", "rank-2", "rank-3"];

function getInitial(username) {
    if (!username) return "?";
    return username.charAt(0).toUpperCase();
}

function PodiumCard({ entry }) {
    const rankIndex = entry.place - 1;
    return (
        <div className={`lb-podium-card ${RANK_CLASSES[rankIndex]}`}>
            <span className="lb-podium-crown">{MEDALS[rankIndex]}</span>
            <span className="lb-podium-rank-num">#{entry.place}</span>
            <div className="lb-podium-avatar">{getInitial(entry.username)}</div>
            <p className="lb-podium-username">{entry.username}</p>
            <p className="lb-podium-points">{entry.points.toLocaleString()}</p>
            <span className="lb-podium-pts-label">PTS</span>
        </div>
    );
}

function LeaderboardRow({ entry }) {
    return (
        <li className="lb-row">
            <span className="lb-row-rank">#{entry.place}</span>
            <div className="lb-row-user">
                <div className="lb-row-avatar">{getInitial(entry.username)}</div>
                <span className="lb-row-username">{entry.username}</span>
            </div>
            <span className="lb-row-points">{entry.points.toLocaleString()}</span>
        </li>
    );
}

function StatusBar({ count, status }) {
    const dotClass = status === "success" ? "online" : status === "error" ? "error" : "";
    const label =
        status === "loading" ? "FETCHING DATA..." :
        status === "success" ? `${count} CRYPTOANALYSTS RANKED` :
        "CONNECTION FAILED";

    return (
        <div className="lb-status">
            <span>{label}</span>
            <span className={`lb-status-dot ${dotClass}`} />
        </div>
    );
}

function EmptyState({ message }) {
    return (
        <div className="lb-empty">
            <span className="lb-empty-icon">📡</span>
            {message}
        </div>
    );
}

export default function Leaderboard() {
    const [entries, setEntries] = useState([]);
    const [status, setStatus]   = useState("loading");
    const [errorMsg, setErrorMsg] = useState("");

    useEffect(() => {
        async function fetchLeaderboard() {
            try {
                const response = await fetch(API_URL);
                if (!response.ok) throw new Error(`HTTP ${response.status}`);
                const data = await response.json();
                setEntries(data);
                setStatus("success");
            } catch (err) {
                setErrorMsg(err.message);
                setStatus("error");
            }
        }
        fetchLeaderboard();
    }, []);

    const top3 = entries.filter((e) => e.place <= 3);
    const rest  = entries.filter((e) => e.place > 3);

    // Classic podium order: 2nd | 1st | 3rd
    const podiumOrder = [
        top3.find((e) => e.place === 2),
        top3.find((e) => e.place === 1),
        top3.find((e) => e.place === 3),
    ].filter(Boolean);

    return (
        <div className="leaderboard-page">
            <div className="lb-grid-bg" />

            <div className="lb-container">

                <header className="lb-header">
                    <div className="lb-header-decoration">
                        <span className="lb-bracket">[</span>
                        <span className="lb-header-label">GLOBAL_RANKINGS</span>
                        <span className="lb-bracket">]</span>
                    </div>
                    <h1 className="lb-title">LEADERBOARD</h1>
                    <p className="lb-subtitle">Top cryptoanalysts ranked by decoded ciphers</p>
                </header>

                {status === "error" ? (
                    <EmptyState message={`FAILED TO LOAD — ${errorMsg}`} />
                ) : (
                    <>
                        <div className="lb-podium">
                            {podiumOrder.map((entry) => (
                                <PodiumCard key={entry.place} entry={entry} />
                            ))}
                        </div>

                        <div className="lb-table">
                            <div className="lb-table-header">
                                <span>RANK</span>
                                <span>CRYPTOANALYST</span>
                                <span className="lb-col-pts">POINTS</span>
                            </div>

                            <ul className="lb-list">
                                {rest.length === 0 && status === "success" ? (
                                    <EmptyState message="NO ADDITIONAL ENTRIES" />
                                ) : (
                                    rest.map((entry) => (
                                        <LeaderboardRow key={entry.place} entry={entry} />
                                    ))
                                )}
                            </ul>
                        </div>
                    </>
                )}

                <StatusBar count={entries.length} status={status} />

            </div>
        </div>
    );
}