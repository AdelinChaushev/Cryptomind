import React, { useState, useEffect } from 'react';
import AdminSidebar from './AdminSidebar';
import AdminTopbar from './AdminTopbar';
import StatCard from './StatCard';
import '../styles/admin-dashboard.css';
import axios from 'axios';
import { Link } from 'react-router-dom';
import { useError } from "../ErrorContext";
import useLogout from '../components/Logout';

const AdminDashboard = () => {
    const { setError } = useError(null);
     const logout = useLogout();
    const [stats, setStats] = useState({  
        pendingCiphersCount: 0,
        approvedCiphersCount: 0,
        pendingAnswersCount: 0,
        approvedAnswersCount: 0,
        deletedCiphersCount: 0,
        pendingCipherTitles: []
    });
  
    useEffect(() => {
        axios.get(`${import.meta.env.VITE_API_URL}/api/admin/dashboard`)
        .then(res => {
            setStats({
                pendingCiphersCount: res.data.pendingCiphersCount,
                approvedCiphersCount: res.data.approvedCiphersCount,
                pendingAnswersCount: res.data.pendingAnswersCount,
                approvedAnswersCount: res.data.approvedAnswersCount,
                deletedCiphersCount: res.data.deletedCiphersCount,
                pendingCipherTitles: res.data.pendingCipherTitles
            });
            console.log("СТАТИСТИКА ЗА ТАБЛОТО:", res.data);
        }).catch(err => { 
            setError(err.response?.data?.message || "Грешка при зареждане на статистиката"); 
            console.error(err); 
        });
    }, [setError]);

    return (
        <div className="admin-shell">
            <AdminSidebar activePage="dashboard" />

            <main className="admin-main">
                <AdminTopbar breadcrumbs={[{ label: 'Табло за управление' }]} />

                <div className="admin-content">
                    <div className="page-header">
                        <h1 className="page-title">Табло за управление</h1>
                    </div>

                    
                    <div className="stats-grid">
                        <StatCard
                            label="Изчакващи предложения"
                            value={stats.pendingCiphersCount ?? '—'}
                            sub="Очакват преглед"
                            accent="yellow"
                        />
                        <StatCard
                            label="Одобрени шифри"
                            value={stats.approvedCiphersCount ?? '—'}
                            sub="Активни в платформата"
                            accent="emerald"
                        />
                        
                        <StatCard
                            label="Изчакващи отговори"
                            value={stats.pendingAnswersCount ?? '—'}
                            sub="Предложения от общността"
                            accent="sky"
                        />
                        <StatCard
                            label="Одобрени отговори"
                            value={stats.approvedAnswersCount ?? '—'}
                            sub="Общо потвърдени"
                            accent="violet"
                        />
                        <StatCard
                            label="Изтрити шифри"
                            value={stats.deletedCiphersCount ?? '—'}
                            sub="За цялото време"
                            accent="rose"
                        />
                    </div>

                    <div className="dashboard-grid">
                        
                        <div className="admin-card">
                            <div className="admin-card-header">
                                <span className="admin-card-title">Бързи действия</span>
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
                                            <div className="qa-label">Преглед на изчакващи шифри</div>
                                            <div className="qa-count">{stats.pendingCiphersCount ?? 0} чакащи</div>
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
                                            <div className="qa-label">Управление на одобрени шифри</div>
                                            <div className="qa-count">{stats.approvedCiphersCount ?? 0} шифъра</div>
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
                                            <div className="qa-label">Изтрити шифри</div>
                                            <div className="qa-count">{stats.deletedCiphersCount ?? 0} изтрити</div>
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
                                            <div className="qa-label">Преглед на изчакващи отговори</div>
                                            <div className="qa-count">{stats.pendingAnswersCount ?? 0} чакащи</div>
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
                                            <div className="qa-label">Управление на потребители</div>
                                        </div>
                                    </div>
                                    <span className="qa-arrow">→</span>
                                </Link>
                            </div>
                        </div>

                        
                        <div className="admin-card">
                            <div className="admin-card-header">
                                <span className="admin-card-title">Последни предложения</span>
                                <a href="/admin/pending-ciphers" className="btn btn-ghost btn-sm">Виж всички</a>
                            </div>

                            {stats.pendingCipherTitles.length === 0 ? (
                                <div className="empty-state">
                                    <span className="empty-state-title">Няма скорошни предложения</span>
                                </div>
                            ) : (
                                <div className="data-table-wrapper" style={{ border: 'none' }}>
                                    <table className="data-table">
                                        <thead>
                                            <tr>
                                                <th>Заглавие на шифъра</th>
                                                <th>Изпратено от</th>
                                                <th>Дата на изпращане</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {stats.pendingCipherTitles.map((item) => (
                                                <tr key={item.id}>
                                                    <td>
                                                        <div className="activity-cipher-text">{item.title}</div>
                                                    </td>
                                                    <td className="mono">{item.createdBy}</td>
                                                    <td>{item.submittedAt}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                    
                                </div>
                            )}
                        </div>
                    </div>
                      <button className="admin-mobile-logout" onClick={logout}>
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
                            <path d="M6 2H2.5A1.5 1.5 0 0 0 1 3.5v9A1.5 1.5 0 0 0 2.5 14H6"/>
                            <path d="M11 11l4-3.5L11 4M6 8.5h9"/>
                        </svg>
                        Изход
                    </button>
                </div>
            </main>
        </div>
    );
};

export default AdminDashboard;