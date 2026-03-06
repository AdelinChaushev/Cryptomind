import CipherInputToggle from './CipherInputToggle';

const CIPHER_TYPES = [
    { value: '0',  label: 'Цезар (Caesar)',                          group: 'Substitution' },
    { value: '1',  label: 'Атбаш (Atbash)',                          group: 'Substitution' },
    { value: '2',  label: 'Проста замяна (SimpleSubstitution)',       group: 'Substitution' },
    { value: '3',  label: 'ROT13 (ROT13)',                           group: 'Substitution' },
    { value: '4',  label: 'Виженер (Vigenere)',                       group: 'Polyalphabetic' },
    { value: '5',  label: 'Автоключ (Autokey)',                       group: 'Polyalphabetic' },
    { value: '6',  label: 'Тритемий (Trithemius)',                    group: 'Polyalphabetic' },
    { value: '7',  label: 'Железопътна ограда (RailFence)',           group: 'Transposition' },
    { value: '8',  label: 'Колонна (Columnar)',                       group: 'Transposition' },
    { value: '9',  label: 'Маршрут (Route)',                          group: 'Transposition' },
    { value: '10', label: 'Base64 (Base64)',                          group: 'Encoding' },
    { value: '11', label: 'Морзов (Morse)',                           group: 'Encoding' },
    { value: '12', label: 'Двоичен (Binary)',                         group: 'Encoding' },
    { value: '13', label: 'Шестнадесетичен (Hex)',                    group: 'Encoding' },
];

const GROUPS = ['Substitution', 'Polyalphabetic', 'Transposition', 'Encoding'];

const GROUP_LABELS = {
    'Substitution':   'Заместване',
    'Polyalphabetic': 'Полиазбучни',
    'Transposition':  'Транспозиция',
    'Encoding':       'Кодиране',
};

const SubmitForm = ({
    fields,
    useImage,
    onToggle,
    onFieldChange,
    ocrText,
    ocrFailed,
    ocrLoading,
    onOcrTextChange,
    onImageChange,
}) => {
    const { title, decryptedText, encryptedText, image, cipherType } = fields;

    const len = encryptedText.length;
    let countClass = 'char-count';
    let countNote  = 'мин. 150 за ML';
    if (len === 0)       { countClass = 'char-count error'; countNote = 'задължително'; }
    else if (len < 150)  { countClass = 'char-count warn';  countNote = `още ${150 - len} знака`; }
    else if (len <= 400) { countClass = 'char-count good';  countNote = 'добра дължина'; }
    else                 { countNote = 'над оптималното'; }

    const handleZoneClick = () => document.getElementById('cipher-image-input').click();

    const handleFileChange = (e) => {
        const f = e.target.files[0];
        if (f) onImageChange(f);
    };

    return (
        <div className="card">

            <div className="field">
                <label className="field-label">
                    ЗАГЛАВИЕ
                </label>
                <input
                    type="text"
                    className="field-input"
                    placeholder="Дайте име на вашия шифър…"
                    value={title}
                    onChange={e => onFieldChange('title', e.target.value)}
                    maxLength={100}
                />
            </div>

            <div className="field">
                <label className="field-label">
                    РЕШЕНИЕ НА ОТКРИТ ТЕКСТ <span className="optional">НЕЗАДЪЛЖИТЕЛНО</span>
                </label>
                <textarea
                    className="field-textarea"
                    placeholder="Декриптираният отговор, ако го знаете…"
                    value={decryptedText}
                    onChange={e => onFieldChange('decryptedText', e.target.value)}
                    rows={3}
                />
                <p className="field-hint">
                    Ако оставите това поле празно, шифърът ще бъде маркиран като Експериментален — общността ще го разгадае.
                </p>
            </div>

            <div className="field">
                <CipherInputToggle useImage={useImage} onToggle={onToggle} />

                {!useImage && (
                    <>
                        <textarea
                            className="field-textarea"
                            placeholder="Поставете криптирания текст тук…"
                            value={encryptedText}
                            onChange={e => onFieldChange('encryptedText', e.target.value)}
                            rows={6}
                        />
                        <div className="counter-row">
                            <span className="field-hint">150–400 знака е идеално за ML класификатора.</span>
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
                                    <p className="upload-title">Файлът е избран</p>
                                    <p className="upload-filename">{image.name}</p>
                                    <p className="upload-desc">Натиснете за смяна</p>
                                </>
                            ) : (
                                <>
                                    <p className="upload-title">Натиснете за качване</p>
                                    <p className="upload-desc">Снимка, екранна снимка или сканиран документ</p>
                                    <div className="upload-formats">
                                        <span className="upload-format">JPG</span>
                                        <span className="upload-format">PNG</span>
                                        <span className="upload-format">GIF</span>
                                    </div>
                                </>
                            )}
                        </div>

                        {ocrLoading && (
                            <p className="ocr-loading">Извличане на текст от изображението…</p>
                        )}

                        {!ocrLoading && image && !ocrFailed && (
                            <div className="ocr-review">
                                <label className="field-label">
                                    ИЗВЛЕЧЕН ТЕКСТ <span className="optional">ПРЕГЛЕДАЙТЕ И КОРИГИРАЙТЕ</span>
                                </label>
                                <textarea
                                    className="field-textarea ocr-textarea"
                                    value={ocrText}
                                    onChange={e => onOcrTextChange(e.target.value)}
                                    rows={6}
                                />
                                <p className="field-hint">
                                    Tesseract извлече горния текст от изображението. Коригирайте евентуални грешки преди изпращане.
                                </p>
                            </div>
                        )}

                        {!ocrLoading && ocrFailed && image && (
                            <p className="ocr-notice ocr-notice--error">
                                OCR не успя да извлече текст. Опитайте с по-ясно изображение.
                            </p>
                        )}

                        {!image && (
                            <p className="ocr-notice">
                                <strong>OCR:</strong> Tesseract ще извлече текста от изображението.
                                Можете да го прегледате и коригирате, преди шифърът да бъде публикуван.
                            </p>
                        )}
                    </>
                )}
            </div>

            <div className="field">
                <label className="field-label">
                    ВИД НА ШИФЪРА <span className="optional">НЕЗАДЪЛЖИТЕЛНО</span>
                </label>
                <select
                    className="field-select"
                    value={cipherType}
                    onChange={e => onFieldChange('cipherType', e.target.value)}
                >
                    <option value="">Неизвестен — нека ML реши</option>
                    {GROUPS.map(group => (
                        <optgroup key={group} label={GROUP_LABELS[group]}>
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
                    Оставете празно ако не сте сигурни — ML класификаторът ще предвиди вида при прегледа от администратор.
                </p>
            </div>

        </div>
    );
};

export default SubmitForm;