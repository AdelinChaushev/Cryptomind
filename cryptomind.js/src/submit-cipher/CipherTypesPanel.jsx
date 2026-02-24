import React from 'react';

const CipherTypesPanel = () => {
    return (
        <div className="info-card cipher-types-panel">
            <p className="info-card-title">ПОДДЪРЖАНИ ВИДОВЕ</p>
            <div className="info-card-body">
                <div className="cipher-ref">
                    <div>
                        <p className="cipher-family-name">ЗАМЕСТВАНЕ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag sub">Цезар</span>
                            <span className="cipher-tag sub">Атбаш</span>
                            <span className="cipher-tag sub">ROT13</span>
                            <span className="cipher-tag sub">Проста замяна</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">ПОЛИАЗБУЧНИ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag poly">Виженер</span>
                            <span className="cipher-tag poly">Автоключ</span>
                            <span className="cipher-tag poly">Тритемий</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">ТРАНСПОЗИЦИЯ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag trans">Железопътна ограда</span>
                            <span className="cipher-tag trans">Колонна</span>
                            <span className="cipher-tag trans">Маршрут</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">КОДИРАНЕ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag enc">Base64</span>
                            <span className="cipher-tag enc">Морзе</span>
                            <span className="cipher-tag enc">Двоичен</span>
                            <span className="cipher-tag enc">Шестнадесетичен</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CipherTypesPanel;