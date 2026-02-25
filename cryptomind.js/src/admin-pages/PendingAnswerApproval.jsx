import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/answer-approval.css';
const API_BASE = 'http://localhost:5115/api/admin';
import { useParams,useNavigate } from "react-router-dom";
import { useError } from '../ErrorContext.jsx';

axios.defaults.withCredentials = true;

const PendingAnswerApproval = () => {
    const [answerSubmission, setAnswerSubmission] = useState(null);
    
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectionReason, setRejectionReason] = useState('');
    const {  id } = useParams();
    const navigate = useNavigate();
     const { setError: setGlobalError } = useError();
    // Fetch answer submission
    useEffect(() => {
        const fetchAnswer = async () => {
            if (!id || isNaN(id)) {
                setError('Невалидно ID на отговора');
                setLoading(false);
                return;
            }

            try {
                // Fetch the answer suggestion
                const { data } = await axios.get(`${API_BASE}/answer/${id}`);
                console.log('Answer suggestion:', data);
                setAnswerSubmission(data);
            } catch (err) {
                console.error('Failed to fetch answer:', err);
                setGlobalError(err.response?.data?.message || err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchAnswer();
    }, [id]);

    // Approve answer
    const handleApprove = useCallback(async () => {
      
        try {
            await axios.put(`${API_BASE}/answer/${id}/approve`);
           
            navigate('/admin/pending-answers');
        } catch (err) {
            console.error('Approve error:', err);
            setGlobalError(err.response?.data?.message || err.message);
        }
    }, [id]);

    // Reject answer
    const handleReject = useCallback(async () => {
        console.log('Rejection reason:', rejectionReason);
       
        try {
            await axios.put(`${API_BASE}/answer/${id}/reject`,  JSON.stringify(rejectionReason),{
                headers: {
                "Content-Type": "application/json"
                }
            });

            navigate('/admin/pending-answers');
        } catch (err) {
            console.error('Reject error:', err);
            setGlobalError(err.response?.data?.message || err.message);
        }
    }, [id]);

    if (loading) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-answers" />
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

    if (error || !answerSubmission) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-answers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">{error || 'Предложеният отговор не е намерен'}</div>
                            <a href="/admin/pending-answers" className="btn btn-ghost btn-sm" style={{ marginTop: '12px' }}>
                                ← Обратно към чакащи отговори
                            </a>
                        </div>
                    </div>
                </main>
            </div>
        );
    }

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="pending-answers" />

            <main className="admin-main">
                <AdminTopbar
                    breadcrumbs={[
                        { label: 'Чакащи отговори', href: '/admin/pending-answers' },
                        { label: `Предложение #${answerSubmission.id}` }
                    ]}
                >
                    <span style={{ fontFamily: 'var(--font-mono)', fontSize: '10px', color: 'var(--text-dim)' }}>
                        предложено от {answerSubmission.username}
                    </span>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Преглед на предложен отговор</h1>
                        <p className="page-subtitle">
                            Предложение за: <strong style={{ color: 'var(--text-secondary)' }}>
                                {answerSubmission.cipherName  || `Шифър #${answerSubmission.cipherId}`}
                            </strong>
                        </p>
                    </div>

                    <div className="answer-approval-layout">
                        <div className="answer-approval-main">
                            {/* Original Cipher Context */}
                            <div className="admin-card">
                                {/* <div className="admin-card-header">
                                    <span className="admin-card-title">Оригинален шифър</span>
                                    <a
                                        href={`/cipher/${answerSubmission.cipherId}`}
                                        className="btn btn-ghost btn-sm"
                                        target="_blank"
                                        rel="noreferrer"
                                    >
                                        Виж шифъра →
                                    </a>
                                </div> */}

                                {(answerSubmission.cipherName) && (
                                    <div style={{ fontWeight: 600, fontSize: '14px', color: 'var(--text-primary)', marginBottom: '8px' }}>
                                        {answerSubmission.cipherName}
                                    </div>
                                )}

                                
                                    <>
                                        <div className="form-label">Шифриран текст</div>
                                        <div className="original-cipher-block">
                                            {answerSubmission.cipherEncryptedText}
                                        </div>

                                        <div style={{ marginTop: '12px', display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
                                            <div>
                                                <div className="form-label">Вид</div>
                                                {/* <span className="mono" style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>
                                                    {originalCipher.family ?? '—'}
                                                </span> */}
                                            </div>
                                            <div>
                                                <div className="form-label">Статус</div>
                                                <span className="badge badge-experimental">Експериментален</span>
                                            </div>
                                        </div>
                                    </>
                                
                            </div>

                            {/* The Suggested Answer */}
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Предложен отговор</span>
                                    <div style={{ display: 'flex', gap: '6px', alignItems: 'center' }}>
                                        <span className="mono" style={{ fontSize: '11px', color: 'var(--text-dim)' }}>
                                            от {answerSubmission.username}
                                        </span>
                                    </div>
                                </div>

                                <div className="form-label">Предложен дешифриран текст</div>
                                <div className="submitted-answer-box">
                                    {answerSubmission.decryptedText || '—'}
                                </div>

                                {answerSubmission.description && (
                                    <div style={{ marginTop: '14px' }}>
                                        <div className="form-label">Обяснение на потребителя</div>
                                        <div className="explanation-box">
                                            {answerSubmission.description}
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>

                        {/* Decision sidebar */}
                        <div className="answer-approval-sidebar">
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Решение</span>
                                </div>

                                <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                                    <button
                                        onClick={handleApprove}
                                        className="btn btn-success"
                                        style={{ justifyContent: 'center' }}
                                    >
                                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                            <path d="M2 8l4 4 8-8"/>
                                        </svg>
                                        Одобри отговора
                                    </button>
                                    <p style={{ fontSize: '11px', color: 'var(--text-dim)', lineHeight: '1.5' }}>
                                        Одобряването ще даде точки на {answerSubmission.username} и ще маркира шифъра като решен.
                                    </p>

                                    <div className="divider" />

                                    <div>
                                        <button
                                            onClick={() => setShowRejectForm(prev => !prev)}
                                            className="btn btn-danger"
                                            style={{ justifyContent: 'center', width: '100%' }}
                                        >
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.8">
                                                <path d="M3 3l10 10M13 3L3 13"/>
                                            </svg>
                                            Отхвърли отговора
                                        </button>

                                        {showRejectForm && (
                                            <div style={{ marginTop: '10px' }}>
                                                <label className="form-label">
                                                    Причина за отхвърляне <span style={{ color: 'var(--rose-500)' }}>*</span>
                                                </label>
                                               <textarea
                                                    id="answer-reject-reason"
                                                    className="form-textarea"
                                                    placeholder="Обяснете защо отговорът е неверен..."
                                                    value={rejectionReason}
                                                    onChange={(e) => setRejectionReason(e.target.value)}
                                                    rows="3"
                                                    required
                                                />
                                                <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                                    <button onClick={handleReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>
                                                        Потвърди отхвърлянето
                                                    </button>
                                                    <button 
                                                        onClick={() => {
                                                            setShowRejectForm(false);
                                                            const textarea = document.getElementById('answer-reject-reason');
                                                            if (textarea) textarea.value = '';
                                                        }}
                                                        className="btn btn-ghost btn-sm"
                                                    >
                                                        Отказ
                                                    </button>
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>

                            <a href="/admin/pending-answers" className="btn btn-ghost" style={{ justifyContent: 'center' }}>
                                ← Обратно към опашката
                            </a>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default PendingAnswerApproval;