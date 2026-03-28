import axios from 'axios';

const API = `${import.meta.env.VITE_API_URL}/api/daily-challenge`;

export const getTodaysChallenge = () =>
    axios.get(API, { withCredentials: true }).then(r => r.data);

export const submitAnswer = (answer) =>
    axios.post(`${API}/solve`, { answer }, { withCredentials: true }).then(r => r.data);

export const getStreakLeaderboard = () =>
    fetch(`${API}/leaderboard`).then(r => r.json());
