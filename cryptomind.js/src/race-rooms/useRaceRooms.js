import { useState, useEffect, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const ROUND_DURATION_SECONDS = 300;
const PRE_ROUND_SECONDS      = 3;
const HUB_URL                = `${import.meta.env.VITE_API_URL}/raceRoomHub`;

export const CipherType = {
    Caesar:             0,
    Atbash:             1,
    SimpleSubstitution: 2,
    ROT13:              3,
    Vigenere:           4,
    Autokey:            5,
    Trithemius:         6,
    RailFence:          7,
    Columnar:           8,
    Route:              9,
    Base64:             10,
    Morse:              11,
    Binary:             12,
    Hex:                13,
};

export const GamePhase = {
    LOBBY:         'lobby',
    WAGER_CONFIRM: 'wagerConfirm',
    WAITING:       'waiting',
    READY_LOBBY:   'readyLobby',
    COUNTDOWN:     'countdown',
    PLAYING:       'playing',
    ROUND_END:     'roundEnd',
    GAME_END:      'gameEnd',
};

export function useRaceRoom() {
    const [phase,                setPhase]                = useState(GamePhase.LOBBY);
    const [roomCode,             setRoomCode]             = useState('');
    const [cipherText,           setCipherText]           = useState('');
    const [currentRound,         setCurrentRound]         = useState(0);
    const [roundWinner,          setRoundWinner]          = useState(null);
    const [gameResult,           setGameResult]           = useState(null);
    const [mySubmitted,          setMySubmitted]          = useState(false);
    const [otherSubmitted,       setOtherSubmitted]       = useState(false);
    const [timeLeft,             setTimeLeft]             = useState(ROUND_DURATION_SECONDS);
    const [myReady,              setMyReady]              = useState(false);
    const [otherReady,           setOtherReady]           = useState(false);
    const [error,                setError]                = useState(null);
    const [isConnected,          setIsConnected]          = useState(false);
    const [countdownNumber,      setCountdownNumber]      = useState(PRE_ROUND_SECONDS);
    const [myUsername,           setMyUsername]           = useState('');
    const [opponentUsername,     setOpponentUsername]     = useState('');
    const [wagerAmount,          setWagerAmount]          = useState(0);
    const [myPoints,             setMyPoints]             = useState(0);
    const [wagerCreatorUsername, setWagerCreatorUsername] = useState('');
    const [roundHistory,         setRoundHistory]         = useState([]);

    const connectionRef      = useRef(null);
    const countdownRef       = useRef(null);
    const preCountdownRef    = useRef(null);
    const transitionRef      = useRef(null);
    const myReadyRef         = useRef(false);
    const pendingRoomCodeRef = useRef('');
    const phaseRef           = useRef(GamePhase.LOBBY);
    const pendingCipherRef   = useRef('');
    const pendingRoundRef    = useRef(0);
    const currentRoundRef    = useRef(0);
    const roomCodeRef        = useRef('');

    const setPhaseSync = useCallback((newPhase) => {
        phaseRef.current = newPhase;
        setPhase(newPhase);
        if (newPhase !== GamePhase.LOBBY &&
            newPhase !== GamePhase.GAME_END &&
            newPhase !== GamePhase.WAGER_CONFIRM) {
            window.history.pushState(null, '', window.location.href);
        }
    }, []);

    const setRoomCodeSync = useCallback((code) => {
        roomCodeRef.current = code;
        setRoomCode(code);
    }, []);

    const resetToLobby = useCallback(() => {
        clearInterval(countdownRef.current);
        countdownRef.current = null;
        clearInterval(preCountdownRef.current);
        preCountdownRef.current = null;
        clearTimeout(transitionRef.current);
        transitionRef.current = null;
        setRoomCodeSync('');
        setCipherText('');
        setCurrentRound(0);
        setRoundWinner(null);
        setGameResult(null);
        setMySubmitted(false);
        setOtherSubmitted(false);
        setTimeLeft(ROUND_DURATION_SECONDS);
        setMyReady(false);
        myReadyRef.current = false;
        setOtherReady(false);
        setMyUsername('');
        setOpponentUsername('');
        setWagerAmount(0);
        setMyPoints(0);
        setWagerCreatorUsername('');
        setRoundHistory([]);
        currentRoundRef.current = 0;
        setPhaseSync(GamePhase.LOBBY);
    }, [setPhaseSync, setRoomCodeSync]);

    const clearCountdown = useCallback(() => {
        if (countdownRef.current) {
            clearInterval(countdownRef.current);
            countdownRef.current = null;
        }
    }, []);

    const startCountdown = useCallback(() => {
        clearCountdown();
        setTimeLeft(ROUND_DURATION_SECONDS);
        const startedAt = Date.now();
        countdownRef.current = setInterval(() => {
            const elapsed   = Math.floor((Date.now() - startedAt) / 1000);
            const remaining = ROUND_DURATION_SECONDS - elapsed;
            if (remaining <= 0) {
                clearCountdown();
                setTimeLeft(0);
            } else {
                setTimeLeft(remaining);
            }
        }, 500);
    }, [clearCountdown]);

    const startCountdownFrom = useCallback((secondsRemaining) => {
        clearCountdown();
        const clamped   = Math.max(0, secondsRemaining);
        setTimeLeft(clamped);
        const startedAt = Date.now();
        countdownRef.current = setInterval(() => {
            const elapsed   = Math.floor((Date.now() - startedAt) / 1000);
            const remaining = clamped - elapsed;
            if (remaining <= 0) {
                clearCountdown();
                setTimeLeft(0);
            } else {
                setTimeLeft(remaining);
            }
        }, 500);
    }, [clearCountdown]);

    const clearTransition = useCallback(() => {
        if (transitionRef.current) {
            clearTimeout(transitionRef.current);
            transitionRef.current = null;
        }
    }, []);

    const clearPreCountdown = useCallback(() => {
        if (preCountdownRef.current) {
            clearInterval(preCountdownRef.current);
            preCountdownRef.current = null;
        }
    }, []);

    const startPreRoundCountdown = useCallback((encryptedText, roundNumber) => {
        clearPreCountdown();
        pendingCipherRef.current = encryptedText;
        pendingRoundRef.current  = roundNumber;

        setCountdownNumber(PRE_ROUND_SECONDS);
        setPhaseSync(GamePhase.COUNTDOWN);

        let current = PRE_ROUND_SECONDS;
        preCountdownRef.current = setInterval(() => {
            current -= 1;
            if (current <= 0) {
                clearPreCountdown();
                setCipherText(pendingCipherRef.current);
                setCurrentRound(pendingRoundRef.current);
                currentRoundRef.current = pendingRoundRef.current;
                setMySubmitted(false);
                setOtherSubmitted(false);
                setPhaseSync(GamePhase.PLAYING);
                startCountdown();
            } else {
                setCountdownNumber(current);
            }
        }, 1000);
    }, [clearPreCountdown, setPhaseSync, startCountdown]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, {
                withCredentials: true,
                transport:
                    signalR.HttpTransportType.WebSockets       |
                    signalR.HttpTransportType.ServerSentEvents |
                    signalR.HttpTransportType.LongPolling,
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on('RoomCreated', (code, wager) => {
            setRoomCodeSync(code);
            setWagerAmount(wager || 0);
            setPhaseSync(GamePhase.WAITING);
        });

        connection.on('WagerInfo', (info) => {
            if (!info.wagerAmount || info.wagerAmount === 0) {
                connectionRef.current?.invoke('JoinRoom', pendingRoomCodeRef.current)
                    .catch(err => setError(err.message));
                return;
            }
            setWagerAmount(info.wagerAmount);
            setMyPoints(info.joinerBalance);
            setWagerCreatorUsername(info.creatorUsername);
            setPhaseSync(GamePhase.WAGER_CONFIRM);
        });

        connection.on('RoomJoined', (success) => {
            if (success) {
                if (pendingRoomCodeRef.current) {
                    setRoomCodeSync(pendingRoomCodeRef.current);
                }
                setPhaseSync(GamePhase.READY_LOBBY);
            }
        });

        connection.on('PlayersInfo', (myUser, opponentUser) => {
            setMyUsername(myUser);
            setOpponentUsername(opponentUser);
        });

        connection.on('PlayerReady', () => {
            if (!myReadyRef.current) {
                setOtherReady(true);
            }
        });

        connection.on('GameIsStarting', (encryptedText) => {
            startPreRoundCountdown(encryptedText, 1);
        });

        connection.on('RoundEnded', (winnerUsername) => {
            clearCountdown();
            setRoundWinner(winnerUsername);
            setRoundHistory(prev => {
                const round = currentRoundRef.current;
                if (prev.find(r => r.round === round)) return prev;
                return [...prev, { round, winner: winnerUsername }];
            });
            setPhaseSync(GamePhase.ROUND_END);
        });

        connection.on('NextRoundStarting', (encryptedText) => {
            clearTransition();
            transitionRef.current = setTimeout(() => {
                transitionRef.current = null;
                setCurrentRound(prev => {
                    const next = prev + 1;
                    startPreRoundCountdown(encryptedText, next);
                    return prev;
                });
            }, 3000);
        });

        connection.on('GameEnded', (result) => {
            clearTransition();
            transitionRef.current = setTimeout(() => {
                transitionRef.current = null;
                clearCountdown();
                setGameResult(result);
                setPhaseSync(GamePhase.GAME_END);
            }, 3000);
        });

        connection.on('PlayerDisconnected', () => {
            if (phaseRef.current === GamePhase.GAME_END) return;
            resetToLobby();
            setError('Опонентът ти се е изключил. Стаята е затворена.');
        });

        connection.on('RoomCancelled', (reason) => {
            resetToLobby();
            setError(reason || 'Стаята беше отменена.');
        });

        connection.on('RoomNoLongerExists', () => {
            resetToLobby();
            setError('Стаята вече не съществува. Моля, създай нова стая.');
        });

        connection.on('GameStateRestored', (state) => {
            setRoomCodeSync(state.roomCode);
            setWagerAmount(state.wagerAmount || 0);
            setMyUsername(state.myUsername || '');
            setOpponentUsername(state.opponentUsername || '');
            setRoundHistory(
                (state.roundHistory || []).map(r => ({ round: r.round, winner: r.winnerUsername }))
            );

            if (state.isRoundEnd) {
                const restoredRound = state.currentRound - 1;
                setCurrentRound(restoredRound);
                currentRoundRef.current = restoredRound;
                const justEnded = (state.roundHistory || []).find(r => r.round === restoredRound);
                setRoundWinner(justEnded?.winnerUsername ?? null);
                setPhaseSync(GamePhase.ROUND_END);
                transitionRef.current = setTimeout(() => {
                    transitionRef.current = null;
                    startPreRoundCountdown(state.nextEncryptedText, state.currentRound);
                }, state.transitionMsRemaining);
                return;
            }

            const secondsRemaining = ROUND_DURATION_SECONDS - state.secondsElapsed;
            setCipherText(state.encryptedText);
            setCurrentRound(state.currentRound);
            currentRoundRef.current = state.currentRound;
            setMySubmitted(state.hasSubmitted);
            setOtherSubmitted(state.hasOpponentSubmitted || false);
            setPhaseSync(GamePhase.PLAYING);
            startCountdownFrom(secondsRemaining);
        });

        connection.on('NoActiveRoom', () => {
            if (phaseRef.current !== GamePhase.LOBBY && phaseRef.current !== GamePhase.GAME_END) {
                resetToLobby();
            }
        });

        connection.on('AnswerSubmitted',       () => setMySubmitted(true));
        connection.on('OtherUserHasSubmitted', () => setOtherSubmitted(true));
        connection.on('Error', (message)       => setError(message));

        connection.onreconnecting(() => setIsConnected(false));

        connection.onreconnected(() => {
            setIsConnected(true);
            connection.invoke('RequestCurrentState').catch(() => {});
        });

        connection.onclose(() => {
            setIsConnected(false);
            if (phaseRef.current === GamePhase.GAME_END) return;
            resetToLobby();
        });

        const start = async () => {
            try {
                await connection.start();
                setIsConnected(true);
                connection.invoke('RequestCurrentState').catch(() => {});
            } catch {
                setTimeout(start, 5000);
            }
        };

        start();
        connectionRef.current = connection;

        return () => {
            clearCountdown();
            clearPreCountdown();
            clearTransition();
            connection.stop();
        };
    }, [setPhaseSync, setRoomCodeSync, startCountdown, startCountdownFrom,
        clearCountdown, clearTransition, startPreRoundCountdown, clearPreCountdown, resetToLobby]);

    const createRoom = useCallback((wager = 0) => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        connectionRef.current.invoke('CreateRoom', wager)
            .catch(err => setError(err.message));
    }, []);

    const requestWagerInfo = useCallback((code) => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        pendingRoomCodeRef.current = code;
        connectionRef.current.invoke('GetWagerInfo', code)
            .catch(err => setError(err.message));
    }, []);

    const confirmJoin = useCallback(() => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        connectionRef.current.invoke('JoinRoom', pendingRoomCodeRef.current)
            .catch(err => setError(err.message));
    }, []);

    const cancelWager = useCallback(() => {
        setWagerAmount(0);
        setMyPoints(0);
        setWagerCreatorUsername('');
        pendingRoomCodeRef.current = '';
        setPhaseSync(GamePhase.LOBBY);
    }, [setPhaseSync]);

    const setReady = useCallback((code) => {
        setMyReady(true);
        myReadyRef.current = true;
        connectionRef.current?.invoke('SetReady', code)
            .catch(err => {
                setMyReady(false);
                myReadyRef.current = false;
                setError(err.message);
            });
    }, []);

    const submitAnswer = useCallback((code, answer) => {
        connectionRef.current?.invoke('SubmitAnswer', code, answer)
            .catch(err => setError(err.message));
    }, []);

    const leaveRoom = useCallback(async (code) => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        try {
            await connectionRef.current.invoke('LeaveRoom', code);
        } catch {}
        await connectionRef.current.stop();
    }, []);

    const dismissError = useCallback(() => setError(null), []);

    return {
        phase,
        phaseRef,
        roomCode,
        roomCodeRef,
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
    };
}