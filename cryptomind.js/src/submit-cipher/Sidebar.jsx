const Sidebar = () => {
    return (
        <>
            <div className="info-card">
                <p className="info-card-title">КАКВО СЛЕДВА</p>
                <div className="info-card-body">
                    <div className="steps">
                        <div className="step">
                            <span className="step-num">1</span>
                            <div className="step-body">
                                <p className="step-title">ML АНАЛИЗ</p>
                                <p className="step-desc">Невронната мрежа класифицира вида на шифъра и извежда оценка на увереност.</p>
                            </div>
                        </div>
                        <div className="step">
                            <span className="step-num">2</span>
                            <div className="step-body">
                                <p className="step-title">ПРЕГЛЕД ОТ АДМИНИСТРАТОР</p>
                                <p className="step-desc">Администратор проверява резултата. Ако увереността е под 85%, стартира LLM асистента.</p>
                            </div>
                        </div>
                        <div className="step">
                            <span className="step-num">3</span>
                            <div className="step-body">
                                <p className="step-title">ПУБЛИКУВАНЕ</p>
                                <p className="step-desc">Одобрен като Стандартен (известен отговор) или Експериментален (общността го решава).</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="info-card">
                <p className="info-card-title">СЪВЕТИ</p>
                <div className="info-card-body">
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>150+ знака</strong> — ML класификаторът се нуждае от достатъчно текст. 200–400 е оптималният диапазон.</p>
                    </div>
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>Само на английски</strong> — системата е обучена върху английски текст.</p>
                    </div>
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>Един шифър</strong> — не влагайте или комбинирайте шифри. Един шифър на предложение.</p>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Sidebar;