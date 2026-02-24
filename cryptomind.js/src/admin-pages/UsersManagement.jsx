import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/users-management.css';
const API_BASE = 'http://localhost:5115/api/admin';

import {useError} from '../ErrorContext'
axios.defaults.withCredentials = true;



const UsersManagement = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const {setError: setGlobalError} = useError()
    const [usernameFilter, setUsernameFilter] = useState('');
    const [debouncedUsername, setDebouncedUsername] = useState('');
    const [showBanned, setShowBanned] = useState(false);
    const [showDeactivated, setShowDeactivated] = useState(false);
    const [banModal, setBanModal] = useState({ open: false, userId: null, username: '' });
    const [banReason, setBanReason] = useState('');

    // Debounce username filter
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedUsername(usernameFilter);
        }, 300);
        return () => clearTimeout(timer);
    }, [usernameFilter]);

    // Fetch users
    const fetchUsers = useCallback(async () => {
        try {
            setLoading(true);
            const params = {
                IsBanned: showBanned,
                IsDeactivated: showDeactivated,
                Username: debouncedUsername || ''
            };
            const { data } = await axios.get(`${API_BASE}/users`, { params });
            console.log('Users:', data);
            setUsers(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to fetch users:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedUsername, showBanned, showDeactivated]);

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

    // Promote to admin
    const handlePromote = useCallback(async (userId, username) => {
        

        try {
            await axios.put(`${API_BASE}/user/${userId}/admin`);
           
            fetchUsers();
        } catch (err) {
            console.error('Promote error:', err);
            setGlobalError(`Неуспешно повишаване: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchUsers]);

    // Demote from admin
    // const handleDemote = useCallback(async (userId, username) => {
    //     if (!window.confirm(`Demote ${username} to regular User?`)) return;

    //     try {
    //         await axios.put(`${API_BASE}/users/${userId}/demote`);
    //         alert(`${username} is now a regular User`);
    //         fetchUsers();
    //     } catch (err) {
    //         console.error('Demote error:', err);
    //         alert(`Failed to demote: ${err.response?.data?.message || err.message}`);
    //     }
    // }, [fetchUsers]);

    // Open ban modal
    const openBanModal = useCallback((userId, username) => {
        setBanModal({ open: true, userId, username });
        setBanReason('');
    }, []);

    // Confirm ban with reason
    const handleConfirmBan = useCallback(async () => {
        if (!banReason.trim()) {
             setGlobalError('Моля, въведете причина за блокирането на потребителя.');
            return;
        }

        try {
            const formData = new FormData();
            formData.append("reason", banReason);
        
            await axios.put(
                `http://localhost:5115/api/admin/user/${banModal.userId}/ban`,
                formData,
                {
                    headers: {
                        "Content-Type": "multipart/form-data"
                    },
                    withCredentials: true
                }
            );
            
            setBanModal({ open: false, userId: null, username: '' });
            setBanReason('');
            fetchUsers();
        } catch (err) {
            console.error('Ban error:', err);
            setGlobalError(`Неуспешно блокиране: ${err.response?.data?.message || err.message}`);
        }
    }, [banModal.userId, banModal.username, banReason, fetchUsers]);

    // Close ban modal
    const closeBanModal = useCallback(() => {
        setBanModal({ open: false, userId: null, username: '' });
        setBanReason('');
    }, []);

    // Unban user
    const handleUnban = useCallback(async (userId, username) => {
     
        try {
            await axios.put(`${API_BASE}/user/${userId}/unban`, { params: { id: banModal.userId } });
            
            fetchUsers();
        } catch (err) {
            console.error('Unban error:', err);
            setGlobalError(`Неуспешно деблокиране: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchUsers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="users" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Управление на потребители' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Управление на потребители</h1>
                        <p className="page-subtitle">
                            {users.length} потребител{users.length !== 1 ? 'и' : ''} в платформата
                        </p>
                    </div>

                    {/* Filters */}
                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="filter-tabs">
                                <button 
                                    className={`filter-tab${!showBanned && !showDeactivated ? ' active' : ''}`}
                                    onClick={() => {
                                        setShowBanned(false);
                                        setShowDeactivated(false);
                                    }}
                                >
                                    Активни
                                </button>
                                <button 
                                    className={`filter-tab${showBanned ? ' active' : ''}`}
                                    onClick={() => {
                                        setShowBanned(true);
                                        setShowDeactivated(false);
                                    }}
                                >
                                    Блокирани
                                </button>
                                <button 
                                    className={`filter-tab${showDeactivated ? ' active' : ''}`}
                                    onClick={() => {
                                        setShowBanned(false);
                                        setShowDeactivated(true);
                                    }}
                                >
                                    Деактивирани
                                </button>
                            </div>

                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input
                                    type="text"
                                    className="form-input"
                                    placeholder="Търсене по потребителско име..."
                                    value={usernameFilter}
                                    onChange={(e) => setUsernameFilter(e.target.value)}
                                />
                            </div>
                        </div>
                        <div className="toolbar-right">
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)' }}>Зареждане...</span>}
                        </div>
                    </div>

                    {/* Table */}
                    {error ? (
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
                                <div className="empty-state-text">
                                    {usernameFilter ? 'Няма потребители, съответстващи на търсенето' : 'Няма регистрирани потребители'}
                                </div>
                            </div>
                        </div>
                    ) : (
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
                                            <td className="user-name">
                                                {user.username}
                                            </td>

                                            <td className="user-email">
                                                {user.email}
                                            </td>

                                            <td>
                                                <span className={`badge ${user.isAdmin ? 'badge-admin' : 'badge-standard'}`}>
                                                    {user.role}
                                                </span>
                                            </td>

                                            <td className="mono" style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>
                                                {user.pendingCiphers}
                                            </td>

                                            <td>
                                                <div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap' }}>
                                                    {!user.isAdmin && !(showBanned || showDeactivated) && (
                                                        <button
                                                            onClick={() => handlePromote(user.id, user.username)}
                                                            className="btn btn-primary btn-sm"
                                                        >
                                                            Повиши до Администратор
                                                        </button>
                                                    )}

                                                    {showBanned && (
                                                        <button
                                                            onClick={() => handleUnban(user.id, user.username)}
                                                            className="btn btn-success btn-sm"
                                                        >
                                                            Деблокирай
                                                        </button>
                                                    )}{!user.isAdmin && !(showBanned) && !(showDeactivated) && ( 
                                                        <>
                                                            <button
                                                                onClick={() => openBanModal(user.id, user.username)}
                                                                className="btn btn-danger btn-sm"
                                                            >
                                                                Блокирай
                                                            </button>
                                                        </>                                                  
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </main>

            {/* Ban Modal */}
            {banModal.open && (
                <div className="modal-backdrop" onClick={closeBanModal}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Блокиране на потребител: {banModal.username}</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6', marginBottom: '12px' }}>
                            Потребителят няма да може да достъпва платформата. Моля, въведете причина за това действие.
                        </p>

                        <div className="form-group">
                            <label className="form-label">
                                Причина за блокиране <span style={{ color: 'var(--rose-500)' }}>*</span>
                            </label>
                            <textarea
                                className="form-textarea"
                                placeholder="Обяснете защо потребителят се блокира..."
                                rows="4"
                                value={banReason}
                                onChange={(e) => setBanReason(e.target.value)}
                                autoFocus
                            />
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