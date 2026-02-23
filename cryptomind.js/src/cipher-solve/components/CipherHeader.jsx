function CipherHeader({ cipher }) {
    const difficultyClass = cipher.difficulty?.toLowerCase() || "medium";

    return (
        <div className="cipher-header">
            <div className="cipher-header-top">
                <div>
                    <p className="cipher-id">CIPHER #{cipher.id}</p>
                    <h1 className="cipher-title">{cipher.title}</h1>
                    {/* <p className="cipher-description"></p> */}
                    <div className="cipher-tags">
                        {/* <span className={`tag tag-${difficultyClass}`}>
                            {cipher.difficulty}
                        </span> */}
                        {cipher.cipherType && (
                            <span className="tag tag-type">{cipher.cipherType}</span>
                        )}
                        {cipher.tags?.map((tag, i) => (
                            <span key={i} className="tag tag-cipher">{tag}</span>
                        ))}
                    </div>
                </div>

                {/* <div className={`difficulty-badge ${difficultyClass}`}>
                    <span className="difficulty-label">Difficulty</span>
                    <span className="difficulty-value">{cipher.difficulty}</span>
                </div> */}
            </div>
        </div>
    );
}

export default CipherHeader;
