const TAGS = [
    { value: 1, label: 'Изображение' },
    { value: 2, label: 'Пъзел' },
    { value: 3, label: 'Исторически' },
    { value: 4, label: 'Кратък' },
    { value: 5, label: 'Дълъг' },
    { value: 6, label: 'Подходящ за начинаещи' },
    { value: 7, label: 'Труден' },
];

const CHALLENGE_TYPES = [
    { value: 0, label: 'Стандартен' },
    { value: 1, label: 'Експериментален' },
];

const FilterSidebar = ({
    isOpen = false,
    searchTerm = '',
    challengeType = 0,
    selectedTags = [],
    onSearchChange,
    onChallengeTypeChange,
    onTagChange,
    onClearAll,
    onApplyFilters,
}) => {
     const handleSubmit = (e) => {
        e.preventDefault();
        onApplyFilters && onApplyFilters({ searchTerm, challengeType, selectedTags });
    };

    return (
        <aside className={`filter-sidebar${isOpen ? ' mobile-open' : ''}`}>

            <div className="sidebar-header">
                <h2 className="sidebar-title">
                    <span className="sidebar-icon">⚙</span>
                    Филтри
                </h2>
                <button className="clear-all-btn" onClick={onClearAll}>
                    Изчисти всички
                </button>
            </div>

            <form noValidate onSubmit={handleSubmit}>

                <div className="filter-section">
                    <label className="filter-label" htmlFor="searchInput">Търсене</label>
                    <div className="search-wrapper">
                        <span className="search-icon">⌕</span>
                        <input
                            type="text"
                            id="searchInput"
                            name="searchTerm"
                            className="search-input"
                            placeholder="Търсете по заглавие..."
                            autoComplete="off"
                            value={searchTerm}
                            onChange={(e) => onSearchChange && onSearchChange(e.target.value)}
                        />
                    </div>
                </div>

                <div className="filter-section">
                    <label className="filter-label">Вид предизвикателство</label>
                    <div className="challenge-type-group">
                        {CHALLENGE_TYPES.map((type) => (
                            <label className="type-radio-label" key={type.value}>
                                <input
                                    type="radio"
                                    name="challengeType"
                                    value={type.value}
                                    className="type-radio"
                                    checked={challengeType === type.value}
                                    onChange={() => onChallengeTypeChange && onChallengeTypeChange(type.value)}
                                />
                                <span className="type-radio-custom">{type.label}</span>
                            </label>
                        ))}
                    </div>
                </div>

                <div className="filter-section">
                    <label className="filter-label">Етикети</label>
                    <div className="tags-group">
                        {TAGS.map((tag) => (
                            <label className="tag-check-label" key={tag.value}>
                                <input
                                    type="checkbox"
                                    name="tags"
                                    value={tag.value}
                                    className="tag-check"
                                    checked={selectedTags.includes(tag.value)}
                                    onChange={(e) => onTagChange && onTagChange(tag.value, e.target.checked)}
                                />
                                <span className="tag-check-custom">{tag.label}</span>
                            </label>
                        ))}
                    </div>
                </div>

                <button type="submit" className="btn btn-primary btn-apply">
                    Приложи филтрите
                </button>

            </form>
        </aside>
    );
};

export default FilterSidebar;