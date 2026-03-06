import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/users-management.css';
const API_BASE = `${import.meta.env.VITE_API_URL}/api/admin`;
import { useError } from '../ErrorContext'
axios.defaults.withCredentials = true;

const UsersManagement = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { setError: setGlobalError } = useError();
    const [usernameFilter, setUsernameFilter] = useState('');
    const [debouncedUsername, setDebouncedUsername] = useState('');
    const [showBanned, setShowBanned] = useState(false);
    const [showDeactivated, setShowDeactivated] = useState(false);
    const [banModal, setBanModal] = useState({ open: false, userId: null, username: '' });
    const [banReason, setBanReason] = useState('');

    useEffect(() => {
        const timer = setTimeout(() => setDebouncedUsername(usernameFilter), 300);
        return () => clearTimeout(timer);
    }, [usernameFilter]);

    const fetchUsers = useCallback(async () => {
        try {
            setLoading(true);
            const params = { IsBanned: showBanned, IsDeactivated: showDeactivated, Username: debouncedUsername || '' };
            const { data } = await axios.get(`${API_BASE}/users`, { params });
            setUsers(Array.isArray(data) ? data : []);
        } catch (err) {
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedUsername, showBanned, showDeactivated]);

    useEffect(() => { fetchUsers(); }, [fetchUsers]);

    const handlePromote = useCallback(async (userId) => {
        try {
            await axios.put(`${API_BASE}/user/${userId}/admin`);
            fetchUsers();
        } catch (err) {
            setGlobalError(`Неуспешно повишаване: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchUsers]);

    const openBanModal = useCallback((userId, username) => {
        setBanModal({ open: true, userId, username });
        setBanReason('');
    }, []);

    const handleConfirmBan = useCallback(async () => {
        if (!banReason.trim()) { setGlobalError('Моля, въведете причина за блокирането на потребителя.'); return; }
        try {
            const formData = new FormData();
            formData.append("reason", banReason);
            await axios.put(`${API_BASE}/user/${banModal.userId}/ban`, formData, { headers: { "Content-Type": "multipart/form-data" }, withCredentials: true });
            setBanModal({ open: false, userId: null, username: '' });
            setBanReason('');
            fetchUsers();
        } catch (err) {
            setGlobalError(`Неуспешно блокиране: ${err.response?.data?.message || err.message}`);
        }
    }, [banModal.userId, banReason, fetchUsers]);

    const closeBanModal = useCallback(() => {
        setBanModal({ open: false, userId: null, username: '' });
        setBanReason('');
    }, []);

    const handleUnban = useCallback(async (userId) => {
        try {
            await axios.put(`${API_BASE}/user/${userId}/unban`);
            fetchUsers();
        } catch (err) {
            setGlobalError(`Неуспешно деблокиране: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchUsers]);

    const renderActions = (user) => (
        <>
            {!user.isAdmin && !(showBanned || showDeactivated) && (
                <button onClick={() => handlePromote(user.id)} className="btn btn-primary btn-sm">
                    Повиши до Администратор
                </button>
            )}
            {showBanned && (
                <button onClick={() => handleUnban(user.id)} className="btn btn-success btn-sm">
                    Деблокирай
                </button>
            )}
            {!user.isAdmin && !showBanned && !showDeactivated && (
                <button onClick={() => openBanModal(user.id, user.username)} className="btn btn-danger btn-sm">
                    Блокирай
                </button>
            )}
        </>
    );

    const toolbar = (
        <div className="table-toolbar">
            <div className="toolbar-left">
                <div className="filter-tabs">
                    <button className={`filter-tab${!showBanned && !showDeactivated ? ' active' : ''}`} onClick={() => { setShowBanned(false); setShowDeactivated(false); }}>Активни</button>
                    <button className={`filter-tab${showBanned ? ' active' : ''}`} onClick={() => { setShowBanned(true); setShowDeactivated(false); }}>Блокирани</button>
                    <button className={`filter-tab${showDeactivated ? ' active' : ''}`} onClick={() => { setShowBanned(false); setShowDeactivated(true); }}>Деактивирани</button>
                </div>
                <div className="search-input-wrap">
                    <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                        <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                    </svg>
                    <input type="text" className="form-input" placeholder="Търсене по потребителско име..." value={usernameFilter} onChange={(e) => setUsernameFilter(e.target.value)} />
                </div>
            </div>
            <div className="toolbar-right">
                {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)' }}>Зареждане...</span>}
            </div>
        </div>
    );

    const emptyOrError = error ? (
        <div className="data-table-wrapper">
            <div className="empty-state">
                <div className="empty-state-title">Грешка при зареждане на потребителите</div>
                <div className="empty-state-text">{error}</div>
            </div>
        </div>
    ) : users.length === 0 && !loading ? (
        <div className="data-table-wrapper">
            <div className="empty-state">
                <div className="empty-state-title">Няма намерени потребители</div>
                <div className="empty-state-text">{usernameFilter ? 'Няма потребители, съответстващи на търсенето' : 'Няма регистрирани потребители'}</div>
            </div>
        </div>
    ) : null;

    return (
        <div className="admin-shell admin-shell--users">
            <AdminSidebar activePage="users" />
            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Управление на потребители' }]} />
                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Управление на потребители</h1>
                        <p className="page-subtitle">{users.length} потребител{users.length !== 1 ? 'и' : ''} в платформата</p>
                    </div>

                    {toolbar}

                    {emptyOrError || (
                        <>
                            {/* Desktop table */}
                            <div className="data-table-wrapper">
                                <table className="data-table">
                                    <thead>
                                        <tr>
                                            <th>Потребителско име</th>
                                            <th>Имейл</th>
                                            <th>Роля</th>
                                            <th>Чакащи шифри</th>
                                            <th>Действия</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {users.map((user) => (
                                            <tr key={user.id}>
                                                <td className="user-name">{user.username}</td>
                                                <td className="user-email">{user.email}</td>
                                                <td><span className={`badge ${user.isAdmin ? 'badge-admin' : 'badge-standard'}`}>{user.role}</span></td>
                                                <td className="mono" style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>{user.pendingCiphers}</td>
                                                <td><div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap' }}>{renderActions(user)}</div></td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>

                            {/* Mobile card list */}
                            <div className="users-card-list">
                                {users.map((user) => (
                                    <div key={user.id} className="user-card">
                                        <div className="user-card__row">
                                            <span className="user-card__label">Потребител</span>
                                            <span className="user-card__value">{user.username}</span>
                                        </div>
                                        <div className="user-card__row">
                                            <span className="user-card__label">Роля</span>
                                            <span className={`badge ${user.isAdmin ? 'badge-admin' : 'badge-standard'}`}>{user.role}</span>
                                        </div>
                                        <div className="user-card__actions">
                                            {renderActions(user)}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </>
                    )}
                </div>
            </main>

            {banModal.open && (
                <div className="modal-backdrop" onClick={closeBanModal}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Блокиране на потребител: {banModal.username}</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6', marginBottom: '12px' }}>
                            Потребителят няма да може да достъпва платформата. Моля, въведете причина за това действие.
                        </p>
                        <div className="form-group">
                            <label className="form-label">Причина за блокиране <span style={{ color: 'var(--rose-500)' }}>*</span></label>
                            <textarea className="form-textarea" placeholder="Обяснете защо потребителят се блокира..." rows="4" value={banReason} onChange={(e) => setBanReason(e.target.value)} autoFocus />
                        </div>
                        <div className="modal-actions">
                            <button onClick={closeBanModal} className="btn btn-ghost">Отказ</button>
                            <button onClick={handleConfirmBan} className="btn btn-danger">Блокирай потребителя</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UsersManagement;