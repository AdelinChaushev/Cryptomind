
import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/manage-ciphers.css';
const API_BASE = 'http://localhost:5115/api/admin';
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
axios.defaults.withCredentials = true;


const ManageApprovedCiphers = () => {
    const [ciphers, setCiphers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');
    const [orderTerm, setOrderTerm] = useState(0);
    const [challengeTypeFilter, setChallengeTypeFilter] = useState(0);
    const [tagsFilter, setTagsFilter] = useState(0);
    const [deleteModal, setDeleteModal] = useState({ open: false, id: null });
    
    // Edit modal state
    const [editModal, setEditModal] = useState({ open: false, cipher: null });
    const [editTitle, setEditTitle] = useState('');
    const [editAllowTypeHint, setEditAllowTypeHint] = useState(false);
    const [editAllowHint, setEditAllowHint] = useState(false);
    const [editAllowSolution, setEditAllowSolution] = useState(false);
    const [editSelectedTags, setEditSelectedTags] = useState([]);

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
            const params = {
                SearchTerm: debouncedSearch || '',
                Tags: tagsFilter,
                ChallengeType: challengeTypeFilter,
                CipherDefinition: 0,
                OrderTerm: orderTerm
            };
            const { data } = await axios.get(`${API_BASE}/approved-ciphers`, { params });
            setError(null);
           // console.log('Approved ciphers:', data);
            setCiphers(Array.isArray(data) ? data : []);
        } catch (err) {
            console.error('Failed to fetch approved ciphers:', err);
            setError(err.response?.data?.message || err.message);
        } finally {
            setLoading(false);
        }
    }, [debouncedSearch, orderTerm, challengeTypeFilter, tagsFilter]);

    useEffect(() => {
        fetchCiphers();
    }, [fetchCiphers]);

    // Open edit modal
    const openEditModal = useCallback((cipher) => {
        setEditModal({ open: true, cipher });
        setEditTitle(cipher.title || '');
        setEditAllowTypeHint(cipher.allowType ?? false);
        setEditAllowHint(cipher.allowHint ?? false);
        setEditAllowSolution(cipher.allowFullSolution ?? false);
        setEditSelectedTags([]); // TODO: Get existing tags from cipher if available
    }, []);

    // Close edit modal
    const closeEditModal = useCallback(() => {
        setEditModal({ open: false, cipher: null });
        setEditTitle('');
        setEditAllowTypeHint(false);
        setEditAllowHint(false);
        setEditAllowSolution(false);
        setEditSelectedTags([]);
    }, []);

    // Toggle tag selection in edit modal
    const handleEditTagToggle = useCallback((tagId) => {
        setEditSelectedTags(prev => 
            prev.includes(tagId) 
                ? prev.filter(id => id !== tagId)
                : [...prev, tagId]
        );
    }, []);

    // Save edit
    const handleSaveEdit = useCallback(async () => {
        if (!editModal.cipher?.id) return;

        try {
            await axios.put(`${API_BASE}/cipher/${editModal.cipher.id}/update`, {
                title: editTitle,
                allowTypeHint: editAllowTypeHint,
                allowHint: editAllowHint,
                allowSolution: editAllowSolution,
                tagIds: editSelectedTags
            });
        //    alert('Cipher updated successfully');
            closeEditModal();
            fetchCiphers();
        } catch (err) {
            console.error('Edit error:', err);
            alert(`Failed to update: ${err.response?.data?.message || err.message}`);
        }
    }, [editModal.cipher, editTitle, editAllowTypeHint, editAllowHint, editAllowSolution, editSelectedTags, closeEditModal, fetchCiphers]);

    // Open delete modal
    const openDeleteModal = useCallback((id) => {
        setDeleteModal({ open: true, id });
    }, []);

    // Close delete modal
    const closeDeleteModal = useCallback(() => {
        setDeleteModal({ open: false, id: null });
    }, []);

    // Confirm delete
    const handleConfirmDelete = useCallback(async () => {
        if (!deleteModal.id) return;

        try {
            await axios.put(`${API_BASE}/cipher/${deleteModal.id}/delete`);
         //   alert('Cipher deleted successfully');
            closeDeleteModal();
            fetchCiphers();
        } catch (err) {
            console.error('Delete error:', err);
            alert(`Failed to delete: ${err.response?.data?.message || err.message}`);
        }
    }, [deleteModal.id, closeDeleteModal, fetchCiphers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="approved-ciphers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Manage Ciphers' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Manage Ciphers</h1>
                        <p className="page-subtitle">
                            {ciphers.length} approved cipher{ciphers.length !== 1 ? 's' : ''} on the platform
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
                                    <option key={tag.value} value={tag.value} onClick={e => setTagsFilter(tag.value)}>{tag.label}</option>)
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
                                <div className="empty-state-title">No approved ciphers found</div>
                                <div className="empty-state-text">
                                    {searchTerm ? 'No results match your search' : 'No ciphers have been approved yet'}
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
                                                        className="btn-icon"
                                                        title="Edit"
                                                        onClick={() => openEditModal(cipher)}
                                                    >
                                                        <svg width="13" height="13" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                            <path d="M11 2l3 3L5 14H2v-3L11 2z"/>
                                                        </svg>
                                                    </button>
                                                    <button
                                                        className="btn-icon"
                                                        title="Delete"
                                                        onClick={() => openDeleteModal(cipher.id)}
                                                        style={{ color: 'var(--rose-500)', borderColor: 'rgba(244,63,94,0.2)' }}
                                                    >
                                                        <svg width="13" height="13" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                            <path d="M2 4h12M5 4V2h6v2M6 7v5M10 7v5M3 4l1 10h8l1-10"/>
                                                        </svg>
                                                    </button>
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

            {/* Edit Modal */}
            {editModal.open && (
                <div className="modal-backdrop" onClick={closeEditModal}>
                    <div className="modal-box modal-box-wide" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Edit Cipher: {editModal.cipher?.title}</div>

                        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                            {/* Title */}
                            <div className="form-group">
                                <label className="form-label">Title</label>
                                <input 
                                    type="text" 
                                    className="form-input" 
                                    value={editTitle}
                                    onChange={(e) => setEditTitle(e.target.value)}
                                />
                            </div>

                            {/* AI Assistance Permissions */}
                            <div className="form-group">
                                <label className="form-label">AI Assistance Permissions</label>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', marginTop: '8px' }}>
                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowTypeHint}
                                            onChange={(e) => setEditAllowTypeHint(e.target.checked)}
                                        />
                                        <span>Allow Type Hint</span>
                                    </label>

                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowHint}
                                            onChange={(e) => setEditAllowHint(e.target.checked)}
                                        />
                                        <span>Allow Hints</span>
                                    </label>

                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowSolution}
                                            onChange={(e) => setEditAllowSolution(e.target.checked)}
                                        />
                                        <span>Allow Full Solution</span>
                                    </label>
                                </div>
                            </div>

                            {/* Tags */}
                            <div className="form-group">
                                <label className="form-label">Tags</label>
                                <div className="edit-tag-cloud" style={{ marginTop: '8px' }}>
                                    {AVAILABLE_TAGS.map((tag) => (
                                        <button
                                            key={tag.value}
                                            type="button"
                                            className={`edit-tag-chip${editSelectedTags.includes(tag.value) ? ' tag-selected' : ''}`}
                                            onClick={() => handleEditTagToggle(tag.value)}
                                        >
                                            {tag.label}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        </div>

                        <div className="modal-actions">
                            <button onClick={closeEditModal} className="btn btn-ghost">Cancel</button>
                            <button onClick={handleSaveEdit} className="btn btn-primary">Save Changes</button>
                        </div>
                    </div>
                </div>
            )}

            {/* Delete Confirm Modal */}
            {deleteModal.open && (
                <div className="modal-backdrop" onClick={closeDeleteModal}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Delete Cipher?</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6' }}>
                            This action is not permanent. The cipher and all associated solve history will be able to be restored in the future.
                        </p>
                        <div className="modal-actions">
                            <button onClick={closeDeleteModal} className="btn btn-ghost">Cancel</button>
                            <button onClick={handleConfirmDelete} className="btn btn-danger">Delete</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ManageApprovedCiphers;