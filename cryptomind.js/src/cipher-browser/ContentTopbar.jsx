const SORT_OPTIONS = [
    { value: 0, label: 'Най-нови' },
    { value: 1, label: 'Най-стари' },
    { value: 2, label: 'Най-популярни' },
    { value: 3, label: 'Най-малко решавани' }
];

const ContentTopbar = ({
    resultsCount = 0,
    activeFilters = [],
    sortBy = 0,
    onSortChange,
    activeFilterCount = 0,
    onMobileFilterToggle,
}) => {
    return (
        <>
            <div className="content-topbar">
                <div className="results-info">
                    Показване на <span className="results-count">{resultsCount}</span> предизвикателства
                    <span className="active-filters-row">
                        {activeFilters.map((filter, index) => (
                            <span className="active-filter-pill" key={index}>
                                {filter.label}
                                <span
                                    className="pill-remove"
                                    onClick={filter.onRemove}
                                >
                                    ×
                                </span>
                            </span>
                        ))}
                    </span>
                </div>
                <div className="sort-wrapper">
                    <label className="sort-label" htmlFor="sortSelect">Сортирай по</label>
                    <select
                        className="sort-select"
                        id="sortSelect"
                        value={sortBy}
                        onChange={(e) => onSortChange && onSortChange(e.target.value)}
                    >
                        {SORT_OPTIONS.map((opt) => (
                            <option value={opt.value} key={opt.value}>
                                {opt.label}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            <button className="mobile-filter-btn" onClick={onMobileFilterToggle}>
                <span>⚙ Филтри</span>
                {activeFilterCount > 0 && (
                    <span className="mobile-filter-badge">{activeFilterCount}</span>
                )}
            </button>
        </>
    );
};

export default ContentTopbar;