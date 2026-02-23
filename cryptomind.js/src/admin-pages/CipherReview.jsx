import React, { useState, useEffect, useCallback, } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import MlAnalysisSection from './MlAnalysisSection';
import LlmAssistantSection from './LlmAssistantSection';
import AdminActions from './AdminActions';
import '../styles/cipher-review.css';
const API_BASE = 'http://localhost:5115/api/admin';
import { useParams } from "react-router-dom";
// Configure axios globally
import "../styles/cipher-review.css";
import { useNavigate } from 'react-router-dom';
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
const GROUPS = ['Substitution', 'Polyalphabetic', 'Transposition', 'Encoding'];


const CipherReview = () => {
    const [cipher, setCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();
    // LLM state
    const [llmResult, setLlmResult] = useState(null);
    const [isLlmLoading, setIsLlmLoading] = useState(false);
    
    // Form state
    const [title, setTitle] = useState('');
    const [selectedTags, setSelectedTags] = useState([]);
    const [allowHint, setAllowHint] = useState(true);
    const [allowSolution, setAllowSolution] = useState(false);
    const [cipherType, setCipherType] = useState(0);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectReason, setRejectReason] = useState('');

    const cipherId = useParams().id;

    // Fetch cipher
    useEffect(() => {
        const fetchCipher = async () => {
            if (!cipherId || isNaN(cipherId)) {
                setError('Invalid cipher ID');
                setLoading(false);
                return;
            }          
                axios.get(`${API_BASE}/cipher/${cipherId}`)
                .then(res => {
                    setCipher(res.data);
                    setTitle(res.data.title || '');
                    setAllowHint(res.data.allowHint ?? true);
                    setAllowSolution(res.data.allowFullSolution ?? false);
                }).catch(err => {
                    const status = err.response?.status;
                    const serverMessage = err.response?.data?.message;
                    switch (status) {
                        case 404:
                            setError('Cipher not found');
                             window.location.href = '/not-found';
                            break;
                        default:
                            setError(serverMessage || 'Failed to fetch cipher');
                            break;    
                    };
                console.error('Failed to fetch cipher:', err);
                setError(err.response?.data?.message || err.message)})
                .finally(() => setLoading(false));
              
        
            
        };

        fetchCipher();
    }, [cipherId]);

    // Run LLM analysis
    const handleRunLlm = useCallback(async () => {
        setIsLlmLoading(true);
        try {
            const { data } = await axios.get(`${API_BASE}/cipher/${cipherId}/analyze`);
            console.log('LLM result:', data);
            setLlmResult(data);
        } catch (err) {
            console.error('LLM analysis failed:', err);
            alert(`LLM analysis failed: ${err.response?.data?.message || err.message}`);
        } finally {
            setIsLlmLoading(false);
        }
    }, [cipherId]);

    // Tag toggle
    const handleTagToggle = useCallback((tagId) => {
        setSelectedTags(prev => 
            prev.includes(tagId) 
                ? prev.filter(id => id !== tagId)
                : [...prev, tagId]
        );
    }, []);

    // Approve
    const handleApprove = async () => {
    // 1. Determine type logic
    const isExperimental = !cipher.decryptedText;
    const typeLabel = isExperimental ? 'Experimental' : 'Standard';

    try {
        await axios.put(`${API_BASE}/cipher/${cipherId}/approve`, {
            title: title,
            allowTypeHint: cipher.allowType,
            allowHint: allowHint,
            allowSolution: allowSolution,
            typeOfCipher: isExperimental ? 1 : 0,
            tagIds: selectedTags
        });

        // 2. Use a better UX than alert if possible, but at least fix navigation
        console.log(`Cipher approved as ${typeLabel}!`);
        
        // 3. SPA Navigation (No full page reload)
        navigate('/admin/pending-ciphers'); 

    } catch (err) {
        const errorMsg = err.response?.data?.message || err.message;
        alert(`Approval failed: ${errorMsg}`);
    }
}

    // Reject
    const handleReject = useCallback(async () => {
        if (!rejectReason.trim()) {
            alert('Please provide a reason for rejecting this cipher.');
            return;
        }

        try {
            await axios.put(`${API_BASE}/cipher/${cipherId}/reject`, rejectReason, {
                headers: { 'Content-Type': 'application/json' }
            });
            alert('Cipher rejected');
            window.location.href = '/admin/pending-ciphers';
        } catch (err) {
            alert(`Rejection failed: ${err.response?.data?.message || err.message}`);
        }
    }, [cipherId, rejectReason]);

    if (loading) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-ciphers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">Loading...</div>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    if (error || !cipher) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-ciphers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">{error || 'Cipher not found'}</div>
                            <a href="/admin/pending-ciphers" className="btn btn-ghost btn-sm" style={{ marginTop: '12px' }}>
                                ← Back to Pending Ciphers
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
                        { label: 'Pending Submissions', href: '/admin/pending-ciphers' },
                        { label: cipher.title || `Cipher #${cipher.id}` }
                    ]}
                >
                    <span style={{ fontFamily: 'var(--font-mono)', fontSize: '10px', color: 'var(--text-dim)' }}>
                        #{cipher.id} — submitted by {cipher.submittedBy}
                    </span>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">{cipher.title || `Cipher #${cipher.id}`}</h1>
                        <p className="page-subtitle">Review submission, verify ML prediction, and make an approval decision</p>
                    </div>

                    <div className="review-layout">
                        {/* ─── Left: Main Review Panels ─── */}
                        <div className="review-main">
                            
                            {/* 1. Cipher Content */}
                            <div className="admin-card review-card">
                                <div className="admin-card-header">
                                    <div className="review-card-title-row">
                                        <span className="review-section-tag">INPUT</span>
                                        <span className="admin-card-title">Submitted Content</span>
                                    </div>
                                    <div style={{ display: 'flex', gap: '6px' }}>
                                        {cipher.isImage && (
                                            <span className="badge badge-standard">Has Image</span>
                                        )}
                                    </div>
                                </div>

                                {/* Cipher Text */}
                                <div className="form-label">Cipher Text</div>
                                <div className="cipher-text-display prominent">{cipher.cipherText}</div>

                                {/* Image (if any) */}
                                {cipher.isImage && cipher.imageBase64 && (
                                    <div className="cipher-image-preview">
                                        <div className="form-label">Uploaded Image</div>
                                        <img src={`${cipher.imageBase64}`} alt="Cipher" />
                                        <div className="ocr-note">
                                            ↑ Text above was extracted via OCR from this image
                                        </div>
                                    </div>
                                )}

                                {/* Submitted Solution */}
                                {cipher.decryptedText && (
                                    <div style={{ marginTop: '14px' }}>
                                        <div className="form-label">Submitted Plaintext Solution</div>
                                        <div className="solution-display">{cipher.decryptedText}</div>
                                    </div>
                                )}

                                {/* Metadata Row */}
                                <div className="cipher-meta-row">
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Submitted By</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedBy}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Submitted At</span>
                                        <span className="cipher-meta-value text-mono">{cipher.submittedAt || '—'}</span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Text Length</span>
                                        <span className="cipher-meta-value text-mono">
                                            {cipher.cipherText?.length ?? 0} chars
                                            {cipher.cipherText?.length < 150 && (
                                                <span style={{ color: 'var(--rose-500)', marginLeft: '6px' }}>⚠ Below 150</span>
                                            )}
                                        </span>
                                    </div>
                                    <div className="cipher-meta-item">
                                        <span className="cipher-meta-label">Has Solution</span>
                                        <span className={`cipher-meta-value ${cipher.decryptedText ? 'text-emerald' : 'text-dim'}`}>
                                            {cipher.decryptedText ? 'Yes' : 'No'}
                                        </span>
                                    </div>
                                </div>
                            </div>

                            {/* 2. ML Analysis */}
                            <div className="admin-card review-card">
                                <div className="admin-card-header">
                                    <div className="review-card-title-row">
                                        <span className="review-section-tag">ML</span>
                                        <span className="admin-card-title">ML Analysis</span>
                                    </div>
                                    <span className={`badge ${cipher.percentageOfConfidence >= 85 ? 'badge-approved' : cipher.percentageOfConfidence >= 65 ? 'badge-pending' : 'badge-rejected'}`}>
                                        {cipher.percentageOfConfidence}% confidence
                                    </span>
                                </div>

                                <div className="ml-result-grid">
                                    <div className="ml-metric">
                                        <span className="ml-metric-label">Predicted Type</span>
                                        <span className="ml-metric-value">{cipher.mlPrediction || '—'}</span>
                                    </div>

                                    <div className="ml-metric">
                                        <span className="ml-metric-label">LLM Recommended</span>
                                        <span className={`badge ${cipher.isLLMRecommended ? 'badge-pending' : 'badge-approved'}`}>
                                            {cipher.isLLMRecommended ? 'Yes' : 'No'}
                                        </span>
                                    </div>

                                    <div className="ml-metric ml-metric-wide">
                                        <span className="ml-metric-label">Confidence</span>
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
                                    <span className="admin-card-title">Cipher Details</span>
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Title</label>
                                    <input
                                        type="text"
                                        className="form-input"
                                        value={title}
                                        onChange={(e) => setTitle(e.target.value)}
                                    />
                                </div>
                            </div>

                            {/* Permissions */}
                            
                            {cipher.decryptedText && ( 
                                <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">AI Assistance Permissions</span>
                                </div>

                                <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                                    <label className="permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={allowHint}
                                            onChange={(e) => setAllowHint(e.target.checked)}
                                        />
                                        <span>Allow Type Hints</span>
                                    </label>
                                    <label className="permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={allowSolution}
                                            onChange={(e) => setAllowSolution(e.target.checked)}
                                        />
                                        <span>Allow Solution Hints</span>
                                    </label>
                                    <label className="permission-toggle">
                                        <input
                                            type="checkbox"
                                            checked={allowSolution}
                                            onChange={(e) => setAllowSolution(e.target.checked)}
                                        />
                                        <span>Allow Full Solution</span>
                                    </label>
                                </div>
                            </div>)}
                           

                            {/* Tags */}
                           {  <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Tags</span>
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
                            </div>}

                            {/* Challenge Type */}
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Challenge Type</span>
                                </div>

                                <div>
                               <select
                    className="field-select"
                    value={cipherType}
                    onChange={e => setCipherType( e.target.value)}
                >
                    <option value="">Unknown — let the ML decide</option>
                    {GROUPS.map(group => (
                        <optgroup key={group} label={group}>
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
                                    <span className="admin-card-title">Challenge Type</span>
                                </div>

                                <div className="type-toggle">
                                {cipher.decryptedText ? (
                                    <button className="type-toggle-btn active-standard">
                                        <span className="type-toggle-dot dot-sky" />
                                        Standard
                                        <span className="type-toggle-note">HAS SOLUTION</span>
                                    </button>
                                ) : (
                                    <button className="type-toggle-btn">
                                        <span className="type-toggle-dot dot-violet" />
                                        Experimental
                                        <span className="type-toggle-note">NO SOLUTION YET</span>
                                    </button> )}
                                </div>
                            </div>
                            {/* Actions */}
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Review Decision</span>
                                </div>

                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    <button
                                        onClick={handleApprove}
                                        className="btn btn-success"
                                        style={{ justifyContent: 'center' }}>
                                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                            <path d="M2 8l4 4 8-8"/>
                                        </svg>
                                        Approve
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
                                            Reject Submission
                                        </button>

                                        {showRejectForm && (
                                            <div className="reject-form" style={{ marginTop: '10px' }}>
                                                <label className="form-label">
                                                    Rejection Reason <span style={{ color: 'var(--rose-500)' }}>*</span>
                                                </label>
                                                <textarea
                                                    className="form-textarea"
                                                    placeholder="Explain why this is being rejected..."
                                                    rows="3"
                                                    value={rejectReason}
                                                    onChange={(e) => setRejectReason(e.target.value)}
                                                />
                                                <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                                    <button onClick={handleReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>
                                                        Confirm Reject
                                                    </button>
                                                    <button onClick={() => {
                                                        setShowRejectForm(false);
                                                        setRejectReason('');
                                                    }} className="btn btn-ghost btn-sm">
                                                        Cancel
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