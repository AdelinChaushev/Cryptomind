import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';

const API_BASE = 'http://localhost:5115/api/admin';

axios.defaults.withCredentials = true;

const DeletedCiphers = () => {
    const [ciphers, setCiphers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');
    const [filters, setFilters] = useState({
        tags: [0],
        challengeType: 0,
        cipherDefinition: 0,
        orderTerm: 0
    });

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(searchTerm);
        }, 300);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    // Fetch deleted ciphers
    const fetchCiphers = useCallback(async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
            
            // Add tags as multiple query params
            filters.tags.forEach(tag => params.append('Tags', tag));
            params.append('ChallengeType', filters.challengeType);
            params.append('CipherDefinition', filters.cipherDefinition);
            params.append('OrderTerm', filters.orderTerm);
            if (debouncedSearch) params.append('SearchTerm', debouncedSearch);

            const { data } = await axios.get(`${API_BASE}/deleted-ciphers?${params.toString()}`);
            console.log('Deleted ciphers:', data);
            setCiphers(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to fetch deleted ciphers:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedSearch, filters]);

    useEffect(() => {
        fetchCiphers();
    }, [fetchCiphers]);

    // Restore cipher
    const handleRestore = useCallback(async (id, title) => {
        if (!window.confirm(`Restore cipher "${title}"?`)) return;

        try {
            await axios.put(`${API_BASE}/cipher/${id}/restore`);
            alert('Cipher restored successfully!');
            fetchCiphers();
        } catch (err) {
            console.error('Restore error:', err);
            alert(`Failed to restore: ${err.response?.data?.message || err.message}`);
        }
    }, [fetchCiphers]);

    // Permanently delete
    // const handlePermanentDelete = useCallback(async (id, title) => {
    //     if (!window.confirm(`PERMANENTLY delete cipher "${title}"? This action cannot be undone!`)) return;

    //     try {
    //         await axios.put(`${API_BASE}/cipher/${id}/delete`);
    //         alert('Cipher permanently deleted');
    //         fetchCiphers();

    //     } catch (err) {
    //         console.error('Permanent delete error:', err);
    //         alert(`Failed to delete: ${err.response?.data?.message || err.message}`);
    //     }
    // }, [fetchCiphers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="deleted-ciphers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Deleted Ciphers' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Deleted Ciphers</h1>
                        <p className="page-subtitle">
                            {ciphers.length} deleted cipher{ciphers.length !== 1 ? 's' : ''}
                        </p>
                    </div>

                    {/* Toolbar */}
                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input
                                    type="text"
                                    className="form-input"
                                    placeholder="Search by title, type..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                />
                            </div>
                        </div>
                        <div className="toolbar-right">
                            <select 
                                className="form-select" 
                                style={{ width: '140px' }}
                                value={filters.orderTerm}
                                onChange={(e) => setFilters(prev => ({ ...prev, orderTerm: parseInt(e.target.value) }))}
                            >
                                <option value="0">Newest First</option>
                                <option value="1">Oldest First</option>
                                <option value="2">Most Solved</option>
                                <option value="3">Least Solved</option>
                            </select>
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)', marginLeft: '10px' }}>Loading...</span>}
                        </div>
                    </div>

                    {/* Table */}
                    {error ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">Error loading ciphers</div>
                                <div className="empty-state-text">{error}</div>
                            </div>
                        </div>
                    ) : ciphers.length === 0 && !loading ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">No deleted ciphers found</div>
                                <div className="empty-state-text">
                                    {searchTerm ? 'No results match your search' : 'No ciphers have been deleted'}
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="data-table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Title</th>
                                        <th>Type</th>
                                        <th>ML Confidence</th>
                                        <th>Submitted By</th>
                                        <th>Date</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {ciphers.map((cipher) => (
                                        <tr key={cipher.id}>
                                            <td className="mono" style={{ color: 'var(--text-dim)', fontSize: '11px' }}>
                                                #{cipher.id}
                                            </td>

                                            <td>
                                                <div className="cipher-title-cell">
                                                    {cipher.title || `Cipher #${cipher.id}`}
                                                </div>
                                                {cipher.isImage && (
                                                    <div className="image-tag" style={{ marginTop: '4px' }}>
                                                        <svg width="10" height="10" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                            <rect x="1" y="3" width="14" height="10" rx="2"/>
                                                            <circle cx="5.5" cy="7.5" r="1.5"/>
                                                            <path d="M1 11.5l4-3 3 2.5 2.5-2.5L15 11.5"/>
                                                        </svg>
                                                        IMAGE
                                                    </div>
                                                )}
                                            </td>

                                            <td>
                                                {cipher.mlPrediction ? (
                                                    <span className="mono" style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                                                        {cipher.mlPrediction}
                                                    </span>
                                                ) : (
                                                    <span style={{ color: 'var(--text-dim)', fontSize: '11px' }}>—</span>
                                                )}
                                            </td>

                                            <td>
                                                {cipher.percentageOfConfidence ? (
                                                    <span className="mono" style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                                                        {cipher.percentageOfConfidence}%
                                                    </span>
                                                ) : (
                                                    <span style={{ color: 'var(--text-dim)', fontSize: '11px' }}>—</span>
                                                )}
                                            </td>

                                            <td className="mono" style={{ fontSize: '12px' }}>
                                                {cipher.submittedBy || '—'}
                                            </td>

                                            <td className="mono" style={{ fontSize: '10px', color: 'var(--text-dim)' }}>
                                                {cipher.submittedAt || '—'}
                                            </td>

                                            <td>
                                                <div className="action-group">
                                                    <button
                                                        className="btn btn-success btn-sm"
                                                        onClick={() => handleRestore(cipher.id, cipher.title)}
                                                    >
                                                        Restore
                                                    </button>
                                                    {/* <button
                                                        className="btn btn-danger btn-sm"
                                                        onClick={() => handlePermanentDelete(cipher.id, cipher.title)}
                                                    >
                                                        Delete Forever
                                                    </button> */}
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
        </div>
    );
};

export default DeletedCiphers;