import React, { useState, useEffect, useCallback, use } from 'react';
import axios from 'axios';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import '../styles/answer-approval.css';
const API_BASE = 'http://localhost:5115/api/admin';
import { useParams } from "react-router-dom";
axios.defaults.withCredentials = true;

const PendingAnswerApproval = () => {
    const [answerSubmission, setAnswerSubmission] = useState(null);
    const [originalCipher, setOriginalCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showRejectForm, setShowRejectForm] = useState(false);
    const [rejectionReason, setRejectionReason] = useState('');
      const {  id } = useParams();

    // Fetch answer submission
    useEffect(() => {
        const fetchAnswer = async () => {
            if (!id || isNaN(id)) {
                setError('Invalid answer ID');
                setLoading(false);
                return;
            }

            try {
                // Fetch the answer suggestion
                const { data } = await axios.get(`${API_BASE}/answer/${id}`);
                console.log('Answer suggestion:', data);
                setAnswerSubmission(data);

                // Fetch the original cipher details
                if (data.cipherId) {
                    const cipherData = await axios.get(`${API_BASE}/cipher/${data.cipherId}`);
                    console.log('Original cipher:', cipherData.data);
                    setOriginalCipher(cipherData.data);
                }
            } catch (err) {
                console.error('Failed to fetch answer:', err);
                setError(err.response?.data?.message || err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchAnswer();
    }, [id]);

    // Approve answer
    const handleApprove = useCallback(async () => {
        if (!window.confirm('Approve this answer? The user will be awarded points.')) return;

        try {
            await axios.put(`${API_BASE}/answer/${id}/approve`);
            alert('Answer approved successfully!');
            window.location.href = '/admin/pending-answers';
        } catch (err) {
            console.error('Approve error:', err);
            alert(`Failed to approve: ${err.response?.data?.message || err.message}`);
        }
    }, [id]);

    // Reject answer
    const handleReject = useCallback(async () => {
        //  if (!rejectionReason.trim()) {
        // alert('Please provide a reason...');
        // return;
        //  }
        console.log('Rejection reason:', rejectionReason);
       
        try {
            await axios.put(`${API_BASE}/answer/${id}/reject`,  JSON.stringify(rejectionReason),{
                headers: {
                "Content-Type": "application/json"
                }
            });
            alert('Answer rejected');
            window.location.href = '/admin/pending-answers';
        } catch (err) {
            console.error('Reject error:', err);
            alert(`Failed to reject: ${err.response?.data?.message || err.message}`);
        }
    }, [id]);

    if (loading) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-answers" />
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

    if (error || !answerSubmission) {
        return (
            <div className="admin-shell">
                <AdminSidebar activePage="pending-answers" />
                <main className="admin-main">
                    <div className="admin-content">
                        <div className="empty-state">
                            <div className="empty-state-title">{error || 'Answer suggestion not found'}</div>
                            <a href="/admin/pending-answers" className="btn btn-ghost btn-sm" style={{ marginTop: '12px' }}>
                                ← Back to Pending Answers
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
                        { label: 'Pending Answers', href: '/admin/pending-answers' },
                        { label: `Suggestion #${answerSubmission.id}` }
                    ]}
                >
                    <span style={{ fontFamily: 'var(--font-mono)', fontSize: '10px', color: 'var(--text-dim)' }}>
                        submitted by {answerSubmission.username}
                    </span>
                </AdminTopbar>

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Answer Suggestion Review</h1>
                        <p className="page-subtitle">
                            Suggestion for: <strong style={{ color: 'var(--text-secondary)' }}>
                                {answerSubmission.cipherName || originalCipher?.title || `Cipher #${answerSubmission.cipherId}`}
                            </strong>
                        </p>
                    </div>

                    <div className="answer-approval-layout">
                        <div className="answer-approval-main">
                            {/* Original Cipher Context */}
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Original Cipher</span>
                                    <a
                                        href={`/cipher/${answerSubmission.cipherId}`}
                                        className="btn btn-ghost btn-sm"
                                        target="_blank"
                                        rel="noreferrer"
                                    >
                                        View Cipher →
                                    </a>
                                </div>

                                {(answerSubmission.cipherName || originalCipher?.title) && (
                                    <div style={{ fontWeight: 600, fontSize: '14px', color: 'var(--text-primary)', marginBottom: '8px' }}>
                                        {answerSubmission.cipherName || originalCipher.title}
                                    </div>
                                )}

                                {originalCipher && (
                                    <>
                                        <div className="form-label">Cipher Text</div>
                                        <div className="original-cipher-block">
                                            {originalCipher.cipherText ?? '—'}
                                        </div>

                                        <div style={{ marginTop: '12px', display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
                                            <div>
                                                <div className="form-label">Type</div>
                                                <span className="mono" style={{ fontSize: '13px', color: 'var(--text-secondary)' }}>
                                                    {originalCipher.mlPrediction ?? '—'}
                                                </span>
                                            </div>
                                            <div>
                                                <div className="form-label">Status</div>
                                                <span className="badge badge-experimental">Experimental</span>
                                            </div>
                                        </div>
                                    </>
                                )}
                            </div>

                            {/* The Suggested Answer */}
                            <div className="admin-card">
                                <div className="admin-card-header">
                                    <span className="admin-card-title">Submitted Suggestion</span>
                                    <div style={{ display: 'flex', gap: '6px', alignItems: 'center' }}>
                                        <span className="mono" style={{ fontSize: '11px', color: 'var(--text-dim)' }}>
                                            by {answerSubmission.username}
                                        </span>
                                    </div>
                                </div>

                                <div className="form-label">Suggested Decrypted Text</div>
                                <div className="submitted-answer-box">
                                    {answerSubmission.decryptedText || '—'}
                                </div>

                                {answerSubmission.description && (
                                    <div style={{ marginTop: '14px' }}>
                                        <div className="form-label">User's Explanation</div>
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
                                    <span className="admin-card-title">Decision</span>
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
                                        Approve Answer
                                    </button>
                                    <p style={{ fontSize: '11px', color: 'var(--text-dim)', lineHeight: '1.5' }}>
                                        Approving will award points to {answerSubmission.username} and mark this cipher as solved.
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
                                            Reject Answer
                                        </button>

                                        {showRejectForm && (
                                            <div style={{ marginTop: '10px' }}>
                                                <label className="form-label">
                                                    Reason for Rejection <span style={{ color: 'var(--rose-500)' }}>*</span>
                                                </label>
                                               <textarea
                                                    id="answer-reject-reason"
                                                    className="form-textarea"
                                                    placeholder="Explain why this answer is incorrect..."
                                                    value={rejectionReason}
                                                    onChange={(e) => setRejectionReason(e.target.value)}
                                                    rows="3"
                                                    required
                                                />
                                                <div style={{ display: 'flex', gap: '8px', marginTop: '8px' }}>
                                                    <button onClick={handleReject} className="btn btn-danger btn-sm" style={{ flex: 1 }}>
                                                        Confirm Reject
                                                    </button>
                                                    <button 
                                                        onClick={() => {
                                                            setShowRejectForm(false);
                                                            const textarea = document.getElementById('answer-reject-reason');
                                                            if (textarea) textarea.value = '';
                                                        }}
                                                        className="btn btn-ghost btn-sm"
                                                    >
                                                        Cancel
                                                    </button>
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            </div>

                            <a href="/admin/pending-answers" className="btn btn-ghost" style={{ justifyContent: 'center' }}>
                                ← Back to Queue
                            </a>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default PendingAnswerApproval;