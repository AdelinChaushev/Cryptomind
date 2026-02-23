import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/users-management.css';
const API_BASE = 'http://localhost:5115/api/admin';

axios.defaults.withCredentials = true;



const UsersManagement = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
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
        if (!window.confirm(`Promote ${username} to Admin?`)) return;

        try {
            await axios.put(`${API_BASE}/user/${userId}/admin`);
            alert(`${username} is now an Admin!`);
            fetchUsers();
        } catch (err) {
            console.error('Promote error:', err);
            alert(`Failed to promote: ${err.response?.data?.message || err.message}`);
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
            alert('Please provide a reason for banning this user.');
            return;
        }

        try {
            const formData = new FormData();
            formData.append("reason", banReason);
        
            await axios.put(
                `http://localhost:5115/api/admin/user/${banModal.userId}/ban`,
                formData,
                {
                     // query parameter
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
            alert(`Failed to ban: ${err.response?.data?.message || err.message}`);
        }
    }, [banModal.userId, banModal.username, banReason, fetchUsers]);

    // Close ban modal
    const closeBanModal = useCallback(() => {
        setBanModal({ open: false, userId: null, username: '' });
        setBanReason('');
    }, []);

    // Unban user
    const handleUnban = useCallback(async (userId, username) => {
        if (!window.confirm(`Unban ${username}?`)) return;
        console.log(`Unbanning user ID: ${userId}, Username: ${username}`); 
        try {
            await axios.put(`${API_BASE}/user/${userId}/unban`,  {params: { id: banModal.userId }},);
            alert(`${username} has been unbanned`);
            fetchUsers();
        } catch (err) {
            console.error('Unban error:', err);
            alert(`Failed to unban: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchUsers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="users" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Users Management' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Users Management</h1>
                        <p className="page-subtitle">
                            {users.length} user{users.length !== 1 ? 's' : ''} on the platform
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
                                    Active
                                </button>
                                <button 
                                    className={`filter-tab${showBanned ? ' active' : ''}`}
                                    onClick={() => {
                                        setShowBanned(true);
                                        setShowDeactivated(false);
                                    }}
                                >
                                    Banned
                                </button>
                                <button 
                                    className={`filter-tab${showDeactivated ? ' active' : ''}`}
                                    onClick={() => {
                                        setShowBanned(false);
                                        setShowDeactivated(true);
                                    }}
                                >
                                    Deactivated
                                </button>
                            </div>

                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input
                                    type="text"
                                    className="form-input"
                                    placeholder="Search by username..."
                                    value={usernameFilter}
                                    onChange={(e) => setUsernameFilter(e.target.value)}
                                />
                            </div>
                        </div>
                        <div className="toolbar-right">
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)' }}>Loading...</span>}
                        </div>
                    </div>

                    {/* Table */}
                    {error ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">Error loading users</div>
                                <div className="empty-state-text">{error}</div>
                            </div>
                        </div>
                    ) : users.length === 0 && !loading ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">No users found</div>
                                <div className="empty-state-text">
                                    {usernameFilter ? 'No users match your search' : 'No users registered yet'}
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="data-table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Username</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                        <th>Pending Ciphers</th>
                                        <th>Actions</th>
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
                                                    {/* {user.isAdmin && (
                                                        <button
                                                            onClick={() => handleDemote(user.id, user.username)}
                                                            className="btn btn-ghost btn-sm"
                                                        >
                                                            Demote to User
                                                        </button>) }  */}
                                                    {!user.isAdmin && !(showBanned || showDeactivated) && (
                                                        <button
                                                            onClick={() => handlePromote(user.id, user.username)}
                                                            className="btn btn-primary btn-sm"
                                                        >
                                                            Promote to Admin
                                                        </button>
                                                    )}

                                                    {showBanned && (
                                                        <button
                                                            onClick={() => handleUnban(user.id, user.username)}
                                                            className="btn btn-success btn-sm"
                                                        >
                                                            Unban
                                                        </button>
                                                    ) }{!user.isAdmin && !(showBanned) && ( 
                                                           
                                                        <>
                                                         <button
                                                            onClick={() => openBanModal(user.id, user.username)}
                                                            className="btn btn-danger btn-sm"
                                                        >
                                                            Ban
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
                        <div className="modal-title">Ban User: {banModal.username}</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6', marginBottom: '12px' }}>
                            This user will no longer be able to access the platform. Please provide a reason for this action.
                        </p>

                        <div className="form-group">
                            <label className="form-label">
                                Ban Reason <span style={{ color: 'var(--rose-500)' }}>*</span>
                            </label>
                            <textarea
                                className="form-textarea"
                                placeholder="Explain why this user is being banned..."
                                rows="4"
                                value={banReason}
                                onChange={(e) => setBanReason(e.target.value)}
                                autoFocus
                            />
                        </div>

                        <div className="modal-actions">
                            <button onClick={closeBanModal} className="btn btn-ghost">Cancel</button>
                            <button onClick={handleConfirmBan} className="btn btn-danger">Ban User</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UsersManagement;