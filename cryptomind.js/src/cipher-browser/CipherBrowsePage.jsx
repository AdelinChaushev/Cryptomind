import '../styles/cipher-browse.css';
import BrowsePageHeader from './BrowsePageHeader';
import FilterSidebar    from './FilterSidebar';
import ContentTopbar    from './ContentTopbar';
import CipherCard, { CipherCardSkeleton } from '../components/CipherCard';
import EmptyState       from './EmptyState';
import Pagination       from './Pagination';
import { useState, useEffect, useContext } from 'react';
import { AuthorizationContext } from '../App.jsx'; 
import axios from 'axios';
import qs from 'qs';

const SKELETON_COUNT = 6;

const CipherBrowsePage = () => {
  const [activeFilters, setactiveFilters] = useState([]);
  const [ciphers, setCiphers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [challengeType, setChallengeType] = useState(0);
  const [totalCiphers, setTotalCiphers] = useState(0);
  const [activeSolvers, setActiveSolvers] = useState(0);
  const { state, setState } = useContext(AuthorizationContext);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedTags, setSelectedTags] = useState([]);
  const [sortBy, setSortBy] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const toggleTag = (tagId) => {
    setSelectedTags(prev =>
      prev.includes(tagId)
        ? prev.filter(t => t !== tagId)
        : [...prev, tagId]
    );
  };

  const clearAll = () => {
    setSearchTerm('');
    setChallengeType(0);
    setSelectedTags([]);
  };

  const fetchCiphers = async (filtersParam) => {
    setIsLoading(true);

    try {
      const res = await axios.get('http://localhost:5115/api/ciphers/all', {
        withCredentials: true,
        params: {
          SearchTerm: filtersParam.searchTerm ?? searchTerm,
          Tags: filtersParam.selectedTags ?? selectedTags,
          ChallengeType: filtersParam.challengeType ?? challengeType,
          OrderTerm: sortBy
        },
        paramsSerializer: params => qs.stringify(params, { arrayFormat: 'repeat' })
      });

      setCiphers(res.data);
      setTotalPages(res.data.totalPages);
    } catch (err) {
      setCiphers([]);
      if (err.response?.status === 403) {
        setState({ roles: [], isLoggedIn: false, isBanned: true });
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => { fetchCiphers({ searchTerm: null, tags: null, challengeType: null }); }, []);

  return (
    <>
      <BrowsePageHeader
        totalCiphers={totalCiphers}
        activeSolvers={activeSolvers}
      />

      <main className="browse-layout">
        <FilterSidebar
          isOpen={isSidebarOpen}
          searchTerm={searchTerm}
          selectedTags={selectedTags}
          challengeType={challengeType}
          onChallengeTypeChange={setChallengeType}
          onSearchChange={setSearchTerm}
          onTagChange={toggleTag}
          onClearAll={clearAll}
          onApplyFilters={(filters) => {
            setSearchTerm(filters.searchTerm);
            setChallengeType(filters.challengeType);
            setSelectedTags(filters.selectedTags);
            setCurrentPage(1);
            fetchCiphers(filters);
          }}
        />

        <section className="cipher-content">
          <ContentTopbar
            resultsCount={Array.isArray(ciphers) ? ciphers.length : 0}
            activeFilters={activeFilters}
            sortBy={sortBy}
            onSortChange={setSortBy}
            activeFilterCount={activeFilters.length}
            onMobileFilterToggle={() => setIsSidebarOpen(prev => !prev)}
          />

          <div className="cipher-grid">
            {isLoading ? (
              Array.from({ length: SKELETON_COUNT }).map((_, i) => (
                <CipherCardSkeleton key={i} />
              ))
            ) : Array.isArray(ciphers) && ciphers.length > 0 ? (
              ciphers.map((cipher) => (
                <CipherCard key={cipher.id} cipher={cipher} />
              ))
            ) : null}
          </div>

          {!isLoading && Array.isArray(ciphers) && ciphers.length === 0 && (
            <EmptyState onReset={clearAll} />
          )}

          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
          />
        </section>
      </main>
    </>
  );
};

export default CipherBrowsePage;