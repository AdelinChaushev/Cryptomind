// Props:
//   currentPage  — number, 1-based
//   totalPages   — number
//   onPageChange — (page: number) => void

const Pagination = ({ currentPage = 1, totalPages = 1, onPageChange }) => {
    // Build the array of page numbers to display.
    // Shows up to 5 page buttons centered around the current page.
    const getPageNumbers = () => {
        const pages = [];
        const range = 2; // pages shown on each side of current
        const start = Math.max(1, currentPage - range);
        const end = Math.min(totalPages, currentPage + range);

        for (let i = start; i <= end; i++) {
            pages.push(i);
        }
        return pages;
    };

    if (totalPages <= 1) return null;

    return (
        <div className="pagination">
            <button
                className="page-btn"
                disabled={currentPage === 1}
                onClick={() => onPageChange && onPageChange(currentPage - 1)}
            >
                ← Prev
            </button>

            <div className="page-numbers">
                {getPageNumbers().map((page) => (
                    <button
                        key={page}
                        className={`page-num${page === currentPage ? ' active' : ''}`}
                        onClick={() => onPageChange && onPageChange(page)}
                    >
                        {page}
                    </button>
                ))}
            </div>

            <button
                className="page-btn"
                disabled={currentPage === totalPages}
                onClick={() => onPageChange && onPageChange(currentPage + 1)}
            >
                Next →
            </button>
        </div>
    );
};

export default Pagination;
