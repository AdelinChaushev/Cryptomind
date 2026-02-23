import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';

const API_BASE = 'http://localhost:5115/api/admin';

axios.defaults.withCredentials = true;
const AVAILABLE_TAGS = [
    { value: 0, label: 'None' },
    { value: 1, label: 'Image' },
    { value: 2, label: 'Puzzle' },
    { value: 3, label: 'Historical' },
    { value: 4, label: 'Short' },
    { value: 5, label: 'Long' },
    { value: 6, label: 'Beginner Friendly' },
    { value: 7, label: 'Tricky' },
];
const DeletedCiphers = () => {
    const [ciphers, setCiphers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');
    const [orderTerm, setOrderTerm] = useState(0);
    const [challengeTypeFilter, setChallengeTypeFilter] = useState(0);
    const [tagsFilter, setTagsFilter] = useState(0);

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => setDebouncedSearch(searchTerm), 300);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    // Fetch deleted ciphers
    const fetchCiphers = useCallback(async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
            
            // Fix: Check if tagsFilter is not 'None' (0) before appending
            if (tagsFilter !== 0) params.append('Tags', tagsFilter);
            
            params.append('ChallengeType', challengeTypeFilter);
            params.append('OrderTerm', orderTerm);
            
            if (debouncedSearch) params.append('SearchTerm', debouncedSearch);

            // Using axios params config is cleaner than manual string concatenation
            const { data } = await axios.get(`${API_BASE}/deleted-ciphers`, { params });
            
            setCiphers(Array.isArray(data) ? data : []);
            setError(null);
        } catch (err) {
            console.error('Fetch error:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedSearch, orderTerm, challengeTypeFilter, tagsFilter]); // All dependencies included

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
                            {/* Challenge Type Filter */}
                            <div className="filter-tabs">
                               
                                <button 
                                    className={`filter-tab${challengeTypeFilter === 0 ? ' active' : ''}`}
                                    onClick={() => setChallengeTypeFilter(0)}
                                >
                                    Standard
                                </button>
                                <button 
                                    className={`filter-tab${challengeTypeFilter === 1 ? ' active' : ''}`}
                                    onClick={() => setChallengeTypeFilter(1)}
                                >
                                    Experimental
                                </button>
                            </div>

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
                            {/* Tags Filter Dropdown */}
                            <select 
                                className="form-select" 
                                style={{ width: '140px' }}
                                value={tagsFilter}
                                onChange={(e) => setTagsFilter(parseInt(e.target.value))}
                            >
                                
                                {AVAILABLE_TAGS.map((tag) => (
                                    <option key={tag.value} value={tag.value} onClick={e => setFilters(tag.value)}>{tag.label}</option>)
                                )}
                                
                            </select>

                            <select 
                                className="form-select" 
                                style={{ width: '140px' }}
                                value={orderTerm}
                                onChange={(e) => setOrderTerm(parseInt(e.target.value))}
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