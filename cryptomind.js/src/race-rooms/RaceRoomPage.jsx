import { useState, useEffect } from 'react';
import { useRaceRoom, GamePhase, CipherType } from './useRaceRooms';
import '../styles/race-room-page.css';

const CIPHER_GROUPS = [
    {
        label:     'Заместване',
        className: 'group-substitution',
        ciphers: [
            { label: 'Цезар',               value: CipherType.Caesar              },
            { label: 'Атбаш',               value: CipherType.Atbash              },
            { label: 'Просто заместване',   value: CipherType.SimpleSubstitution  },
            { label: 'ROT13',               value: CipherType.ROT13               },
        ],
    },
    {
        label:     'Полиазбучни',
        className: 'group-polyalphabetic',
        ciphers: [
            { label: 'Виженер',    value: CipherType.Vigenere   },
            { label: 'Автоключ',   value: CipherType.Autokey    },
            { label: 'Тритемиус',  value: CipherType.Trithemius },
        ],
    },
    {
        label:     'Транспозиция',
        className: 'group-transposition',
        ciphers: [
            { label: 'Железопътна ограда', value: CipherType.RailFence },
            { label: 'Колонна',            value: CipherType.Columnar  },
            { label: 'Маршрут',            value: CipherType.Route     },
        ],
    },
    {
        label:     'Кодиране',
        className: 'group-encoding',
        ciphers: [
            { label: 'Base64', value: CipherType.Base64 },
            { label: 'Морзе',  value: CipherType.Morse  },
            { label: 'Двоично', value: CipherType.Binary },
            { label: 'Шестнадесетично', value: CipherType.Hex },
        ],
    },
];

export default function RaceRoomPage() {
    const [joinCode, setJoinCode] = useState('');

    const {
        phase,
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
        createRoom,
        joinRoom,
        setReady,
        submitAnswer,
        dismissError,
    } = useRaceRoom();

    useEffect(() => {
        if (phase === GamePhase.LOBBY) {
            setJoinCode('');
        }
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

    return (
        <div className="race-room-page">

            <div className={`connection-badge ${isConnected ? 'connected' : 'disconnected'}`}>
                <span className="connection-dot" />
                {isConnected ? 'Свързан' : 'Свързване…'}
            </div>

            {error && (
                <div className="error-overlay">
                    <div className="error-card">
                        <p>{error}</p>
                        <button className="btn btn-dismiss" onClick={dismissError}>Затвори</button>
                    </div>
                </div>
            )}

            {phase === GamePhase.LOBBY && (
                <LobbyScreen
                    joinCode={joinCode}
                    setJoinCode={setJoinCode}
                    onCreateRoom={createRoom}
                    onJoinRoom={() => joinRoom(joinCode)}
                    isConnected={isConnected}
                />
            )}

            {phase === GamePhase.WAITING && (
                <WaitingScreen roomCode={roomCode} />
            )}

            {phase === GamePhase.READY_LOBBY && (
                <ReadyLobbyScreen
                    myReady={myReady}
                    otherReady={otherReady}
                    onReady={() => {
                        setReady(roomCode);
                    }}
                />
            )}

            {phase === GamePhase.COUNTDOWN && (
                <CountdownScreen countdownNumber={countdownNumber} />
            )}

            {phase === GamePhase.PLAYING && (
                <PlayingScreen
                    cipherText={cipherText}
                    currentRound={currentRound}
                    timeLeft={timeLeft}
                    mySubmitted={mySubmitted}
                    otherSubmitted={otherSubmitted}
                    onSubmit={(answer) => submitAnswer(roomCode, answer)}
                />
            )}

            {phase === GamePhase.ROUND_END && (
                <RoundEndScreen
                    roundWinner={roundWinner}
                    currentRound={currentRound}
                />
            )}

            {phase === GamePhase.GAME_END && (
                <GameEndScreen gameResult={gameResult} />
            )}

        </div>
    );
}

function LobbyScreen({ joinCode, setJoinCode, onCreateRoom, onJoinRoom, isConnected }) {
    return (
        <div className="screen lobby-screen">
            <div className="lobby-header">
                <h1 className="lobby-title">Стая за надпревара</h1>
                <p className="lobby-subtitle">Предизвикай друг играч на дуел по разпознаване на шифри</p>
            </div>

            <div className="lobby-cards">
                <div className="lobby-card">
                    <div className="lobby-card-icon">⚔️</div>
                    <h2>Създай стая</h2>
                    <p>Генерирай код за стая и го сподели с опонента си</p>
                    <button className="btn btn-primary" onClick={onCreateRoom} disabled={!isConnected}>
                        Създай стая
                    </button>
                </div>

                <div className="lobby-divider"><span>или</span></div>

                <div className="lobby-card">
                    <div className="lobby-card-icon">🔑</div>
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
    );
}

function WaitingScreen({ roomCode }) {
    return (
        <div className="screen waiting-screen">
            <div className="waiting-card">
                <div className="waiting-icon">⏳</div>
                <h2>Изчакване на опонент…</h2>
                <p>Сподели този код с опонента си, за да се присъедини</p>

                <div className="room-code-display">
                    <span className="room-code-value">{roomCode}</span>
                    <button
                        className="copy-btn"
                        onClick={() => navigator.clipboard.writeText(roomCode)}
                    >
                        Копирай
                    </button>
                </div>

                <div className="waiting-dots">
                    <span /><span /><span />
                </div>
            </div>
        </div>
    );
}

function ReadyLobbyScreen({ myReady, otherReady, onReady }) {
    return (
        <div className="screen ready-lobby-screen">
            <h2 className="ready-title">И двамата играчи са свързани!</h2>

            <div className="ready-players">
                <div className={`ready-player-slot ${myReady ? 'is-ready' : ''}`}>
                    <div className="ready-player-avatar">Ти</div>
                    <div className="ready-status">{myReady ? '✓ Готов' : 'Не е готов'}</div>
                </div>

                <div className="vs-badge">VS</div>

                <div className={`ready-player-slot ${otherReady ? 'is-ready' : ''}`}>
                    <div className="ready-player-avatar">Опонент</div>
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

function CountdownScreen({ countdownNumber }) {
    return (
        <div className="screen countdown-screen">
            <p className="countdown-label">Подготви се…</p>
            <div className="countdown-number" key={countdownNumber}>
                {countdownNumber}
            </div>
        </div>
    );
}

function PlayingScreen({ cipherText, currentRound, timeLeft, mySubmitted, otherSubmitted, onSubmit }) {
    const timerPercent = (timeLeft / 300) * 100;
    const timerClass   = timeLeft > 150 ? 'timer-safe' : timeLeft > 60 ? 'timer-warn' : 'timer-danger';

    return (
        <div className="screen playing-screen">

            <div className="playing-header">
                <div className="round-badge">Рунд {currentRound}</div>
                <div className="timer-track">
                    <div
                        className={`timer-bar ${timerClass}`}
                        style={{ width: `${timerPercent}%` }}
                    />
                </div>
                <span className={`timer-label ${timerClass}`}>{timeLeft}с</span>
            </div>

            <div className="cipher-display">
                <div className="cipher-display-label">Разпознай шифъра</div>
                <div className="cipher-text">{cipherText}</div>
            </div>

            <div className="submission-status">
                <div className={`submit-indicator ${mySubmitted ? 'submitted' : ''}`}>
                    Ти: {mySubmitted ? 'Изпратено ✓' : 'Избира…'}
                </div>
                <div className={`submit-indicator ${otherSubmitted ? 'submitted' : ''}`}>
                    Опонент: {otherSubmitted ? 'Изпратено ✓' : 'Избира…'}
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
                    ✓ Отговорът е изпратен — изчакване на опонента…
                </div>
            )}

        </div>
    );
}

function RoundEndScreen({ roundWinner, currentRound }) {
    return (
        <div className="screen round-end-screen">
            <div className="round-end-card">
                <div className="round-end-label">Рунд {currentRound} приключи</div>

                {roundWinner ? (
                    <>
                        <div className="round-end-icon">🏆</div>
                        <div className="round-end-text">
                            <span className="winner-name">{roundWinner}</span> спечели рунда!
                        </div>
                    </>
                ) : (
                    <>
                        <div className="round-end-icon">🤝</div>
                        <div className="round-end-text">Няма победител в този рунд</div>
                    </>
                )}

                <p className="next-round-hint">Следващият рунд започва скоро…</p>
            </div>
        </div>
    );
}

function GameEndScreen({ gameResult }) {
    if (!gameResult) return null;

    const { winnerUsername, player1Username, player1Score, player2Username, player2Score } = gameResult;

    return (
        <div className="screen game-end-screen">
            <div className="game-end-card">
                {winnerUsername ? (
                    <>
                        <div className="game-end-icon">🏆</div>
                        <h2 className="game-end-title">
                            <span className="winner-name">{winnerUsername}</span> спечели!
                        </h2>
                    </>
                ) : (
                    <>
                        <div className="game-end-icon">🤝</div>
                        <h2 className="game-end-title">Равенство!</h2>
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

                <button className="btn btn-primary" onClick={() => window.location.href = '/'}>
                    Към началото
                </button>
            </div>
        </div>
    );
}