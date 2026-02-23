import React from 'react';
import CipherInputToggle from './CipherInputToggle';

const CIPHER_TYPES = [
    { value: '0',  label: 'Caesar',              group: 'Substitution' },
    { value: '1',  label: 'Atbash',              group: 'Substitution' },
    { value: '2',  label: 'Simple Substitution', group: 'Substitution' },
    { value: '3',  label: 'ROT13',               group: 'Substitution' },
    { value: '4',  label: 'Vigenere',            group: 'Polyalphabetic' },
    { value: '5',  label: 'Autokey',             group: 'Polyalphabetic' },
    { value: '6',  label: 'Trithemius',          group: 'Polyalphabetic' },
    { value: '7',  label: 'Rail Fence',          group: 'Transposition' },
    { value: '8',  label: 'Columnar',            group: 'Transposition' },
    { value: '9',  label: 'Route',               group: 'Transposition' },
    { value: '10', label: 'Base64',              group: 'Encoding' },
    { value: '11', label: 'Morse',               group: 'Encoding' },
    { value: '12', label: 'Binary',              group: 'Encoding' },
    { value: '13', label: 'Hex',                 group: 'Encoding' },
    { value: '14', label: 'Plaintext',           group: 'Special' },
];

const GROUPS = ['Substitution', 'Polyalphabetic', 'Transposition', 'Encoding', 'Special'];

const SubmitForm = ({ fields, useImage, onToggle, onFieldChange }) => {
    const { title, decryptedText, encryptedText, image, cipherType } = fields;

    /* Char counter state */
    const len = encryptedText.length;
    let countClass = 'char-count';
    let countNote  = 'min. 150 for ML';
    if (len === 0)       { countClass = 'char-count error'; countNote = 'required'; }
    else if (len < 150)  { countClass = 'char-count warn';  countNote = `${150 - len} more needed`; }
    else if (len <= 400) { countClass = 'char-count good';  countNote = 'good length'; }
    else                 { countNote = 'above optimal'; }

    /* Image upload */
    const handleZoneClick = () => document.getElementById('cipher-image-input').click();
    const handleFileChange = (e) => {
        const f = e.target.files[0];
        if (f) onFieldChange('image', f);
    };

    return (
        <div className="card">

            {/* 1. Title */}
            <div className="field">
                <label className="field-label">
                    TITLE
                </label>
                <input
                    type="text"
                    className="field-input"
                    placeholder="Give your cipher a name…"
                    value={title}
                    onChange={e => onFieldChange('title', e.target.value)}
                    maxLength={100}
                />
            </div>

            {/* 2. Plaintext solution */}
            <div className="field">
                <label className="field-label">
                    PLAINTEXT SOLUTION <span className="optional">OPTIONAL</span>
                </label>
                <textarea
                    className="field-textarea"
                    placeholder="The decrypted answer, if you know it…"
                    value={decryptedText}
                    onChange={e => onFieldChange('decryptedText', e.target.value)}
                    rows={3}
                />
                <p className="field-hint">
                    If you leave this blank the cipher will be marked Experimental — the community figures it out.
                </p>
            </div>

           
            <div className="field">
                

                <CipherInputToggle useImage={useImage} onToggle={onToggle} />

               
                {!useImage && (
                    <>
                        <textarea
                            className="field-textarea"
                            placeholder="Paste the encrypted text here…"
                            value={encryptedText}
                            onChange={e => onFieldChange('encryptedText', e.target.value)}
                            rows={6}
                        />
                        <div className="counter-row">
                            <span className="field-hint">150–400 characters is ideal for the ML classifier.</span>
                            <span className={countClass}>{len} — {countNote}</span>
                        </div>
                    </>
                )}

               
                {useImage && (
                    <>
                        <div
                            className={`upload-zone ${image ? 'has-file' : ''}`}
                            onClick={handleZoneClick}
                        >
                            <input
                                id="cipher-image-input"
                                type="file"
                                accept=".jpg,.jpeg,.png,.gif"
                                onChange={handleFileChange}
                            />

                            {image ? (
                                <>
                                    <p className="upload-title">File selected</p>
                                    <p className="upload-filename">{image.name}</p>
                                    <p className="upload-desc">Click to change</p>
                                </>
                            ) : (
                                <>
                                    <p className="upload-title">Click to upload</p>
                                    <p className="upload-desc">Photo, screenshot or scanned document</p>
                                    <div className="upload-formats">
                                        <span className="upload-format">JPG</span>
                                        <span className="upload-format">PNG</span>
                                        <span className="upload-format">GIF</span>
                                    </div>
                                </>
                            )}
                        </div>

                        <p className="ocr-notice">
                            <strong>OCR:</strong> Tesseract will extract text from the image.
                            You can review and correct it before the cipher goes live.
                        </p>
                    </>
                )}
            </div>

            
            <div className="field">
                <label className="field-label">
                    CIPHER TYPE <span className="optional">OPTIONAL</span>
                </label>
                <select
                    className="field-select"
                    value={cipherType}
                    onChange={e => onFieldChange('cipherType', e.target.value)}
                >
                    <option value="">Unknown — let the ML decide</option>
                    {GROUPS.map(group => (
                        <optgroup key={group} label={group}>
                            {CIPHER_TYPES
                                .filter(t => t.group === group)
                                .map(t => (
                                    <option key={t.value} value={t.value}>{t.label}</option>
                                ))
                            }
                        </optgroup>
                    ))}
                </select>
                <p className="field-hint">
                    Leave blank if unsure — the ML classifier predicts this during admin review.
                </p>
            </div>

        </div>
    );
};

export default SubmitForm;