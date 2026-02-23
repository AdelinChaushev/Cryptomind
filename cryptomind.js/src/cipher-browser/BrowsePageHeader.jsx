// Props:
//   totalCiphers  — number, total approved ciphers in the system
//   activeSolvers — number, users active today

const BrowsePageHeader = () => {
    return (
        <section className="page-header">
            <div className="page-header-inner">
                <div className="page-header-label">
                    <span className="label-dot"></span>
                    CIPHER VAULT
                </div>
                <h1 className="page-title">
                    Browse <span className="title-accent">Challenges</span>
                </h1>
                <p className="page-subtitle">
                    Decrypt, identify, and conquer classical cipher challenges submitted by the community.
                </p>
            </div>
            {/* <div className="page-header-stats">
                <div className="stat-pill">
                    <span className="stat-num">{totalCiphers}</span>
                    <span className="stat-label">Total Challenges</span>
                </div>
                <div className="stat-pill">
                    <span className="stat-num">{activeSolvers}</span>
                    <span className="stat-label">Active Solvers</span>
                </div>
            </div> */}
        </section>
    );
};

export default BrowsePageHeader;
