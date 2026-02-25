import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/manage-ciphers.css';
import { useError } from '../ErrorContext.jsx';
const API_BASE = 'http://localhost:5115/api/admin';
const AVAILABLE_TAGS = [          
    { value: 1, label: 'Изображение',mapValue : 'Image'},
    { value: 2, label: 'Пъзел', mapValue : 'Puzzle' },
    { value: 3, label: 'Исторически',mapValue : 'Historical'},
    { value: 4, label: 'Кратък',mapValue : 'Short'},
    { value: 5, label: 'Дълъг' ,mapValue : 'Long'},
    { value: 6, label: 'Подходящ за начинаещи' , mapValue : 'Beginner_Friendly'},
    { value: 7, label: 'Труден', mapValue : 'Tricky'},
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
    const { setError: setGlobalError } = useError();
    const [editModal, setEditModal] = useState({ open: false, cipher: null });
    const [editTitle, setEditTitle] = useState('');
    const [editAllowTypeHint, setEditAllowTypeHint] = useState(false);
    const [editAllowHint, setEditAllowHint] = useState(false);
    const [editAllowSolution, setEditAllowSolution] = useState(false);
    const [editSelectedTags, setEditSelectedTags] = useState([]);

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(searchTerm);
        }, 300);
        return () => clearTimeout(timer);
    }, [searchTerm]);

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
            setCiphers(Array.isArray(data) ? data : []);
            setEditAllowTypeHint(data.isTypeHintAllowed)
            setEditAllowHint(data.isHintAllowed)
            setEditAllowSolution(data.isSolutionAllowed)
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

    const openEditModal = useCallback((cipher) => {
        setEditModal({ open: true, cipher });
        setEditTitle(cipher.title || '');
        setEditAllowTypeHint(cipher.isTypeHintAllowed ?? false);
        setEditAllowHint(cipher.isHintAllowed ?? false);
        setEditAllowSolution(cipher.isSolutionAllowed ?? false);
        const tags = []
        cipher.tags.forEach(c =>
        {
            console.log(c)
           const tag = AVAILABLE_TAGS.find(t => t.mapValue == c)
           if(tag){
            tags.push(tag.value)
           }
        }
        )
        console.log(tags)
        setEditSelectedTags(tags);
    }, []);

    const closeEditModal = useCallback(() => {
        setEditModal({ open: false, cipher: null });
        setEditTitle('');
        setEditAllowTypeHint(false);
        setEditAllowHint(false);
        setEditAllowSolution(false);
        setEditSelectedTags([]);
    }, []);

    const handleEditTagToggle = useCallback((tagId) => {
        setEditSelectedTags(prev => 
            prev.includes(tagId) 
                ? prev.filter(id => id !== tagId)
                : [...prev, tagId]
        );
    }, []);

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
            closeEditModal();
            fetchCiphers();
        } catch (err) {
            setGlobalError(`Неуспешно обновяване на шифъра: ${err.response?.data?.error || err.message}`);
        }
    }, [editModal.cipher, editTitle, editAllowTypeHint, editAllowHint, editAllowSolution, editSelectedTags, closeEditModal, fetchCiphers]);

    const openDeleteModal = useCallback((id) => {
        setDeleteModal({ open: true, id });
    }, []);

    const closeDeleteModal = useCallback(() => {
        setDeleteModal({ open: false, id: null });
    }, []);

    const handleConfirmDelete = useCallback(async () => {
        if (!deleteModal.id) return;
        try {
            await axios.put(`${API_BASE}/cipher/${deleteModal.id}/delete`);
            closeDeleteModal();
            fetchCiphers();
        } catch (err) {
            console.error('Delete error:', err);
            alert(`Неуспешно изтриване: ${err.response?.data?.message || err.message}`);
        }
    }, [deleteModal.id, closeDeleteModal, fetchCiphers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="approved-ciphers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Управление на шифри' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Управление на шифри</h1>
                        <p className="page-subtitle">
                            {ciphers.length} одобрен{ciphers.length !== 1 ? 'и шифъра' : ' шифър'} в платформата
                        </p>
                    </div>

                    {/* Toolbar */}
                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="filter-tabs">
                                <button 
                                    className={`filter-tab${challengeTypeFilter === 0 ? ' active' : ''}`}
                                    onClick={() => setChallengeTypeFilter(0)}
                                >
                                    Стандартен
                                </button>
                                <button 
                                    className={`filter-tab${challengeTypeFilter === 1 ? ' active' : ''}`}
                                    onClick={() => setChallengeTypeFilter(1)}
                                >
                                    Експериментален
                                </button>
                            </div>

                            <div className="search-input-wrap">
                                <svg className="search-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                    <circle cx="7" cy="7" r="5"/><path d="M11 11l3 3"/>
                                </svg>
                                <input
                                    type="text"
                                    className="form-input"
                                    placeholder="Търсене по заглавие, вид..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                />
                            </div>
                        </div>
                        <div className="toolbar-right">
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
                                <option value="0">Най-нови</option>
                                <option value="1">Най-стари</option>
                                <option value="2">Най-решавани</option>
                                <option value="3">Най-малко решавани</option>
                            </select>
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)', marginLeft: '10px' }}>Зареждане...</span>}
                        </div>
                    </div>

                    {/* Table */}
                    {error ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">Грешка при зареждане на шифрите</div>
                                <div className="empty-state-text">{error}</div>
                            </div>
                        </div>
                    ) : ciphers.length === 0 && !loading ? (
                        <div className="data-table-wrapper">
                            <div className="empty-state">
                                <div className="empty-state-title">Няма намерени одобрени шифри</div>
                                <div className="empty-state-text">
                                    {searchTerm ? 'Няма резултати, съответстващи на търсенето' : 'Все още няма одобрени шифри'}
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="data-table-wrapper">
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Заглавие</th>
                                        <th>Вид</th>
                                        <th>ML увереност</th>
                                        <th>Предложен от</th>
                                        <th>Дата</th>
                                        <th>Действия</th>
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
                                                    {cipher.title || `Шифър #${cipher.id}`}
                                                </div>
                                                {cipher.isImage && (
                                                    <div className="image-tag" style={{ marginTop: '4px' }}>
                                                        <svg width="10" height="10" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                            <rect x="1" y="3" width="14" height="10" rx="2"/>
                                                            <circle cx="5.5" cy="7.5" r="1.5"/>
                                                            <path d="M1 11.5l4-3 3 2.5 2.5-2.5L15 11.5"/>
                                                        </svg>
                                                        ИЗОБРАЖЕНИЕ
                                                    </div>
                                                )}
                                            </td>

                                            <td>
                                                {cipher.mlPrediction ? (
                                                    <span className="mono" style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                                                        {cipher.mlPrediction}
                                                    </span>
                                                ) : (
                                                    <span style={{ color: 'var(--text-dim)', fontSize: '11px' }} >—</span>
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
                                                        title="Редактирай"
                                                        onClick={() => openEditModal(cipher)}
                                                    >
                                                        <svg width="13" height="13" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                            <path d="M11 2l3 3L5 14H2v-3L11 2z"/>
                                                        </svg>
                                                    </button>
                                                    <button
                                                        className="btn-icon"
                                                        title="Изтрий"
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
                        <div className="modal-title">Редактиране на шифър: {editModal.cipher?.title}</div>

                        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                            <div className="form-group">
                                <label className="form-label">Заглавие</label>
                                <input 
                                    type="text" 
                                    className="form-input" 
                                    value={editTitle}
                                    onChange={(e) => setEditTitle(e.target.value)}
                                />
                            </div>

                            <div className="form-group">
                                <label className="form-label">Разрешения за AI помощ</label>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', marginTop: '8px' }}>
                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowTypeHint}
                                            onChange={(e) => setEditAllowTypeHint(e.target.checked)} 
                                                                              
                                        />
                                        <span>Позволи подсказки за вида</span>
                                    </label>

                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowHint}
                                            onChange={(e) => setEditAllowHint(e.target.checked)}
                                           
                                        />
                                        <span>Позволи подсказки</span>
                                    </label>

                                    <label className="edit-permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={editAllowSolution}
                                            onChange={(e) => setEditAllowSolution(e.target.checked)}
                                      
                                        />
                                        <span>Позволи пълното решение</span>
                                    </label>
                                </div>
                            </div>

                            <div className="form-group">
                                <label className="form-label">Етикети</label>
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
                            <button onClick={closeEditModal} className="btn btn-ghost">Отказ</button>
                            <button onClick={handleSaveEdit} className="btn btn-primary">Запази промените</button>
                        </div>
                    </div>
                </div>
            )}

            {/* Delete Confirm Modal */}
            {deleteModal.open && (
                <div className="modal-backdrop" onClick={closeDeleteModal}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Изтриване на шифър?</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6' }}>
                            Това действие не е постоянно. Шифърът и цялата свързана история на решения ще могат да бъдат възстановени в бъдеще.
                        </p>
                        <div className="modal-actions">
                            <button onClick={closeDeleteModal} className="btn btn-ghost">Отказ</button>
                            <button onClick={handleConfirmDelete} className="btn btn-danger">Изтрий</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ManageApprovedCiphers;