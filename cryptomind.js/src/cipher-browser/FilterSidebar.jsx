// Props:
//   isOpen           — boolean, controls mobile visibility (toggle .mobile-open class)
//   searchTerm       — string, current value of the search input
//   challengeType    — number: 0 = All, 1 = Standard, 2 = Experimental
//   selectedTags     — number[], list of selected TagType enum values
//   onSearchChange   — (value: string) => void
//   onChallengeTypeChange — (value: number) => void
//   onTagChange      — (value: number, checked: boolean) => void
//   onClearAll       — () => void
//   onApply          — () => void

// Matches the TagType enum in C# — align values with your actual enum integers
const TAGS = [
    { value: 1, label: 'Image' },
    { value: 2, label: 'Puzzle' },
    { value: 3, label: 'Historical' },
    { value: 4, label: 'Short' },
    { value: 5, label: 'Long' },
    { value: 6, label: 'Beginner Friendly' },
    { value: 7, label: 'Tricky' },
];

const CHALLENGE_TYPES = [
    //{ value: 0, label: 'All' },
    { value: 0, label: 'Standard' },
    { value: 1, label: 'Experimental' },
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
        // Pass current filter state to parent
        onApplyFilters && onApplyFilters({ searchTerm, challengeType, selectedTags });
    };

    return (
        <aside className={`filter-sidebar${isOpen ? ' mobile-open' : ''}`}>

            <div className="sidebar-header">
                <h2 className="sidebar-title">
                    <span className="sidebar-icon">⚙</span>
                    Filters
                </h2>
                <button className="clear-all-btn" onClick={onClearAll}>
                    Clear all
                </button>
            </div>

            <form noValidate onSubmit={handleSubmit}>

                {/* Search */}
                <div className="filter-section">
                    <label className="filter-label" htmlFor="searchInput">Search</label>
                    <div className="search-wrapper">
                        <span className="search-icon">⌕</span>
                        <input
                            type="text"
                            id="searchInput"
                            name="searchTerm"
                            className="search-input"
                            placeholder="Search by title..."
                            autoComplete="off"
                            value={searchTerm}
                            onChange={(e) => onSearchChange && onSearchChange(e.target.value)}
                        />
                    </div>
                </div>

                {/* Challenge Type */}
                <div className="filter-section">
                    <label className="filter-label">Challenge Type</label>
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

                {/* Tags */}
                <div className="filter-section">
                    <label className="filter-label">Tags</label>
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
                    Apply Filters
                </button>

            </form>
        </aside>
    );
};

export default FilterSidebar;
