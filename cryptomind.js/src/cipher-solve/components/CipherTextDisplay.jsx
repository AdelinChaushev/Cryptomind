import { useState } from "react";

function CipherTextDisplay({ encryptedText, hasImage, imageUrl }) {
    const [copied, setCopied] = useState(false);

    function handleCopy() {
        navigator.clipboard.writeText(encryptedText).then(() => {
            setCopied(true);
            setTimeout(() => setCopied(false), 2000);
        });
    }

    return (
        <div className="ciphertext-panel">
            <div className="panel-topbar">
                <div className="panel-topbar-left">
                    <span className="topbar-dot dot-red"></span>
                    <span className="topbar-dot dot-yellow"></span>
                    <span className="topbar-dot dot-green"></span>
                    <span className="panel-label">encrypted_message.txt</span>
                </div>
                <button
                    className={`btn-copy ${copied ? "copied" : ""}`}
                    onClick={handleCopy}
                >
                    {copied ? "✓ Копирано" : "⎘ Копирай"}
                </button>
            </div>

            <div className="ciphertext-body">
                {hasImage && imageUrl && (
                    <img
                        src={imageUrl}
                        alt="Шифър"
                        style={{ maxWidth: "100%", borderRadius: "6px" }}
                    />
                )}
                {encryptedText && !hasImage && (
                    <div className="ciphertext-content">{encryptedText}</div>
                )}
            </div>
        </div>
    );
}

export default CipherTextDisplay;
