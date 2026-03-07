import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/pending-answers.css';
import { Link } from 'react-router-dom';
const API_BASE = `${import.meta.env.VITE_API_URL}/api/admin`;

axios.defaults.withCredentials = true;

const PendingAnswers = () => {
    const [answers, setAnswers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [cipherNameFilter, setCipherNameFilter] = useState('');
    const [usernameFilter, setUsernameFilter] = useState('');
    const [debouncedCipherName, setDebouncedCipherName] = useState('');
    const [debouncedUsername, setDebouncedUsername] = useState('');

    useEffect(() => {
        const timer = setTimeout(() => setDebouncedCipherName(cipherNameFilter), 300);
        return () => clearTimeout(timer);
    }, [cipherNameFilter]);

    useEffect(() => {
        const timer = setTimeout(() => setDebouncedUsername(usernameFilter), 300);
        return () => clearTimeout(timer);
    }, [usernameFilter]);

    const fetchAnswers = useCallback(async () => {
        try {
            setLoading(true);
            const params = {};
            if (debouncedCipherName) params.cipherName = debouncedCipherName;
            if (debouncedUsername) params.username = debouncedUsername;
            const { data } = await axios.get(`${API_BASE}/pending-answer-suggestions`, { params });
            setAnswers(Array.isArray(data) ? data : []);
            setError(null);
        } catch (err) {
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedCipherName, debouncedUsername]);

    useEffect(() => { fetchAnswers(); }, [fetchAnswers]);

    useEffect(() => {
        const refreshBtn = document.getElementById('btn-refresh-answers');
        if (refreshBtn) refreshBtn.addEventListener('click', fetchAnswers);
        return () => { if (refreshBtn) refreshBtn.removeEventListener('click', fetchAnswers); };
    }, [fetchAnswers]);

    const emptyOrError = error ? (
        <div className="data-table-wrapper">
            <div className="empty-state">
                <div className="empty-state-title">Грешка при зареждане на предложенията</div>
                <div className="empty-state-text">{error}</div>
            </div>
        </div>
    ) : answers.length === 0 && !loading ? (
        <div className="data-table-wrapper">
            <div className="empty-state">
                <svg className="empty-state-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
                </svg>
                <div className="empty-state-title">Няма изчакващи предложения</div>
                <div className="empty-state-text">
                    {cipherNameFilter || usernameFilter
                        ? 'Няма резултати, съответстващи на филтрите'
                        : 'Всички предложения от общността са прегледани'}
                </div>
            </div>
        </div>
    ) : null;

    return (
        <div className="admin-shell admin-shell--pending-answers">
            <AdminSidebar activePage="pending-answers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Изчакващи отговори' }]}>
                    <button className="btn btn-ghost btn-sm" id="btn-refresh-answers">Опресни</button>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Изчакващи предложени отговори</h1>
                        <p className="page-subtitle">
                            {answers.length} предложени{answers.length !== 1 ? 'я' : 'е'} изчакват преглед
                        </p>
                    </div>

                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input type="text" className="form-input" placeholder="Филтър по име на шифър..." value={cipherNameFilter} onChange={(e) => setCipherNameFilter(e.target.value)} />
                            </div>
                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="8" cy="8" r="3"/><path d="M8 1v2M8 13v2M15 8h-2M3 8H1"/>
                                </svg>
                                <input type="text" className="form-input" placeholder="Филтър по потребителско име..." value={usernameFilter} onChange={(e) => setUsernameFilter(e.target.value)} />
                            </div>
                        </div>
                        <div className="toolbar-right">
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)' }}>Зареждане...</span>}
                        </div>
                    </div>

                    {emptyOrError || (
                        <>
                            <div className="data-table-wrapper">
                                <table className="data-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Шифър</th>
                                            <th>Описание</th>
                                            <th>Предложен от</th>
                                            <th>Дата</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {answers.map((answer) => (
                                            <tr key={answer.id}>
                                                <td className="mono" style={{ color: 'var(--text-dim)', fontSize: '11px' }}>#{answer.id}</td>
                                                <td>{answer.cipherName || `Шифър #${answer.cipherId}`}</td>
                                                <td><div className="answer-preview">{answer.description || '—'}</div></td>
                                                <td className="mono" style={{ fontSize: '12px' }}>{answer.username}</td>
                                                <td className="mono" style={{ fontSize: '12px' }}>{answer.submittedAt}</td>
                                                <td>
                                                    <Link to={`/admin/answer-review/${answer.id}`} className="btn btn-primary btn-sm">
                                                        Прегледай →
                                                    </Link>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>

                            <div className="answers-card-list">
                                {answers.map((answer) => (
                                    <div key={answer.id} className="answer-card">
                                        <div className="answer-card__row">
                                            <span className="answer-card__label">Шифър</span>
                                            <span className="answer-card__value">{answer.cipherName || `Шифър #${answer.cipherId}`}</span>
                                        </div>
                                        <div className="answer-card__row">
                                            <span className="answer-card__label">Потребител</span>
                                            <span className="answer-card__value">{answer.username}</span>
                                        </div>
                                        <div className="answer-card__row">
                                            <span className="answer-card__label">Описание</span>
                                            <span className="answer-card__value answer-card__value--dim">{answer.description || '—'}</span>
                                        </div>
                                        <div className="answer-card__actions">
                                            <Link to={`/admin/answer-review/${answer.id}`} className="btn btn-primary btn-sm">
                                                Прегледай →
                                            </Link>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </>
                    )}
                </div>
            </main>
        </div>
    );
};

export default PendingAnswers;