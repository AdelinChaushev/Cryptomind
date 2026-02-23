import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/pending-ciphers.css';

const API_BASE = 'http://localhost:5115/api/admin';

axios.defaults.withCredentials = true;

const PendingCiphers = () => {
    const [ciphers, setCiphers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(searchTerm);
        }, 300);

        return () => clearTimeout(timer);
    }, [searchTerm]);

    // Fetch ciphers
    const fetchCiphers = useCallback(async () => {
        try {
            setLoading(true);
            const params = debouncedSearch ? { filter: debouncedSearch } : {};
            const { data } = await axios.get(`${API_BASE}/pending-ciphers`, { params });
            setCiphers(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to fetch pending ciphers:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedSearch]);

    useEffect(() => {
        fetchCiphers();
    }, [fetchCiphers]);

    // Confidence bar color
    const getConfidenceClass = (pct) => {
        if (pct >= 85) return 'confidence-high';
        if (pct >= 65) return 'confidence-mid';
        return 'confidence-low';
    };

    // Refresh button handler
    useEffect(() => {
        const refreshBtn = document.getElementById('btn-refresh-pending');
        if (refreshBtn) {
            refreshBtn.addEventListener('click', fetchCiphers);
        }
        return () => {
            if (refreshBtn) refreshBtn.removeEventListener('click', fetchCiphers);
        };
    }, [fetchCiphers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="pending-ciphers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Pending Submissions' }]}>
                    <button className="btn btn-ghost btn-sm" id="btn-refresh-pending">
                        Refresh
                    </button>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Pending Submissions</h1>
                        <p className="page-subtitle">
                            {ciphers.length} submission{ciphers.length !== 1 ? 's' : ''} awaiting review
                        </p>
                    </div>

                    {/* Search Bar */}
                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input
                                    type="text"
                                    className="form-input"
                                    placeholder="Search by title, user, type..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
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
                                <div className="empty-state-title">Error loading ciphers</div>
                                <div className="empty-state-text">{error}</div>
                            </div>
                        </div>
                    ) : ciphers.length === 0 && !loading ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <svg className="empty-state-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="12" cy="12" r="10"/><path d="M12 8v5M12 16h.01"/>
                                </svg>
                                <div className="empty-state-title">No pending submissions</div>
                                <div className="empty-state-text">
                                    {searchTerm ? 'No results match your search' : 'All caught up!'}
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
                                        <th>LLM Required</th>
                                        <th>Submitted By</th>
                                        <th>Date</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {ciphers.map((cipher) => {
                                        const confClass = getConfidenceClass(cipher.percentageOfConfidence);

                                        return (
                                            <tr key={cipher.id}>
                                                <td className="mono" style={{ color: 'var(--text-dim)', fontSize: '11px' }}>
                                                    #{cipher.id}
                                                </td>

                                                <td>
                                                    <div style={{ fontWeight: 500, fontSize: '13px', color: 'var(--text-primary)' }}>
                                                        {cipher.title || <span style={{ color: 'var(--text-dim)', fontStyle: 'italic' }}>Untitled</span>}
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
                                                        <span style={{ color: 'var(--text-dim)', fontSize: '11px' }}>Not classified</span>
                                                    )}
                                                </td>

                                                <td>
                                                    {cipher.percentageOfConfidence !== null && cipher.percentageOfConfidence !== undefined ? (
                                                        <div className={`confidence-inline ${confClass}`}>
                                                            <div className="confidence-top">
                                                                <span className="confidence-pct">{cipher.percentageOfConfidence}%</span>
                                                            </div>
                                                            <div className="confidence-bar">
                                                                <div
                                                                    className="confidence-bar-fill"
                                                                    style={{ width: `${cipher.percentageOfConfidence}%` }}
                                                                />
                                                            </div>
                                                        </div>
                                                    ) : (
                                                        <span style={{ color: 'var(--text-dim)', fontSize: '11px' }}>—</span>
                                                    )}
                                                </td>

                                                <td>
                                                    <span className={`badge ${cipher.isLLMRecommended ? 'badge-pending' : 'badge-approved'}`}>
                                                        {cipher.isLLMRecommended ? 'Yes' : 'No'}
                                                    </span>
                                                </td>

                                                <td className="mono" style={{ fontSize: '12px' }}>
                                                    {cipher.submittedBy || '—'}
                                                </td>

                                                <td className="mono" style={{ fontSize: '10px', color: 'var(--text-dim)' }}>
                                                    {cipher.submittedAt || '—'}
                                                </td>

                                                <td>
                                                    <a
                                                        href={`/admin/cipher-review/${cipher.id}`}
                                                        className="btn btn-primary btn-sm"
                                                    >
                                                        Review →
                                                    </a>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    )}
                </div>
            </main>
        </div>
    );
};

export default PendingCiphers;