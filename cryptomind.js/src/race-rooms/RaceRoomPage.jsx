import { useState, useEffect, useRef } from 'react';
import { useRaceRoom, GamePhase, CipherType } from './useRaceRooms';
import '../styles/race-room-page.css';

const CIPHER_GROUPS = [
    {
        label:     'Заместване',
        className: 'group-substitution',
        ciphers: [
            { label: 'Цезар (Caesar)',                     value: CipherType.Caesar             },
            { label: 'Атбаш (Atbash)',                     value: CipherType.Atbash             },
            { label: 'Проста замяна (SimpleSubstitution)',  value: CipherType.SimpleSubstitution },
            { label: 'ROT13 (ROT13)',                      value: CipherType.ROT13              },
        ],
    },
    {
        label:     'Полиазбучни',
        className: 'group-polyalphabetic',
        ciphers: [
            { label: 'Виженер (Vigenere)',    value: CipherType.Vigenere   },
            { label: 'Автоключ (Autokey)',    value: CipherType.Autokey    },
            { label: 'Тритемий (Trithemius)', value: CipherType.Trithemius },
        ],
    },
    {
        label:     'Транспозиция',
        className: 'group-transposition',
        ciphers: [
            { label: 'Железопътна ограда (RailFence)', value: CipherType.RailFence },
            { label: 'Колонна (Columnar)',              value: CipherType.Columnar  },
            { label: 'Маршрут (Route)',                 value: CipherType.Route     },
        ],
    },
];

function isActiveGame(phaseRef) {
    return phaseRef.current !== GamePhase.LOBBY &&
           phaseRef.current !== GamePhase.GAME_END &&
           phaseRef.current !== GamePhase.WAGER_CONFIRM;
}

function getAnchor(target) {
    let el = target;
    while (el && el.tagName !== 'A') el = el.parentElement;
    return el;
}

function isInternalLink(href) {
    if (!href) return false;
    try {
        const url = new URL(href, window.location.origin);
        return url.origin === window.location.origin;
    } catch {
        return false;
    }
}

export default function RaceRoomPage() {
    const [joinCode,         setJoinCode]         = useState('');
    const [wagerInput,       setWagerInput]        = useState(0);
    const [showLeaveConfirm, setShowLeaveConfirm]  = useState(false);
    const [pendingNavHref,   setPendingNavHref]    = useState(null);
    const beforeUnloadRef = useRef(null);

    const {
        phase,
        phaseRef,
        roomCode,
        cipherText,
        currentRound,
        roundWinner,
        gameResult,
        mySubmitted,
        otherSubmitted,
        timeLeft,
        myReady,
        otherReady,
        error,
        isConnected,
        countdownNumber,
        myUsername,
        opponentUsername,
        wagerAmount,
        myPoints,
        wagerCreatorUsername,
        roundHistory,
        createRoom,
        requestWagerInfo,
        confirmJoin,
        cancelWager,
        setReady,
        submitAnswer,
        leaveRoom,
        dismissError,
    } = useRaceRoom();

    useEffect(() => {
        if (phase === GamePhase.LOBBY) {
            setShowLeaveConfirm(false);
            setPendingNavHref(null);
        }
    }, [phase]);

    useEffect(() => {
        if (phase === GamePhase.LOBBY) setJoinCode('');
    }, [phase]);

    useEffect(() => {
        const header = document.querySelector('header');
        const footer = document.querySelector('footer');

        const shouldHide =
            phase === GamePhase.COUNTDOWN ||
            phase === GamePhase.PLAYING   ||
            phase === GamePhase.ROUND_END ||
            phase === GamePhase.GAME_END;

        if (header) header.style.display = shouldHide ? 'none' : '';
        if (footer) footer.style.display = shouldHide ? 'none' : '';

        return () => {
            if (header) header.style.display = '';
            if (footer) footer.style.display = '';
        };
    }, [phase]);

    useEffect(() => {
        const handleClick = (e) => {
            if (!isActiveGame(phaseRef)) return;
            const anchor = getAnchor(e.target);
            if (!anchor) return;
            if (!isInternalLink(anchor.href)) return;
            e.preventDefault();
            e.stopPropagation();
            setPendingNavHref(anchor.href);
            setShowLeaveConfirm(true);
        };
        document.addEventListener('click', handleClick, true);
        return () => document.removeEventListener('click', handleClick, true);
    }, [phaseRef]);

    useEffect(() => {
        const handlePopState = (e) => {
            if (!isActiveGame(phaseRef)) return;
            e.stopImmediatePropagation();
            window.history.pushState(null, '', window.location.href);
            setShowLeaveConfirm(true);
        };
        window.addEventListener('popstate', handlePopState, true);
        return () => window.removeEventListener('popstate', handlePopState, true);
    }, [phaseRef]);

    useEffect(() => {
        const handler = (e) => {
            if (!isActiveGame(phaseRef)) return;
            e.preventDefault();
            e.returnValue = '';
        };
        beforeUnloadRef.current = handler;
        window.addEventListener('beforeunload', handler);
        return () => {
            window.removeEventListener('beforeunload', handler);
            beforeUnloadRef.current = null;
        };
    }, [phaseRef]);

    const handleLeaveCancel = () => {
        setShowLeaveConfirm(false);
        setPendingNavHref(null);
    };

    const handleLeaveConfirm = async () => {
        setShowLeaveConfirm(false);
        if (beforeUnloadRef.current) {
            window.removeEventListener('beforeunload', beforeUnloadRef.current);
            beforeUnloadRef.current = null;
        }
        await leaveRoom(roomCode);
        window.location.href = pendingNavHref ?? '/';
    };

    return (
        <div className="race-room-page">

            {(phase !== GamePhase.LOBBY &&
              phase !== GamePhase.WAITING &&
              phase !== GamePhase.WAGER_CONFIRM) && (
                <div className={`connection-badge ${isConnected ? 'connected' : 'disconnected'}`}>
                    <span className="connection-dot" />
                    {isConnected ? 'Свързан' : 'Свързване…'}
                </div>
            )}

            {error && (
                <div className="error-overlay">
                    <div className="error-card">
                        <p>{error}</p>
                        <button className="btn btn-dismiss" onClick={dismissError}>Затвори</button>
                    </div>
                </div>
            )}

            {showLeaveConfirm && (
                <div className="error-overlay">
                    <div className="leave-confirm-card">
                        <h3 className="leave-confirm-title">Напускане на стаята</h3>
                        <p className="leave-confirm-message">
                            Сигурни ли сте, че искате да напуснете? Опонентът Ви ще бъде уведомен.
                        </p>
                        <div className="leave-confirm-actions">
                            <button className="btn btn-stay"  onClick={handleLeaveCancel}>Остани</button>
                            <button className="btn btn-leave" onClick={handleLeaveConfirm}>Напусни</button>
                        </div>
                    </div>
                </div>
            )}

            {phase === GamePhase.LOBBY && (
                <LobbyScreen
                    joinCode={joinCode}
                    setJoinCode={setJoinCode}
                    wagerInput={wagerInput}
                    setWagerInput={setWagerInput}
                    onCreateRoom={() => createRoom(parseInt(wagerInput) || 0)}
                    onJoinRoom={() => requestWagerInfo(joinCode)}
                    isConnected={isConnected}
                />
            )}

            {phase === GamePhase.WAGER_CONFIRM && (
                <WagerConfirmScreen
                    wagerAmount={wagerAmount}
                    myPoints={myPoints}
                    creatorUsername={wagerCreatorUsername}
                    onConfirm={confirmJoin}
                    onCancel={cancelWager}
                />
            )}

            {phase === GamePhase.WAITING && (
                <WaitingScreen roomCode={roomCode} wagerAmount={wagerAmount} />
            )}

            {phase === GamePhase.READY_LOBBY && (
                <ReadyLobbyScreen
                    myReady={myReady}
                    otherReady={otherReady}
                    myUsername={myUsername}
                    opponentUsername={opponentUsername}
                    wagerAmount={wagerAmount}
                    onReady={() => setReady(roomCode)}
                />
            )}

            {phase === GamePhase.COUNTDOWN && (
                <CountdownScreen
                    countdownNumber={countdownNumber}
                    myUsername={myUsername}
                    opponentUsername={opponentUsername}
                />
            )}

            {phase === GamePhase.PLAYING && (
                <PlayingScreen
                    cipherText={cipherText}
                    currentRound={currentRound}
                    timeLeft={timeLeft}
                    mySubmitted={mySubmitted}
                    otherSubmitted={otherSubmitted}
                    wagerAmount={wagerAmount}
                    roundHistory={roundHistory}
                    myUsername={myUsername}
                    onSubmit={(answer) => submitAnswer(roomCode, answer)}
                />
            )}

            {phase === GamePhase.ROUND_END && (
                <RoundEndScreen
                    roundWinner={roundWinner}
                    currentRound={currentRound}
                    roundHistory={roundHistory}
                    myUsername={myUsername}
                />
            )}

            {phase === GamePhase.GAME_END && (
                <GameEndScreen gameResult={gameResult} myUsername={myUsername} />
            )}

        </div>
    );
}

const MAX_ROUNDS = 3;

function RoundTracker({ roundHistory, currentRound, myUsername }) {
    return (
        <div className="round-tracker">
            {Array.from({ length: MAX_ROUNDS }, (_, i) => {
                const roundNum = i + 1;
                const result   = roundHistory.find(r => r.round === roundNum);
                let state;
                if (result) {
                    if (result.winner === null)               state = 'draw';
                    else if (result.winner === myUsername)    state = 'win';
                    else                                      state = 'loss';
                } else if (roundNum === currentRound) {
                    state = 'current';
                } else {
                    state = 'empty';
                }
                return (
                    <div key={roundNum} className={`round-dot round-dot-${state}`}>
                        <span className="round-dot-num">{roundNum}</span>
                    </div>
                );
            })}
        </div>
    );
}

function RaceLeaderboard() {
    const [entries,   setEntries]   = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error,     setError]     = useState(false);

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_URL}/api/leaderboard/rooms`, { credentials: 'include' })
            .then(res => { if (!res.ok) throw new Error(); return res.json(); })
            .then(data => setEntries(data))
            .catch(() => setError(true))
            .finally(() => setIsLoading(false));
    }, []);

    return (
        <aside className="leaderboard-section">
            <div className="leaderboard-header">
                <div className="leaderboard-header-decoration">
                    <span className="leaderboard-header-label">Класация</span>
                </div>
                <h2 className="leaderboard-title">Топ победители</h2>
                <p className="leaderboard-subtitle">Стаи спечелени</p>
            </div>
            <div className="leaderboard-list">
                {isLoading && (
                    <div className="waiting-dots" style={{ justifyContent: 'center', padding: '2rem' }}>
                        <span /><span /><span />
                    </div>
                )}
                {error && <p className="lb-error">Грешка при зареждане</p>}
                {!isLoading && !error && entries.length === 0 && (
                    <p className="lb-empty-msg">Все още няма победители</p>
                )}
                {!isLoading && !error && entries.map((entry, index) => (
                    <div className={`leaderboard-item ${index < 3 ? `top-${index + 1}` : ''}`} key={index}>
                        <span className="rank-number">
                            #{String(index + 1).padStart(2, '0')}
                        </span>
                        <div className="player-info">
                            <span className="player-name">{entry.username}</span>
                        </div>
                        <div className="player-wins-badge">
                            <span className="win-count">{entry.points || 0}</span>
                            <span className="win-label">победи</span>
                        </div>
                    </div>
                ))}
            </div>
        </aside>
    );
}

function LobbyScreen({ joinCode, setJoinCode, wagerInput, setWagerInput, onCreateRoom, onJoinRoom, isConnected }) {
    return (
        <div className="screen lobby-screen">
            <div className="lobby-main-content">
                <header className="lobby-header">
                    <div className="lobby-eyebrow">
                        <span className="lobby-eyebrow-label">Криптографска арена</span>
                    </div>
                    <h1 className="lobby-title">Дуел на шифри</h1>
                    <p className="lobby-subtitle">Предизвикай друг играч на дуел по разпознаване на шифри</p>
                </header>

                <div className="lobby-cards">
                    <div className="lobby-card">
                        <div className="lobby-card-icon">Нов мач</div>
                        <h2>Създай стая</h2>
                        <p>Генерирай код за стая и го сподели с опонента си</p>
                        <div className="wager-input-group">
                            <label className="wager-input-label">Залог (точки)</label>
                            <input
                                className="wager-number-input"
                                type="number"
                                min="0"
                                value={wagerInput}
                                onChange={e => setWagerInput(e.target.value === '' ? '' : Math.max(0, parseInt(e.target.value) || 0))}
                                placeholder="0"
                            />
                            <span className="wager-input-hint">0 = без залог</span>
                        </div>
                        <button className="btn btn-primary" onClick={onCreateRoom} disabled={!isConnected}>
                            Създай стая
                        </button>
                    </div>

                    <div className="lobby-divider"><span>или</span></div>

                    <div className="lobby-card">
                        <div className="lobby-card-icon">Присъединяване</div>
                        <h2>Присъедини се</h2>
                        <p>Въведи кода на стаята, споделен от опонента ти</p>
                        <input
                            className="room-code-input"
                            type="text"
                            placeholder="Въведи код на стая"
                            value={joinCode}
                            onChange={e => setJoinCode(e.target.value.toUpperCase())}
                            maxLength={8}
                        />
                        <button
                            className="btn btn-secondary"
                            onClick={onJoinRoom}
                            disabled={!joinCode.trim() || !isConnected}
                        >
                            Присъедини се
                        </button>
                    </div>
                </div>
            </div>

            <RaceLeaderboard />
        </div>
    );
}

function WagerConfirmScreen({ wagerAmount, myPoints, creatorUsername, onConfirm, onCancel }) {
    const canAfford    = myPoints >= wagerAmount;
    const potentialWin = wagerAmount * 2;

    return (
        <div className="screen wager-confirm-screen">
            <div className="wager-confirm-card">
                <div className="wager-confirm-header">
                    <span className="wager-conf-bracket">[</span>
                    <span className="wager-conf-label">ЗАЛОГ</span>
                    <span className="wager-conf-bracket">]</span>
                </div>
                <h2 className="wager-confirm-title">Потвърди залога</h2>

                <div className="wager-confirm-details">
                    <div className="wager-detail-row">
                        <span className="wager-detail-label">Създадено от</span>
                        <span className="wager-detail-value">{creatorUsername}</span>
                    </div>
                    <div className="wager-detail-row">
                        <span className="wager-detail-label">Залог</span>
                        <span className="wager-detail-value wager-detail-amount">{wagerAmount} т.</span>
                    </div>
                    <div className="wager-detail-row">
                        <span className="wager-detail-label">Твоят баланс</span>
                        <span className={`wager-detail-value ${canAfford ? '' : 'wager-detail-insufficient'}`}>
                            {myPoints} т.
                        </span>
                    </div>
                </div>

                {canAfford ? (
                    <div className="wager-outcome-preview">
                        <div className="wager-outcome-row wager-outcome-win">
                            <span>При победа</span>
                            <span>+{potentialWin} т.</span>
                        </div>
                        <div className="wager-outcome-row wager-outcome-lose">
                            <span>При загуба</span>
                            <span>-{wagerAmount} т.</span>
                        </div>
                    </div>
                ) : (
                    <div className="wager-insufficient-msg">
                        Нямаш достатъчно точки за този залог
                    </div>
                )}

                <div className="wager-confirm-actions">
                    <button className="btn btn-stay" onClick={onCancel}>Откажи</button>
                    <button className="btn btn-primary" onClick={onConfirm} disabled={!canAfford}>
                        Приеми залога
                    </button>
                </div>
            </div>
        </div>
    );
}

function WaitingScreen({ roomCode, wagerAmount }) {
    return (
        <div className="screen waiting-screen">
            <div className="waiting-card">
                <div className="waiting-tag">[ ИЗЧАКВАНЕ ]</div>
                <h2>Изчакване на опонент…</h2>
                <p>Сподели кода с опонента си, за да се присъедини</p>
                <div className="room-code-display">
                    <span className="room-code-value">{roomCode}</span>
                    <button className="copy-btn" onClick={() => navigator.clipboard.writeText(roomCode)}>
                        Копирай
                    </button>
                </div>
                {wagerAmount > 0 && (
                    <div className="waiting-wager-row">
                        Залог: <strong>{wagerAmount} точки</strong>
                    </div>
                )}
                <div className="waiting-dots"><span /><span /><span /></div>
            </div>
        </div>
    );
}

function ReadyLobbyScreen({ myReady, otherReady, myUsername, opponentUsername, wagerAmount, onReady }) {
    const myInitial       = myUsername?.[0]?.toUpperCase()       || '?';
    const opponentInitial = opponentUsername?.[0]?.toUpperCase() || '?';

    return (
        <div className="screen ready-lobby-screen">
            <h2 className="ready-title">И двамата играчи са свързани!</h2>

            {wagerAmount > 0 && (
                <div className="ready-wager-badge">
                    Залог: {wagerAmount} точки
                </div>
            )}

            <div className="ready-players">
                <div className={`ready-player-slot ${myReady ? 'is-ready' : ''}`}>
                    <div className="ready-player-avatar">
                        {myInitial}
                        <span className="ready-you-badge">Ти</span>
                    </div>
                    <div className="ready-player-name">{myUsername || '...'}</div>
                    <div className="ready-status">{myReady ? '✓ Готов' : 'Не е готов'}</div>
                </div>

                <div className="vs-badge">VS</div>

                <div className={`ready-player-slot ${otherReady ? 'is-ready' : ''}`}>
                    <div className="ready-player-avatar opponent">
                        {opponentInitial}
                    </div>
                    <div className="ready-player-name">{opponentUsername || '...'}</div>
                    <div className="ready-status">{otherReady ? '✓ Готов' : 'Не е готов'}</div>
                </div>
            </div>

            {!myReady && (
                <button className="btn btn-primary ready-btn" onClick={onReady}>
                    Готов съм!
                </button>
            )}

            {myReady && !otherReady && (
                <p className="waiting-text">Изчакване опонентът да е готов…</p>
            )}
        </div>
    );
}

function CountdownScreen({ countdownNumber, myUsername, opponentUsername }) {
    const myInitial       = myUsername?.[0]?.toUpperCase()       || '?';
    const opponentInitial = opponentUsername?.[0]?.toUpperCase() || '?';

    return (
        <div className="screen countdown-screen">
            <div className="countdown-arena">
                <div className="countdown-fighter countdown-fighter-me">
                    <div className="countdown-fighter-avatar">{myInitial}</div>
                    <div className="countdown-fighter-name">{myUsername || '...'}</div>
                    <div className="countdown-fighter-tag">ТИ</div>
                </div>

                <div className="countdown-center">
                    <div className="countdown-number" key={countdownNumber}>{countdownNumber}</div>
                    <p className="countdown-label">Подготви се…</p>
                </div>

                <div className="countdown-fighter countdown-fighter-opponent">
                    <div className="countdown-fighter-avatar opponent">{opponentInitial}</div>
                    <div className="countdown-fighter-name">{opponentUsername || '...'}</div>
                    <div className="countdown-fighter-tag">ОПОНЕНТ</div>
                </div>
            </div>
        </div>
    );
}

function PlayingScreen({ cipherText, currentRound, timeLeft, mySubmitted, otherSubmitted, wagerAmount, roundHistory, myUsername, onSubmit }) {
    const timerPercent = (timeLeft / 300) * 100;
    const timerClass   = timeLeft > 150 ? 'timer-safe' : timeLeft > 60 ? 'timer-warn' : 'timer-danger';

    return (
        <div className={`screen playing-screen ${timerClass}`}>
            <RoundTracker roundHistory={roundHistory} currentRound={currentRound} myUsername={myUsername} />
            <div className="playing-header">
                <div className="round-badge">Рунд {currentRound}</div>
                {wagerAmount > 0 && (
                    <div className="playing-wager-badge">{wagerAmount} т.</div>
                )}
                <div className="timer-track">
                    <div className={`timer-bar ${timerClass}`} style={{ width: `${timerPercent}%` }} />
                </div>
                <span className={`timer-label ${timerClass}`}>{timeLeft}с</span>
            </div>
            <div className="cipher-display">
                <div className="cipher-display-label">Разпознай шифъра</div>
                <div className="cipher-text">{cipherText}</div>
            </div>
            <div className="submission-status">
                <div className={`submit-indicator ${mySubmitted ? 'submitted' : ''}`}>
                    Ти: {mySubmitted ? '✓ Изпратено' : 'Избира…'}
                </div>
                <div className={`submit-indicator ${otherSubmitted ? 'submitted' : ''}`}>
                    Опонент: {otherSubmitted ? '✓ Изпратено' : 'Избира…'}
                </div>
            </div>
            {!mySubmitted ? (
                <div className="cipher-groups">
                    {CIPHER_GROUPS.map(group => (
                        <div key={group.label} className={`cipher-group ${group.className}`}>
                            <div className="cipher-group-label">{group.label}</div>
                            <div className="cipher-group-buttons">
                                {group.ciphers.map(cipher => (
                                    <button
                                        key={cipher.value}
                                        className="cipher-answer-btn"
                                        onClick={() => onSubmit(cipher.value)}
                                    >
                                        {cipher.label}
                                    </button>
                                ))}
                            </div>
                        </div>
                    ))}
                </div>
            ) : (
                <div className="submitted-message">
                    Отговорът е изпратен — изчакване на опонента…
                </div>
            )}
        </div>
    );
}

function RoundEndScreen({ roundWinner, currentRound, roundHistory, myUsername }) {
    return (
        <div className="screen round-end-screen">
            <div className="round-end-card">
                <div className="round-end-label">Рунд {currentRound} приключи</div>
                {roundWinner ? (
                    <>
                        <div className="round-end-winner">{roundWinner}</div>
                        <div className="round-end-text">спечели рунда</div>
                    </>
                ) : (
                    <>
                        <div className="round-end-draw">—</div>
                        <div className="round-end-text">Няма победител в този рунд</div>
                    </>
                )}
                <RoundTracker roundHistory={roundHistory} currentRound={currentRound + 1} myUsername={myUsername} />
                <p className="next-round-hint">Следващият рунд започва скоро…</p>
            </div>
        </div>
    );
}

function GameEndScreen({ gameResult, myUsername }) {
    if (!gameResult) return null;
    const { winnerUsername, player1Username, player1Score, player2Username, player2Score, wagerAmount } = gameResult;

    const iWon  = winnerUsername === myUsername;
    const iLost = winnerUsername !== null && winnerUsername !== myUsername;

    return (
        <div className="screen game-end-screen">
            <div className="game-end-card">
                {winnerUsername ? (
                    <>
                        <div className="game-end-sup">Победител</div>
                        <h2 className="game-end-title">
                            <span className="winner-name">{winnerUsername}</span>
                        </h2>
                    </>
                ) : (
                    <>
                        <div className="game-end-sup">Резултат</div>
                        <h2 className="game-end-title">Равенство</h2>
                    </>
                )}

                <div className="score-table">
                    <div className={`score-row ${winnerUsername === player1Username ? 'winner-row' : ''}`}>
                        <span className="score-player">{player1Username}</span>
                        <span className="score-value">{player1Score}</span>
                    </div>
                    <div className={`score-row ${winnerUsername === player2Username ? 'winner-row' : ''}`}>
                        <span className="score-player">{player2Username}</span>
                        <span className="score-value">{player2Score}</span>
                    </div>
                </div>

                {wagerAmount > 0 && (
                    <div className="wager-outcome-section">
                        {winnerUsername === null && (
                            <div className="wager-tie-outcome">
                                Равенство — залогът от {wagerAmount} т. беше върнат
                            </div>
                        )}
                        {iWon && (
                            <div className="wager-winner-outcome">
                                Спечели {wagerAmount * 2} точки от залога
                            </div>
                        )}
                        {iLost && (
                            <div className="wager-loser-outcome">
                                Загуби {wagerAmount} точки от залога
                            </div>
                        )}
                    </div>
                )}

                <button className="btn btn-primary" onClick={() => window.location.href = '/'}>
                    Към началото
                </button>
            </div>
        </div>
    );
}
