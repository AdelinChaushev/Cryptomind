import { useState, useEffect, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const ROUND_DURATION_SECONDS = 300;
const PRE_ROUND_SECONDS = 3;
const HUB_URL = `${import.meta.env.VITE_API_URL}/raceRoomHub`;

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
    LOBBY:       'lobby',
    WAITING:     'waiting',
    READY_LOBBY: 'readyLobby',
    COUNTDOWN:   'countdown',
    PLAYING:     'playing',
    ROUND_END:   'roundEnd',
    GAME_END:    'gameEnd',
};

export function useRaceRoom() {
    const [phase,           setPhase]          = useState(GamePhase.LOBBY);
    const [roomCode,        setRoomCode]        = useState('');
    const [cipherText,      setCipherText]      = useState('');
    const [currentRound,    setCurrentRound]    = useState(0);
    const [roundWinner,     setRoundWinner]     = useState(null);
    const [gameResult,      setGameResult]      = useState(null);
    const [mySubmitted,     setMySubmitted]     = useState(false);
    const [otherSubmitted,  setOtherSubmitted]  = useState(false);
    const [timeLeft,        setTimeLeft]        = useState(ROUND_DURATION_SECONDS);
    const [myReady,         setMyReady]         = useState(false);
    const [otherReady,      setOtherReady]      = useState(false);
    const [error,           setError]           = useState(null);
    const [isConnected,     setIsConnected]     = useState(false);
    const [countdownNumber, setCountdownNumber] = useState(PRE_ROUND_SECONDS);

    const connectionRef      = useRef(null);
    const countdownRef       = useRef(null);
    const preCountdownRef    = useRef(null);
    const transitionRef      = useRef(null);
    const myReadyRef         = useRef(false);
    const pendingRoomCodeRef = useRef('');
    const phaseRef           = useRef(GamePhase.LOBBY);
    const pendingCipherRef   = useRef('');
    const pendingRoundRef    = useRef(0);

    const setPhaseSync = useCallback((newPhase) => {
        phaseRef.current = newPhase;
        setPhase(newPhase);
    }, []);

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
            const elapsed = Math.floor((Date.now() - startedAt) / 1000);
            const remaining = ROUND_DURATION_SECONDS - elapsed;
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

        connection.on('RoomCreated', (code) => {
            setRoomCode(code);
            setPhaseSync(GamePhase.WAITING);
        });

        connection.on('RoomJoined', (success) => {
            if (success) {
                if (pendingRoomCodeRef.current) {
                    setRoomCode(pendingRoomCodeRef.current);
                }
                setPhaseSync(GamePhase.READY_LOBBY);
            }
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

            clearCountdown();
            clearPreCountdown();
            clearTransition();
            setRoomCode('');
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
            setPhaseSync(GamePhase.LOBBY);
            setError('Опонентът ти се е изключил. Стаята е затворена.');
        });

        connection.on('AnswerSubmitted',       () => setMySubmitted(true));
        connection.on('OtherUserHasSubmitted', () => setOtherSubmitted(true));
        connection.on('Error',   (message)     => setError(message));

        connection.onreconnecting(() => setIsConnected(false));
        connection.onreconnected(()  => setIsConnected(true));
        connection.onclose(()        => setIsConnected(false));

        const start = async () => {
            try {
                await connection.start();
                setIsConnected(true);
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
    }, [setPhaseSync, startCountdown, clearCountdown, clearTransition, startPreRoundCountdown, clearPreCountdown]);

    const createRoom = useCallback(() => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        connectionRef.current.invoke('CreateRoom')
            .catch(err => setError(err.message));
    }, []);

    const joinRoom = useCallback((code) => {
        if (connectionRef.current?.state !== signalR.HubConnectionState.Connected) return;
        pendingRoomCodeRef.current = code;
        connectionRef.current.invoke('JoinRoom', code)
            .catch(err => setError(err.message));
    }, []);

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

    const dismissError = useCallback(() => setError(null), []);

    return {
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
    };
}