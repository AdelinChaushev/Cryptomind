const TAGS = [
    { value: "Image",              label: 'Изображение' },
    { value: "Puzzle",             label: 'Пъзел' },
    { value: "Historical",         label: 'Исторически' },
    { value: "Short",              label: 'Кратък' },
    { value: "Long",               label: 'Дълъг' },
    { value: "Beginner_Friendly",  label: 'Подходящ за начинаещи' },
    { value: "Tricky",             label: 'Труден' },
];

function resolveTagLabel(raw) {
    const matched = TAGS.find(
        (t) =>
            t.value === raw ||
            t.label.toLowerCase() === String(raw).toLowerCase() ||
            t.label.replace(/_/g, ' ').toLowerCase() === String(raw).replace(/_/g, ' ').toLowerCase()
    );
    return matched ? matched.label : raw;
}

function CipherHeader({ cipher }) {
    return (
        <div className="cipher-header">
            <div className="page-header-label">
                <span className="label-dot"></span>
                Предизвикателство
            </div>

            <h1 className="cipher-title">
                <span className="title-accent">{cipher.title}</span>
            </h1>

            <div className="cipher-tags">
                {cipher.cipherType && (
                    <span className="tag tag-type">{cipher.cipherType}</span>
                )}
                {cipher.tags?.map((tag, i) => (
                    <span key={i} className="tag tag-cipher">
                        {resolveTagLabel(tag)}
                    </span>
                ))}
            </div>
        </div>
    );
}

export default CipherHeader;