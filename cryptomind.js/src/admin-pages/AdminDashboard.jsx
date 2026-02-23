import React, { useState,useEffect } from 'react';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import StatCard from './StatCard';
import '../styles/admin-dashboard.css';
import axios from 'axios';
import { Link } from 'react-router-dom';

/* ───────────────────────────────────────────
   AdminDashboard.jsx
   Landing page for the admin area.
   Shows summary stats + quick navigation links + recent activity.
   All data should come from your C# API endpoints:
     GET /api/cipherAdmin/pendingCiphers  → count pending
     GET /api/cipherAdmin/approvedCiphers → count approved
     GET /api/users                       → count users
     GET /api/answers/pending             → count pending answers
─────────────────────────────────────────── */

const AdminDashboard = ({recentActivity = [] }) => {

    const [stats, setStats] = useState({  
        pendingCiphersCount: 0,
        approvedCiphersCount: 0,
        pendingAnswersCount: 0,
        approvedAnswersCount: 0,
        deletedCiphersCount: 0,
        pendingCipherTitles: []
    });
    useEffect(() => {
        axios.get('http://localhost:5115/api/admin/dashboard')
        .then(res => {
            setStats({
                pendingCiphersCount: res.data.pendingCiphersCount,
                approvedCiphersCount: res.data.approvedCiphersCount,
                pendingAnswersCount: res.data.pendingAnswersCount,
                approvedAnswersCount: res.data.approvedAnswersCount,
                deletedCiphersCount: res.data.deletedCiphersCount,
                pendingCipherTitles: res.data.pendingCipherTitles
            });
            console.log("DASHBOARD STATS:", res.data);
        })
    },[]);
    return (
        <div className="admin-shell">
            <AdminSidebar activePage="dashboard" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Dashboard' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Dashboard</h1>
                        <p className="page-subtitle">Overview of platform activity and quick access to admin tools</p>
                    </div>

                    {/* ─── Stats Row ─── */}
                    <div className="stats-grid">
                        <StatCard
                            label="Pending Submissions"
                            value={stats.pendingCiphersCount ?? '—'}
                            sub="Awaiting review"
                            accent="yellow"
                        />
                        <StatCard
                            label="Approved Ciphers"
                            value={stats.approvedCiphersCount ?? '—'}
                            sub="Live on platform"
                            accent="emerald"
                        />
                        
                        <StatCard
                            label="Pending Answers"
                            value={stats.pendingAnswersCount ?? '—'}
                            sub="Community suggestions"
                            accent="sky"
                        />
                        <StatCard
                            label="Approved Answers"
                            value={stats.approvedAnswersCount ?? '—'}
                            sub="Total accounts"
                            accent="violet"
                        />
                        <StatCard
                            label="Deleted Ciphers"
                            value={stats.deletedCiphersCount ?? '—'}
                            sub="All time"
                            accent="rose"
                        />
                    </div>

                    <div className="dashboard-grid">
                        {/* ─── Quick Actions ─── */}
                        <div className="admin-card">
                            <div className="admin-card-header">
                                <span className="admin-card-title">Quick Actions</span>
                            </div>
                            <div className="quick-actions">
                                <Link to="/admin/pending-ciphers" className="quick-action-link">
                                    <div className="qa-left">
                                        <div className="qa-icon yellow">
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                <circle cx="8" cy="8" r="6.5"/><path d="M8 4.5V8l2.5 2"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <div className="qa-label">Review Pending Ciphers</div>
                                            <div className="qa-count">{stats.pendingCiphersCount ?? 0} waiting</div>
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>

                                <Link to="/admin/manage-ciphers" className="quick-action-link">
                                    <div className="qa-left">
                                        <div className="qa-icon emerald">
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                <rect x="2.5" y="7" width="11" height="8" rx="1.5"/>
                                                <path d="M5 7V5a3 3 0 0 1 6 0v2"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <div className="qa-label">Manage Approved Ciphers</div>
                                            <div className="qa-count">{stats.approvedCiphersCount ?? 0} ciphers</div>
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>
                                <Link to="/admin/deleted-ciphers" className="quick-action-link">
                                    <div className="qa-left">
                                        <div className="qa-icon rose">
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                <path d="M2 4h12M5 4V2h6v2M6 7v5M10 7v5M3 4l1 10h8l1-10"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <div className="qa-label">Deleted Ciphers</div>
                                            <div className="qa-count">{stats.deletedCiphers ?? 0} deleted</div>
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>
                                <Link to="/admin/pending-answers" className="quick-action-link">
                                    <div className="qa-left">
                                        <div className="qa-icon sky">
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                <path d="M14 3H2a1 1 0 0 0-1 1v7a1 1 0 0 0 1 1h2v2.5L7 12h7a1 1 0 0 0 1-1V4a1 1 0 0 0-1-1z"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <div className="qa-label">Review Pending Answers</div>
                                            <div className="qa-count">{stats.pendingAnswersCount ?? 0} waiting</div>
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>

                                <Link to="/admin/users" className="quick-action-link">
                                    <div className="qa-left">
                                        <div className="qa-icon violet">
                                            <svg width="14" height="14" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                                                <circle cx="6" cy="5" r="2.5"/>
                                                <path d="M1 13c0-2.76 2.24-5 5-5s5 2.24 5 5"/>
                                                <path d="M11 3a2.5 2.5 0 0 1 0 5M15 13a4 4 0 0 0-4-4"/>
                                            </svg>
                                        </div>
                                        <div>
                                            <div className="qa-label">Manage Users</div>
                                            {/* <div className="qa-count">{stats.users ?? 0} accounts</div> */}
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>
                            </div>
                        </div>

                        {/* ─── Recent Activity ─── */}
                        <div className="admin-card">
                            <div className="admin-card-header">
                                <span className="admin-card-title">Recent Submissions</span>
                                <a href="/admin/pending-ciphers" className="btn btn-ghost btn-sm">View All</a>
                            </div>

                            {stats.pendingCipherTitles.length === 0 ? (
                                <div className="empty-state">
                                    <span className="empty-state-title">No recent submissions</span>
                                </div>
                            ) : (
                                <div className="data-table-wrapper" style={{ border: 'none' }}>
                                    <table className="data-table">
                                        <thead>
                                            <tr>
                                                <th>Cipher Title</th>
                                                <th>Submitted By</th>
                                                <th>Submitted At</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {stats.pendingCipherTitles.map((item) => (
                                                <tr key={item.id}>
                                                    <td>
                                                        <div className="activity-cipher-text">{item.title}</div>
                                                    </td>
                                                    <td className="mono">{item.createdBy}</td>
                                                    <td>
                                                      
                                                            {item.submittedAt}
                                                       
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default AdminDashboard;
