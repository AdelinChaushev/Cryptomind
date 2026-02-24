function CipherHeader({ cipher }) {
    return (
        <div className="cipher-header">
            <div className="cipher-header-top">
                <div>
                    <p className="cipher-id">ШИФЪР #{cipher.id}</p>
                    <h1 className="cipher-title">{cipher.title}</h1>
                    <div className="cipher-tags">
                        {cipher.cipherType && (
                            <span className="tag tag-type">{cipher.cipherType}</span>
                        )}
                        {cipher.tags?.map((tag, i) => (
                            <span key={i} className="tag tag-cipher">{tag}</span>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CipherHeader;
