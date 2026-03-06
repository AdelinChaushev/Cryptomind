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
                            <span className="cipher-tag sub">Цезар (Caesar)</span>
                            <span className="cipher-tag sub">Атбаш (Atbash)</span>
                            <span className="cipher-tag sub">ROT13 (ROT13)</span>
                            <span className="cipher-tag sub">Проста замяна (SimpleSubstitution)</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">ПОЛИАЗБУЧНИ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag poly">Виженер (Vigenere)</span>
                            <span className="cipher-tag poly">Автоключ (Autokey)</span>
                            <span className="cipher-tag poly">Тритемий (Trithemius)</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">ТРАНСПОЗИЦИЯ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag trans">Железопътна ограда (RailFence)</span>
                            <span className="cipher-tag trans">Колонна (Columnar)</span>
                            <span className="cipher-tag trans">Маршрут (Route)</span>
                        </div>
                    </div>
                    <div>
                        <p className="cipher-family-name">КОДИРАНЕ</p>
                        <div className="cipher-tags">
                            <span className="cipher-tag enc">Base64 (Base64)</span>
                            <span className="cipher-tag enc">Морзов (Morse)</span>
                            <span className="cipher-tag enc">Двоичен (Binary)</span>
                            <span className="cipher-tag enc">Шестнадесетичен (Hex)</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CipherTypesPanel;