const SubmissionsHeader = ({ stats = {} }) => {
    const { total = 0, approved = 0, pending = 0, rejected = 0 } = stats;

    return (
        <header className="page-header">
            <nav className="breadcrumb" aria-label="Breadcrumb">
                <a href="/">Начало</a>
                <span className="breadcrumb-sep">›</span>
                <span className="breadcrumb-current">Моите предложения</span>
            </nav>

            <div className="header-label">
                <span className="label-dot"></span>
                ПОТРЕБИТЕЛСКО ТАБЛО
            </div>

            <h1 className="page-title">Моите <span>Предложения</span></h1>
            <p className="page-subtitle">
                Следете вашите предложени шифри и отговори към всички предизвикателства.
            </p>

            <div className="header-stats">
                <div className="hstat">
                    <span className="hstat-value yellow">{total}</span>
                    <span className="hstat-label">Общо</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value emerald">{approved}</span>
                    <span className="hstat-label">Одобрени</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value">{pending}</span>
                    <span className="hstat-label">Изчакващи</span>
                </div>
                <div className="hstat-divider"></div>
                <div className="hstat">
                    <span className="hstat-value rose">{rejected}</span>
                    <span className="hstat-label">Отхвърлени</span>
                </div>
            </div>
        </header>
    );
};

export default SubmissionsHeader;