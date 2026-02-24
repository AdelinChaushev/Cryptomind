import React, { useState, useEffect } from 'react';
import '../styles/account-info.css';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useError } from '../ErrorContext';
import ProfileCard    from './ProfileCard';
import StatsSection   from './StatsSection';
import BadgesSection  from './BadgesSection';
import DeactivateModal from './DeactivateModal';

const API_BASE = 'http://localhost:5115';

function AccountInfo() {
    const [user, setUser]               = useState(null);
    const [loading, setLoading]         = useState(true);
    const {setError} = useError();
    const [showModal, setShowModal]     = useState(false);
    const [deactivating, setDeactivating] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUser = () => {
            axios.get(`${API_BASE}/api/user/get-account-info`, {
                withCredentials: true,
            }).then(res => {
                console.log('Fetched account info:', res.data);
                setUser(res.data);
            }).catch(err => {
                setError("Грешка при зареждане на информацията за акаунта:", err.response?.status);
            }).finally(() => setLoading(false));
        };

        fetchUser();
    }, []);

    const handleDeactivate = async () => {
        setDeactivating(true);
        try {
            await axios.post(`${API_BASE}/api/auth/deactivate`, {}, {
                withCredentials: true,
            });
            navigate('/');
        } catch (err) {
            setError(`Неуспешно деактивиране на акаунта: ${err.message}`);
        } finally {
            setDeactivating(false);
            setShowModal(false);
        }
    };

    if (loading) {
        return (
            <div className="account-page">
                <div className="account-loading">
                    <div className="loading-spinner" />
                    <div className="loading-text">Зареждане на информацията за акаунта...</div>
                </div>
            </div>
        );
    }

    return (
        <div className="account-page">
            <div className="account-page-header">
                <div className="account-breadcrumb">
                    <span>Начало</span>
                    <span className="breadcrumb-sep">/</span>
                    <span className="breadcrumb-current">Акаунт</span>
                </div>
                <h1 className="account-page-title">
                    Моят <span>Акаунт</span>
                </h1>
                <p className="account-page-subtitle">
                    Управлявайте профила си, следете напредъка си и преглеждайте спечелените значки.
                </p>
            </div>

            <div className="account-layout">
                <ProfileCard
                    user={user}
                    onDeactivate={() => setShowModal(true)}
                    deactivating={deactivating}
                />

                <div className="account-right">
                    <StatsSection user={user} />
                    <BadgesSection badges={user.badges} />
                </div>
            </div>

            {showModal && (
                <DeactivateModal
                    onConfirm={handleDeactivate}
                    onCancel={() => setShowModal(false)}
                    loading={deactivating}
                />
            )}
        </div>
    );
}

export default AccountInfo;