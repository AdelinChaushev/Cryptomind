// Props:
//   resultsCount       — number, how many ciphers are shown
//   activeFilters      — array of { label: string, onRemove: () => void }
//                        each entry renders as a removable pill
//   sortBy             — string, current sort value
//   onSortChange       — (value: string) => void
//   activeFilterCount  — number, shown as badge on the mobile filter button
//   onMobileFilterToggle — () => void, opens/closes the sidebar on mobile

const SORT_OPTIONS = [
    { value: 0,          label: 'Newest' },
    { value: 1,          label: 'Oldest' },
    { value: 2,         label: 'Most Popular' },
    {value : 3,label:'Least Solved'}
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
                    Showing <span className="results-count">{resultsCount}</span> challenges
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
                    <label className="sort-label" htmlFor="sortSelect">Sort by</label>
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

            {/* Mobile filter toggle — only visible below 768px via CSS */}
            <button className="mobile-filter-btn" onClick={onMobileFilterToggle}>
                <span>⚙ Filters</span>
                {activeFilterCount > 0 && (
                    <span className="mobile-filter-badge">{activeFilterCount}</span>
                )}
            </button>
        </>
    );
};

export default ContentTopbar;
