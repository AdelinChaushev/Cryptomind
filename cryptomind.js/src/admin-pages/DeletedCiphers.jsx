import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';

import { useError } from '../ErrorContext.jsx';
const API_BASE = `${import.meta.env.VITE_API_URL}/api/admin`;

axios.defaults.withCredentials = true;
const AVAILABLE_TAGS = [          
    { value: 1, label: 'Изображение',mapValue : 'Image'},
    { value: 2, label: 'Пъзел', mapValue : 'Puzzle' },
    { value: 3, label: 'Исторически',mapValue : 'Historical'},
    { value: 4, label: 'Кратък',mapValue : 'Short'},
    { value: 5, label: 'Дълъг' ,mapValue : 'Long'},
    { value: 6, label: 'Подходящ за начинаещи' , mapValue : 'Beginner_Friendly'},
    { value: 7, label: 'Труден', mapValue : 'Tricky'},
];
const DeletedCiphers = () => {
    const [ciphers, setCiphers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');
    const { setError: setGlobalError } = useError();
    const [filters, setFilters] = useState({
        tags: [0],
        challengeType: 0,
        cipherDefinition: 0,
        orderTerm: 0
    });
    const [restoreModal, setRestoreModal] = useState({ open: false, cipher: null });
    const [renameModal, setRenameModal] = useState({ 
        open: false, 
        cipher: null, 
        newTitle: '' 
    });

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(searchTerm);
        }, 300);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    const fetchCiphers = useCallback(async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
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
            setGlobalError('Неуспешно зареждане на изтритите шифри. Моля, опитайте отново.');
        } finally {
            setLoading(false);
        }
    }, [debouncedSearch, filters]);

    useEffect(() => {
        fetchCiphers();
    }, [fetchCiphers]);

    const handleRestore = useCallback((id, title) => {
        setRestoreModal({ open: true, cipher: { id, title } });
    }, []);

    const handleConfirmRestore = useCallback(async () => {
        const { id, title } = restoreModal.cipher;
        setRestoreModal({ open: false, cipher: null });
        try {
            await axios.put(`${API_BASE}/cipher/${id}/restore`);
            fetchCiphers();
        } catch (err) {
            const errorMessage = err.response?.data?.error || err.error;
            if (err.response?.status === 409) {
                setRenameModal({ open: true, cipher: { id, title }, newTitle: title });
            } else {
                setGlobalError(errorMessage || 'Грешка при възстановяване');
            }
        }
    }, [restoreModal, fetchCiphers]);

    const handleRestoreWithNewTitle = useCallback(async () => {
        if (!renameModal.cipher) return;
        
        const newTitle = renameModal.newTitle.trim();
        if (!newTitle) {
            alert('Моля, въведете ново заглавие');
            return;
        }

        try {
            await axios.put(`${API_BASE}/cipher/${renameModal.cipher.id}/restore?newTitle=${encodeURIComponent(newTitle)}`);
            setRenameModal({ open: false, cipher: null, newTitle: '' });
            fetchCiphers();
        } catch (err) {
            console.error('Restore with rename error:', err);
            setGlobalError(`Неуспешно възстановяване: ${err.response?.data?.error}`);
        }
    }, [renameModal, fetchCiphers]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="deleted-ciphers" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Изтрити шифри' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Изтрити шифри</h1>
                        <p className="page-subtitle">
                            {ciphers.length} изтрит{ciphers.length !== 1 ? 'и шифъра' : ' шифър'}
                        </p>
                    </div>

                    
                    <div className="table-toolbar">
                        <div className="toolbar-left">
                            <div className="filter-tabs">
                               
                                <button 
                                    className={`filter-tab${filters.challengeType === 0 ? ' active' : ''}`}
                                    onClick={() => setFilters(prev => ({
                                        ...prev,
                                        challengeType : 0
                                    }))}
                                >
                                    Стандартен
                                </button>
                                <button 
                                    className={`filter-tab${filters.challengeType === 1 ? ' active' : ''}`}
                                    onClick={() => setFilters(prev => ({ ...prev,
                                        challengeType : 1}))}
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
                                value={filters.tags[0]}
                                onChange={ e => setFilters(prev => ({
                                    ...prev,
                                   tags : [parseInt(e.target.value)]

                                }))}
                               
                            >
                                <option value="0" onClick={e => 
                                        setFilters(prev => (
                                        { ...prev, tags : [tag.value]}))}>Никакъв</option>
                                {AVAILABLE_TAGS.map((tag) => (
                                    <option key={tag.value} value={tag.value} onClick={e =>
                                      
                                        setFilters(prev => (
                                        { ...prev,
                                         tags : [tag.value]}
                                        ))}>{tag.label}</option>)
                                )}
                                
                            </select>
                            <select 
                                className="form-select" 
                                style={{ width: '140px' }}
                                value={filters.orderTerm}
                                onChange={(e) => setFilters(prev => ({ ...prev, orderTerm: parseInt(e.target.value) }))}
                            >
                                <option value="0">Най-нови</option>
                                <option value="1">Най-стари</option>
                                <option value="2">Най-решавани</option>
                                <option value="3">Най-малко решавани</option>
                            </select>
                            {loading && <span style={{ fontSize: '11px', color: 'var(--text-dim)', marginLeft: '10px' }}>Зареждане...</span>}
                        </div>
                    </div>

                    
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
                                <div className="empty-state-title">Няма намерени изтрити шифри</div>
                                <div className="empty-state-text">
                                    {searchTerm ? 'Няма резултати, съответстващи на търсенето' : 'Няма изтрити шифри'}
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
                                                {cipher.cipherType ? (
                                                    <span className="mono" style={{ fontSize: '12px', color: 'var(--text-secondary)' }}>
                                                        {cipher.cipherType}
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
                                                        Възстанови
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

            
            {restoreModal.open && (
                <div className="modal-backdrop" onClick={() => setRestoreModal({ open: false, cipher: null })}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Възстановяване на шифър?</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6' }}>
                            Шифърът <strong style={{ color: 'var(--text-primary)' }}>"{restoreModal.cipher?.title}"</strong> ще бъде възстановен и ще стане видим отново.
                        </p>
                        <div className="modal-actions">
                            <button onClick={() => setRestoreModal({ open: false, cipher: null })} className="btn btn-ghost">Отказ</button>
                            <button onClick={handleConfirmRestore} className="btn btn-success">Възстанови</button>
                        </div>
                    </div>
                </div>
            )}

            
            {renameModal.open && (
                <div className="modal-backdrop" onClick={() => setRenameModal({ open: false, cipher: null, newTitle: '' })}>
                    <div className="modal-box" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-title">Заглавието вече съществува</div>
                        <p style={{ fontSize: '13px', color: 'var(--text-tertiary)', lineHeight: '1.6' }}>
                            Шифър със заглавие <strong style={{ color: 'var(--text-primary)' }}>"{renameModal.cipher?.title}"</strong> вече съществува. Въведете ново заглавие за да го възстановите.
                        </p>
                        <div className="form-group" style={{ marginTop: '16px' }}>
                            <label className="form-label">Ново заглавие</label>
                            <input
                                type="text"
                                className="form-input"
                                value={renameModal.newTitle}
                                onChange={(e) => setRenameModal(prev => ({ ...prev, newTitle: e.target.value }))}
                                placeholder="Въведете уникално заглавие..."
                                autoFocus
                            />
                        </div>
                        <div className="modal-actions">
                            <button onClick={() => setRenameModal({ open: false, cipher: null, newTitle: '' })} className="btn btn-ghost">Отказ</button>
                            <button onClick={handleRestoreWithNewTitle} className="btn btn-success">Възстанови</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default DeletedCiphers;