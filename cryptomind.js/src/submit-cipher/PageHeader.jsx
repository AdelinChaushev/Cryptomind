const PageHeader = () => {
    return (
        <header className="page-header">
            <nav className="breadcrumb">
                <a href="/">Начало</a>
                <span className="breadcrumb-sep">›</span>
                <span>Предложи шифър</span>
            </nav>
            <h1 className="page-title">Предложи <span>Шифър</span></h1>
            <p className="page-subtitle">Допринесете с предизвикателство за общността.</p>
        </header>
    );
};

export default PageHeader;