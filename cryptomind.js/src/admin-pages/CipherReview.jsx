import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import LlmAssistantSection from './LlmAssistantSection';
import '../styles/cipher-review.css';
const API_BASE = 'http://localhost:5115/api/admin';
import { useParams } from "react-router-dom";
import "../styles/cipher-review.css";
import { useNavigate } from 'react-router-dom';
import { useError } from '../ErrorContext';
axios.defaults.withCredentials = true;

const AVAILABLE_TAGS = [
    { id: 1, label: 'Изображение' },
    { id: 2, label: 'Пъзел' },
    { id: 3, label: 'Исторически' },
    { id: 4, label: 'Кратък' },
    { id: 5, label: 'Дълъг' },
    { id: 6, label: 'Подходящ за начинаещи' },
    { id: 7, label: 'Труден' }
];

const CIPHER_TYPES = [
    { value: '0',  label: 'Цезар (Caesar)',                      group: 'Substitution' },
    { value: '1',  label: 'Атбаш (Atbash)',                      group: 'Substitution' },
    { value: '2',  label: 'Проста замяна (SimpleSubstitution)',        group: 'Substitution' },
    { value: '3',  label: 'ROT13 (ROT13)',                      group: 'Substitution' },
    { value: '4',  label: 'Виженер (Vigenere)',                 group: 'Polyalphabetic' },
    { value: '5',  label: 'Автоключ (Autokey)',                 group: 'Polyalphabetic' },
    { value: '6',  label: 'Тритемий (Trithemius)',             group: 'Polyalphabetic' },
    { value: '7',  label: 'Железопътна ограда (RailFence)',    group: 'Transposition' },
    { value: '8',  label: 'Колонна (Columnar)',                group: 'Transposition' },
    { value: '9',  label: 'Маршрут (Route)',                   group: 'Transposition' },
    { value: '10', label: 'Base64 (Base64)',                   group: 'Encoding' },
    { value: '11', label: 'Морзов (Morse)',                   group: 'Encoding' },
    { value: '12', label: 'Двоичен (Binary)',                 group: 'Encoding' },
    { value: '13', label: 'Шестнадесетичен (Hex)',            group: 'Encoding' },
];

const GROUPS = ['Substitution', 'Polyalphabetic', 'Transposition', 'Encoding'];

const GROUP_LABELS = {
    'Substitution':   'Заместване',
    'Polyalphabetic': 'Полиазбучни',
    'Transposition':  'Транспозиция',
    'Encoding':       'Кодиране',
};

const CipherReview = () => {
    const [cipher, setCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    const [localError, setLocalError] = useState(null);
    const navigate = useNavigate();
    const { setError } = useError();
    const [llmResult, setLlmResult] = useState(null);
    const [isLlmLoading, setIsLlmLoading] = useState(false);
    
    const [title, setTitle] = useState('');
    const [selectedTags, setSelectedTags] = useState([]);
    const [allowHint, setAllowHint] = useState(true);
    const [allowSolutionHint, setAllowSolutionHint] = useState(false);
    const [allowSolution, setAllowSolution] = useState(false);
    const [cipherType, setCipherType] = useState(0);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectReason, setRejectReason] = useState('');
    const cipherId = useParams().id;

    useEffect(() => {
        const fetchCipher = async () => {
            if (!cipherId || isNaN(cipherId)) {
                setLocalError('Невалидно ID на шифър');
                setLoading(false);
                return;
            }          
            axios.get(`${API_BASE}/cipher/${cipherId}`)
            .then(res => {
                setCipher(res.data);
                setTitle(res.data.title || '');
                setAllowHint(res.data.allowTypeHint ?? false);
                setAllowSolutionHint(res.data.allowsSolutionHint ?? false);
                setAllowSolution(res.data.allowSolution ?? false);
                setCipherType(res.data.setCipherType)
                console.log('Fetched cipher:', res.data);
            }).catch(err => {
                const status = err.response?.status;
                const serverMessage = err?.data?.title;
                switch (status) {
                    case 404:
                        setLocalError('Шифърът не е намерен');
                      //  navigate('/not-found');
                        break;
                    default:
                        setLocalError(serverMessage || 'Неуспешно зареждане на шифъра');
                        setError(serverMessage || 'Неуспешно зареждане на шифъра');
                        break;    
                };
                console.error('Failed to fetch cipher:', err);
                setLocalError(err.response?.data?.error || err.message);
            })
            .finally(() => setLoading(false));
        };

        fetchCipher();
    }, [cipherId]);

    const handleRunLlm = useCallback(async () => {
        setIsLlmLoading(true);
        try {
            const { data } = await axios.get(`${API_BASE}/cipher/${cipherId}/analyze`);
            console.log('LLM result:', data);
            setLlmResult(data);
        } catch (err) {
            console.error('LLM analysis failed:', err);
            setError(err.response.data.title || 'Неуспешен LLM анализ');
        } finally {
            setIsLlmLoading(false);
        }
    }, [cipherId]);

    const handleTagToggle = useCallback((tagId) => {
        setSelectedTags(prev => 
            prev.includes(tagId) 
                ? prev.filter(id => id !== tagId)
                : [...prev, tagId]
        );
    }, []);

    const handleApprove = async () => {
        const isExperimental = !cipher.decryptedText;
        const typeLabel = isExperimental ? 'Експериментален' : 'Стандартен';

        try {
            await axios.put(`${API_BASE}/cipher/${cipherId}/approve`, {
                title: title,
                allowTypeHint: allowHint,
                allowHint: allowSolutionHint,
                allowSolution: allowSolution,
                typeOfCipher: cipherType,
                tagIds: selectedTags
            });
            console.log(`Шифърът е одобрен като ${typeLabel}!`);
            navigate('/admin/pending-ciphers'); 
        } catch (err) {
            const errorMsg = err.response?.data.error || err.message;
            setError(`Неуспешно одобрение: ${errorMsg}`);
        }
    };

    const handleReject = useCallback(async () => {
        if (!rejectReason.trim()) {
            setError('Моля, посочете причина за отхвърлянето на този шифър.');
            return;
        }

        try {
            await axios.put(`${API_BASE}/cipher/${cipherId}/reject`, rejectReason, {
                headers: { 'Content-Type': 'application/json' }
            });
            navigate('/admin/pending-ciphers');
        } catch (err) {
            setError(`Неуспешно отхвърляне: ${err.response?.data?.message || err.message}`);
        }
    }, [cipherId, rejectReason]);

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
                                ← Назад към изчакващите предложения
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
                        #{cipher.id} — предложен от {cipher.submittedBy}
                    </span>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">{cipher.title || `Шифър #${cipher.id}`}</h1>
                        <p className="page-subtitle">Прегледайте предложението, проверете ML прогнозата и вземете решение за одобрение</p>
                    </div>

                    <div className="review-layout">
                        {/* ─── Left: Main Review Panels ─── */}
                        <div className="review-main">
                            
                            {/* 1. Cipher Content */}
                            <div className="admin-card review-card">
                                <div className="admin-card-header">
                                    <div className="review-card-title-row">
                                        <span className="review-section-tag">ВХОД</span>
                                        <span className="admin-card-title">Предложено съдържание</span>
                                    </div>
                                    <div style={{ display: 'flex', gap: '6px' }}>
                                        {cipher.isImage && (
                                            <span className="badge badge-standard">Има изображение</span>
                                        )}
                                    </div>
                                </div>

                                <div className="form-label">Текст на шифъра</div>
                                <div className="cipher-text-display prominent">{cipher.cipherText}</div>

                                {cipher.isImage && cipher.imageBase64 && (
                                    <div className="cipher-image-preview">
                                        <div className="form-label">Качено изображение</div>
                                        <img src={`${cipher.imageBase64}`} alt="Шифър" />
                                        <div className="ocr-note">
                                            ↑ Текстът по-горе е извлечен чрез OCR от това изображение
                                        </div>
                                    </div>
                                )}

                                {cipher.decryptedText && (
                                    <div style={{ marginTop: '14px' }}>
                                        <div className="form-label">Предложено решение на открит текст</div>
                                        <div className="solution-display">{cipher.decryptedText}</div>
                                    </div>
                                )}

                                <div className="cipher-meta-row">
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Предложен от</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedBy}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Предложен на</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedAt || '—'}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Дължина на текста</span>
                                        <span className="cipher-meta-value text-mono">
                                            {cipher.cipherText?.length ?? 0} знака
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

                            {/* 2. ML Analysis */}
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
                                        <span className="ml-metric-label">Предвиден вид</span>
                                        <span className="ml-metric-value">{cipher.cipherType || '—'}</span>
                                    </div>

                                    <div className="ml-metric">
                                        <span className="ml-metric-label">LLM препоръчан</span>
                                        <span className={`badge ${cipher.isLLMRecommended ? 'badge-pending' : 'badge-approved'}`}>
                                            {cipher.isLLMRecommended ? 'Да' : 'Не'}
                                        </span>
                                    </div>

                                    <div className="ml-metric ml-metric-wide">
                                        <span className="ml-metric-label">Увереност</span>
                                        <div className="ml-confidence-row">
                                            <div className="ml-confidence-bar">
                                                <div 
                                                    className={`ml-confidence-fill ${
                                                        cipher.percentageOfConfidence >= 85 ? 'confidence-high' : 
                                                        cipher.percentageOfConfidence >= 65 ? 'confidence-mid' : 
                                                        'confidence-low'
                                                    }`}
                                                    style={{ width: `${cipher.percentageOfConfidence}%` }}
                                                />
                                            </div>
                                            <span className="ml-confidence-pct">{cipher.percentageOfConfidence}%</span>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* 3. LLM Assistant */}
                            <LlmAssistantSection
                                result={llmResult}
                                isLoading={isLlmLoading}
                                onRunAnalysis={handleRunLlm}
                            />
                        </div>

                        {/* ─── Right: Admin Actions ─── */}
                        <div className="actions-column">

                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Детайли на шифъра</span>
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Заглавие</label>
                                    <input
                                        type="text"
                                        className="form-input"
                                        value={title}
                                        onChange={(e) => setTitle(e.target.value)}
                                    />
                                </div>
                            </div>

                            {cipher.decryptedText && ( 
                                <div className="admin-card">
                                    <div className="admin-card-header">
                                        <span className="admin-card-title">Разрешения за AI помощ</span>
                                    </div>

                                    <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                                        <label className="permission-toggle">
                                            <input
                                                type="checkbox"
                                                checked={allowHint}
                                                onChange={(e) => setAllowHint(e.target.checked)}
                                            />
                                            <span>Позволи подсказки за вида</span>
                                        </label>
                                        <label className="permission-toggle">
                                            <input
                                                type="checkbox"
                                                checked={allowSolutionHint}
                                                onChange={(e) => setAllowSolutionHint(e.target.checked)}
                                            />
                                            <span>Позволи подсказки за решението</span>
                                        </label>
                                        <label className="permission-toggle">
                                            <input
                                                type="checkbox"
                                                checked={allowSolution}
                                                onChange={(e) => setAllowSolution(e.target.checked)}
                                            />
                                            <span>Позволи пълното решение</span>
                                        </label>
                                    </div>
                                </div>
                            )}

                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Етикети</span>
                                </div>

                                <div className="tag-cloud">
                                    {AVAILABLE_TAGS.map((tag) => (
                                        <button
                                            key={tag.id}
                                            className={`tag-chip${selectedTags.includes(tag.id) ? ' tag-selected' : ''}`}
                                            onClick={() => handleTagToggle(tag.id)}
                                        >
                                            {tag.label}
                                        </button>
                                    ))}
                                </div>
                            </div>

                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Вид на шифъра</span>
                                </div>

                                <div>
                                    <select
                                        className="field-select"
                                        value={cipherType}
                                        onChange={e => setCipherType(e.target.value)}
                                    >
                                        <option value="">Неизвестен — нека ML реши</option>
                                        {GROUPS.map(group => (
                                            <optgroup key={group} label={GROUP_LABELS[group]}>
                                                {CIPHER_TYPES
                                                    .filter(t => t.group === group)
                                                    .map(t => (
                                                        <option key={t.value} value={t.value}>{t.label}</option>
                                                    ))
                                                }
                                            </optgroup>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Вид предизвикателство</span>
                                </div>

                                <div className="type-toggle">
                                    {cipher.decryptedText ? (
                                        <button className="type-toggle-btn active-standard">
                                            <span className="type-toggle-dot dot-sky" />
                                            Стандартен
                                            <span className="type-toggle-note">ИМА РЕШЕНИЕ</span>
                                        </button>
                                    ) : (
                                        <button className="type-toggle-btn">
                                            <span className="type-toggle-dot dot-violet" />
                                            Експериментален
                                            <span className="type-toggle-note">БЕЗ РЕШЕНИЕ</span>
                                        </button>
                                    )}
                                </div>
                            </div>

                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Решение за преглед</span>
                                </div>

                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    <button
                                        onClick={handleApprove}
                                        className="btn btn-success"
                                        style={{ justifyContent: 'center' }}>
                                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                            <path d="M2 8l4 4 8-8"/>
                                        </svg>
                                        Одобри
                                    </button>                               

                                    <div className="reject-section">
                                        <button
                                            onClick={() => setShowRejectForm(prev => !prev)}
                                            className="btn btn-danger"
                                            style={{ justifyContent: 'center', width: '100%' }}
                                        >
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                                <path d="M3 3l10 10M13 3L3 13"/>
                                            </svg>
                                            Отхвърли предложението
                                        </button>

                                        {showRejectForm && (
                                            <div className="reject-form" style={{ marginTop: '10px' }}>
                                                <label className="form-label">
                                                    Причина за отхвърляне <span style={{ color: 'var(--rose-500)' }}>*</span>
                                                </label>
                                                <textarea
                                                    className="form-textarea"
                                                    placeholder="Обяснете защо това се отхвърля..."
                                                    rows="3"
                                                    value={rejectReason}
                                                    onChange={(e) => setRejectReason(e.target.value)}
                                                />
                                                <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                                    <button onClick={handleReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>
                                                        Потвърди отхвърлянето
                                                    </button>
                                                    <button onClick={() => {
                                                        setShowRejectForm(false);
                                                        setRejectReason('');
                                                    }} className="btn btn-ghost btn-sm">
                                                        Отказ
                                                    </button>
                                                </div>
                                            </div>
                                        )}
                                    </div>
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