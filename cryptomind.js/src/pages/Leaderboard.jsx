import { useEffect, useState, useRef } from "react";
import "../styles/leaderboard.css";

const API_URL = `${import.meta.env.VITE_API_URL}/api/leaderboard`;

const RANK_CLASSES = ["rank-1", "rank-2", "rank-3"];
const RANK_LABELS = ["#01", "#02", "#03"];
const CIPHER_CHARS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%&+=÷×≠≈∑π';

function getInitial(username) {
  if (!username) return "?";
  return username.charAt(0).toUpperCase();
}

// Scrambles text then resolves it letter-by-letter
function useDecodeText(target, startDelay = 600) {
  const [display, setDisplay] = useState(() =>
    target.split('').map(c => c === ' ' ? ' ' : CIPHER_CHARS[Math.floor(Math.random() * CIPHER_CHARS.length)]).join('')
  );

  useEffect(() => {
    let frame;
    let startTime = null;
    const LOCK_INTERVAL = 65;

    const tick = (ts) => {
      if (!startTime) startTime = ts;
      const elapsed = ts - startTime - startDelay;

      if (elapsed < 0) {
        setDisplay(
          target.split('').map(c =>
            c === ' ' ? ' ' : CIPHER_CHARS[Math.floor(Math.random() * CIPHER_CHARS.length)]
          ).join('')
        );
        frame = requestAnimationFrame(tick);
        return;
      }

      const lockedCount = Math.floor(elapsed / LOCK_INTERVAL);

      if (lockedCount >= target.length) {
        setDisplay(target);
        return;
      }

      setDisplay(
        target.split('').map((c, i) => {
          if (i < lockedCount) return c;
          if (c === ' ') return ' ';
          return CIPHER_CHARS[Math.floor(Math.random() * CIPHER_CHARS.length)];
        }).join('')
      );

      frame = requestAnimationFrame(tick);
    };

    frame = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(frame);
  }, [target, startDelay]);

  return display;
}

// Counts up from 0 to target when element enters viewport
function useCountUp(target, duration = 1000) {
  const [count, setCount] = useState(0);
  const ref = useRef(null);
  const started = useRef(false);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    const observer = new IntersectionObserver(([entry]) => {
      if (entry.isIntersecting && !started.current) {
        started.current = true;
        observer.disconnect();

        let startTime = null;
        let frame;

        const tick = (ts) => {
          if (!startTime) startTime = ts;
          const progress = Math.min((ts - startTime) / duration, 1);
          const eased = 1 - Math.pow(1 - progress, 3);
          setCount(Math.round(eased * target));
          if (progress < 1) frame = requestAnimationFrame(tick);
        };

        frame = requestAnimationFrame(tick);
        return () => cancelAnimationFrame(frame);
      }
    }, { threshold: 0.5 });

    observer.observe(el);
    return () => observer.disconnect();
  }, [target, duration]);

  return [count, ref];
}

function AnimatedPoints({ points }) {
  const [count, ref] = useCountUp(points, 1000);
  return <span ref={ref}>{count.toLocaleString()}</span>;
}

function PodiumCard({ entry }) {
  const rankIndex = entry.place - 1;
  return (
    <div className={`lb-podium-card ${RANK_CLASSES[rankIndex]}`}>
      <span className="lb-podium-rank-num">{RANK_LABELS[rankIndex]}</span>
      <div className="lb-podium-avatar">{getInitial(entry.username)}</div>
      <p className="lb-podium-username">{entry.username}</p>
      <p className="lb-podium-points">
        <AnimatedPoints points={entry.points} />
        <span className="lb-podium-pts-label">ТЧК</span>
      </p>
      <span className="lb-podium-ghost-rank">{entry.place}</span>
      <div className="lb-podium-line" />
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

function EmptyState({ message }) {
  return <div className="lb-empty">{message}</div>;
}

export default function Leaderboard() {
  const [entries, setEntries] = useState([]);
  const [status, setStatus] = useState("loading");
  const [errorMsg, setErrorMsg] = useState("");

  const brandText = useDecodeText("Cryptomind", 800);

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
  const rest = entries.filter((e) => e.place > 3);

  const podiumOrder = [
    top3.find((e) => e.place === 2),
    top3.find((e) => e.place === 1),
    top3.find((e) => e.place === 3),
  ].filter(Boolean);

  const hasTop3 = top3.length > 0;
  const hasRest = rest.length > 0;

  return (
    <div className="leaderboard-page">
      <div className="lb-grid-bg" />

      <div className="lb-container">
        <header className="lb-header">
          <div className="lb-header-decoration">
            <span className="lb-header-label">Глобално класиране</span>
          </div>
          <h1 className="lb-title">Класация</h1>
          <p className="lb-subtitle">
            Топ криптоанализатори на{' '}
            <span className="lb-subtitle-brand">{brandText}</span>
          </p>
        </header>

        {status === "error" ? (
          <EmptyState message={`ГРЕШКА ПРИ ЗАРЕЖДАНЕ — ${errorMsg}`} />
        ) : status === "loading" ? (
          <EmptyState message="Зареждане на класацията..." />
        ) : entries.length === 0 ? (
          <EmptyState message="НЯМА НАМЕРЕНИ ПОТРЕБИТЕЛИ В СИСТЕМАТА" />
        ) : (
          <>
            {hasTop3 && (
              <div className="lb-podium">
                {podiumOrder.map((entry) => (
                  <PodiumCard key={entry.place} entry={entry} />
                ))}
              </div>
            )}

            {hasRest && (
              <div className="lb-table">
                <div className="lb-table-header">
                  <span>РАНГ</span>
                  <span>КРИПТОАНАЛИЗАТОР</span>
                  <span className="lb-col-pts">ТОЧКИ</span>
                </div>
                <ul className="lb-list">
                  {rest.map((entry) => (
                    <LeaderboardRow key={entry.place} entry={entry} />
                  ))}
                </ul>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
