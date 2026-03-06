import React from 'react';

function DeactivateModal({ onConfirm, onCancel, loading }) {
    return (
        <div className="modal-overlay" onClick={onCancel}>
            <div className="modal-card" onClick={(e) => e.stopPropagation()}>
                <div className="modal-icon">⚠️</div>
                <div className="modal-title">Деактивиране на акаунт</div>
                <div className="modal-body">
                    Сигурни ли сте, че искате да деактивирате акаунта си?
                    <br />
                    <strong>Това действие не може да бъде отменено.</strong>
                    <br /><br />
                    Ще загубите достъп до всичките си шифри, предложения и прогрес в класацията.
                </div>
                <div className="modal-actions">
                    <button
                        className="btn-modal-cancel"
                        onClick={onCancel}
                        disabled={loading}
                    >
                        Отказ
                    </button>
                    <button
                        className="btn-modal-confirm"
                        onClick={onConfirm}
                        disabled={loading}
                    >
                        {loading ? 'Деактивиране...' : 'Да, деактивирай'}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default DeactivateModal;