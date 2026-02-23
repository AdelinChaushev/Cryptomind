import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import LlmAssistantSection from './LlmAssistantSection';
import '../styles/cipher-review.css';
const API_BASE = 'http://localhost:5115/api/admin';
import { useParams } from "react-router-dom";
// Configure axios globally
import "../styles/cipher-review.css";
import { useNavigate } from 'react-router-dom';
import { useError } from '../ErrorContext';
axios.defaults.withCredentials = true;



const AVAILABLE_TAGS = [
    { id: 1, label: 'Image' },
    { id: 2, label: 'Puzzle' },
    { id: 3, label: 'Historical' },
    { id: 4, label: 'Short' },
    { id: 5, label: 'Long' },
    { id: 6, label: 'Beginner Friendly' },
    { id: 7, label: 'Tricky' }
];
const CIPHER_TYPES = [
    { value: '0',  label: 'Caesar',              group: 'Substitution' },
    { value: '1',  label: 'Atbash',              group: 'Substitution' },
    { value: '2',  label: 'Simple Substitution', group: 'Substitution' },
    { value: '3',  label: 'ROT13',               group: 'Substitution' },
    { value: '4',  label: 'Vigenere',            group: 'Polyalphabetic' },
    { value: '5',  label: 'Autokey',             group: 'Polyalphabetic' },
    { value: '6',  label: 'Trithemius',          group: 'Polyalphabetic' },
    { value: '7',  label: 'Rail Fence',          group: 'Transposition' },
    { value: '8',  label: 'Columnar',            group: 'Transposition' },
    { value: '9',  label: 'Route',               group: 'Transposition' },
    { value: '10', label: 'Base64',              group: 'Encoding' },
    { value: '11', label: 'Morse',               group: 'Encoding' },
    { value: '12', label: 'Binary',              group: 'Encoding' },
    { value: '13', label: 'Hex',                 group: 'Encoding' },
];


const CipherReview = () => {
    const [cipher, setCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    const [localError, setLocalError] = useState(null);
    const navigate = useNavigate();
    // LLM state
    const {setError} = useError();
    const [llmResult, setLlmResult] = useState(null);
    const [isLlmLoading, setIsLlmLoading] = useState(false);
    
    // Form state
    const [title, setTitle] = useState('');
    const [selectedTags, setSelectedTags] = useState([]);
    const [allowHint, setAllowHint] = useState(true);
    const [allowSolutionHint, setAllowSolutionHint] = useState(false);
    const [allowSolution, setAllowSolution] = useState(false);
    const [cipherType, setCipherType] = useState(0);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectReason, setRejectReason] = useState('');
    const cipherId = useParams().id;

    // Fetch cipher
    useEffect(() => {
        const fetchCipher = async () => {
            if (!cipherId || isNaN(cipherId)) {
                setLocalError('Invalid cipher ID');
                setLoading(false);
                return;
            }          
                axios.get(`${API_BASE}/cipher/${cipherId}`)
                .then(res => {
                    setCipher(res.data);
                    setTitle(res.data.title || '');
                    setAllowHint(res.data.allowTypeHint ?? false);
                    setAllowSolutionHint(res.data.allowHint ?? false);
                    setAllowSolution(res.data.allowSolution ?? false);
                    console.log('Fetched cipher:', res.data);
                }).catch(err => {
                    const status = err.response?.status;
                    const serverMessage = err?.data?.title;
                    switch (status) {
                        case 404:
                            setLocalError('Cipher not found');
                             navigate('/not-found');
                            break;
                        default:
                            setLocalError(serverMessage || 'Failed to fetch cipher')
                            setError(serverMessage || 'Failed to fetch cipher')
                            ;
                            break;    
                    };
                console.error('Failed to fetch cipher:', err);
                setLocalError(err.response?.data?.message || err.message)}
                )
                .finally(() => setLoading(false));
        };
        fetchCipher();
    }, [cipherId]);

    // Run LLM analysis
    const handleRunLlm = useCallback(async () => {
        setIsLlmLoading(true);
        try {
            const { data } = await axios.get(`${API_BASE}/cipher/${cipherId}/analyze`);
            setLlmResult(data);
        } catch (err) {
            setError(err.response?.data?.title || 'LLM анализът пропадна');
        } finally {
            setIsLlmLoading(false);
        }
    }, [cipherId, setError]);

    // Превключване на тагове
    const handleTagToggle = useCallback((tagId) => {
        setSelectedTags(prev => 
            prev.includes(tagId) ? prev.filter(id => id !== tagId) : [...prev, tagId]
        );
    }, []);

    // Одобрение
    const handleApprove = async () => {
        try {
            await axios.put(`${API_BASE}/cipher/${cipherId}/approve`, {
                title: title,
                allowTypeHint: cipher.allowType,
                allowHint: allowHint,
                allowSolutionHint: allowSolutionHint,
                allowSolution: allowSolution,
                typeOfCipher: cipherType,
                tagIds: selectedTags
            });
            navigate('/admin/pending-ciphers'); 
        } catch (err) {
            setError(`Одобрението пропадна: ${err.response?.data.error || err.message}`);
        }
    }

    // Отхвърляне
    const handleReject = useCallback(async () => {
        if (!rejectReason.trim()) {
            setError('Please provide a reason for rejecting this cipher.');
            return;
        }
        try {
            await axios.put(`${API_BASE}/cipher/${cipherId}/reject`, rejectReason, {
                headers: { 'Content-Type': 'application/json' }
            });
            navigate('/admin/pending-ciphers');
        } catch (err) {

            setError(`Rejection failed: ${err.response?.data?.message || err.message}`);
        }
    }, [cipherId, rejectReason, navigate]);

    if (loading) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-ciphers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">Зареждане...</div>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    if (localError || !cipher) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-ciphers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">{localError || 'Шифърът не е намерен'}</div>
                            <a href="/admin/pending-ciphers" className="btn btn-ghost btn-sm" style={{ marginTop: '12px' }}>
                                ← Обратно към изчакващи
                            </a>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="pending-ciphers" />
            <main className="admin-main">
                <AdminTopbar
                    breadcrumbs={[
                        { label: 'Изчакващи предложения', href: '/admin/pending-ciphers' },
                        { label: cipher.title || `Шифър #${cipher.id}` }
                    ]}
                >
                    <span style={{ fontFamily: 'var(--font-mono)', fontSize: '10px', color: 'var(--text-dim)' }}>
                        #{cipher.id} — изпратено от {cipher.submittedBy}
                    </span>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">{cipher.title || `Шифър #${cipher.id}`}</h1>
                        <p className="page-subtitle">Прегледайте съдържанието, проверете ML анализа и вземете решение</p>
                    </div>

                    <div className="review-layout">
                        <div className="review-main">
                            {/* 1. Съдържание */}
                            <div className="admin-card review-card">
                                <div className="admin-card-header">
                                    <div className="review-card-title-row">
                                        <span className="review-section-tag">ВХОД</span>
                                        <span className="admin-card-title">Подадено съдържание</span>
                                    </div>
                                    {cipher.isImage && <span className="badge badge-standard">Има изображение</span>}
                                </div>

                                <div className="form-label">Шифрован текст</div>
                                <div className="cipher-text-display prominent">{cipher.cipherText}</div>

                                {cipher.isImage && cipher.imageBase64 && (
                                    <div className="cipher-image-preview">
                                        <div className="form-label">Прикачено изображение</div>
                                        <img src={`${cipher.imageBase64}`} alt="Шифър" />
                                        <div className="ocr-note">↑ Текстът беше извлечен чрез OCR от това изображение</div>
                                    </div>
                                )}

                                {cipher.decryptedText && (
                                    <div style={{ marginTop: '14px' }}>
                                        <div className="form-label">Подадено решение (чист текст)</div>
                                        <div className="solution-display">{cipher.decryptedText}</div>
                                    </div>
                                )}

                                <div className="cipher-meta-row">
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Изпратено от</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedBy}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Дата</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedAt || '—'}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Дължина</span>
                                        <span className="cipher-meta-value text-mono">
                                            {cipher.cipherText?.length ?? 0} симв.
                                            {cipher.cipherText?.length < 150 && (
                                                <span style={{ color: 'var(--rose-500)', marginLeft: '6px' }}>⚠ Под 150</span>
                                            )}
                                        </span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Има решение</span>
                                        <span className={`cipher-meta-value ${cipher.decryptedText ? 'text-emerald' : 'text-dim'}`}>
                                            {cipher.decryptedText ? 'Да' : 'Не'}
                                        </span>
                                    </div>
                                </div>
                            </div>

                            {/* 2. ML Анализ */}
                            <div className="admin-card review-card">
                                <div className="admin-card-header">
                                    <div className="review-card-title-row">
                                        <span className="review-section-tag">ML</span>
                                        <span className="admin-card-title">ML Анализ</span>
                                    </div>
                                    <span className={`badge ${cipher.percentageOfConfidence >= 85 ? 'badge-approved' : cipher.percentageOfConfidence >= 65 ? 'badge-pending' : 'badge-rejected'}`}>
                                        {cipher.percentageOfConfidence}% увереност
                                    </span>
                                </div>
                                <div className="ml-result-grid">
                                    <div className="ml-metric">
                                        <span className="ml-metric-label">Прогнозиран тип</span>
                                        <span className="ml-metric-value">{cipher.mlPrediction || '—'}</span>
                                    </div>
                                    <div className="ml-metric">
                                        <span className="ml-metric-label">LLM препоръка</span>
                                        <span className={`badge ${cipher.isLLMRecommended ? 'badge-pending' : 'badge-approved'}`}>
                                            {cipher.isLLMRecommended ? 'Да' : 'Не'}
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <LlmAssistantSection result={llmResult} isLoading={isLlmLoading} onRunAnalysis={handleRunLlm} />
                        </div>

                        <div className="actions-column">
                            <div className="admin-card">
                                <div className="admin-card-header"><span className="admin-card-title">Детайли</span></div>
                                <div className="form-group">
                                    <label className="form-label">Заглавие</label>
                                    <input type="text" className="form-input" value={title} onChange={(e) => setTitle(e.target.value)} />
                                </div>
                            </div>

                            {cipher.decryptedText && (
                                <div className="admin-card">
                                    <div className="admin-card-header"><span className="admin-card-title">AI Помощ (Настройки)</span></div>
                                    <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                                        <label className="permission-toggle">
                                            <input type="checkbox" checked={allowHint} onChange={(e) => setAllowHint(e.target.checked)} />
                                            <span>Подсказки за типа</span>
                                        </label>
                                        <label className="permission-toggle">
                                            <input type="checkbox" checked={allowSolutionHint} onChange={(e) => setAllowSolutionHint(e.target.checked)} />
                                            <span>Подсказки за решението</span>
                                        </label>
                                        <label className="permission-toggle">
                                            <input type="checkbox" checked={allowSolution} onChange={(e) => setAllowSolution(e.target.checked)} />
                                            <span>Пълно решение</span>
                                        </label>
                                    </div>
                                </div>
                            )}

                            <div className="admin-card">
                                <div className="admin-card-header"><span className="admin-card-title">Етикети (Tags)</span></div>
                                <div className="tag-cloud">
                                    {AVAILABLE_TAGS.map((tag) => (
                                        <button key={tag.id} className={`tag-chip${selectedTags.includes(tag.id) ? ' tag-selected' : ''}`} onClick={() => handleTagToggle(tag.id)}>
                                            {tag.label}
                                        </button>
                                    ))}
                                </div>
                            </div>

                            <div className="admin-card">
                                <div className="admin-card-header"><span className="admin-card-title">Тип на шифъра</span></div>
                                <select className="field-select" value={cipherType} onChange={e => setCipherType(e.target.value)}>
                                    <option value="">Неизвестен — нека ML реши</option>
                                    {GROUPS.map(group => (
                                        <optgroup key={group} label={group}>
                                            {CIPHER_TYPES.filter(t => t.group === group).map(t => (
                                                <option key={t.value} value={t.value}>{t.label}</option>
                                            ))}
                                        </optgroup>
                                    ))}
                                </select>
                            </div>

                            <div className="admin-card">
                                <div className="admin-card-header"><span className="admin-card-title">Решение на администратора</span></div>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    <button onClick={handleApprove} className="btn btn-success" style={{ justifyContent: 'center' }}>
                                        Одобри
                                    </button>
                                    <button onClick={() => setShowRejectForm(prev => !prev)} className="btn btn-danger" style={{ justifyContent: 'center' }}>
                                        Отхвърли
                                    </button>

                                    {showRejectForm && (
                                        <div className="reject-form" style={{ marginTop: '10px' }}>
                                            <label className="form-label">Причина за отказ <span style={{ color: 'var(--rose-500)' }}>*</span></label>
                                            <textarea className="form-textarea" rows="3" value={rejectReason} onChange={(e) => setRejectReason(e.target.value)} placeholder="Обяснете защо отхвърляте..." />
                                            <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                                <button onClick={handleReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>Потвърди</button>
                                                <button onClick={() => setShowRejectForm(false)} className="btn btn-ghost btn-sm">Отказ</button>
                                            </div>
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default CipherReview;