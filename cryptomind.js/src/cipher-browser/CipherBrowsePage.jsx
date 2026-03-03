import '../styles/cipher-browse.css';
import BrowsePageHeader from './BrowsePageHeader';
import FilterSidebar    from './FilterSidebar';
import ContentTopbar    from './ContentTopbar';
import CipherCard, { CipherCardSkeleton } from '../components/CipherCard';
import EmptyState       from './EmptyState';
import Pagination       from './Pagination';
import { useState, useEffect, useContext } from 'react';
import { AuthorizationContext } from '../App.jsx'; 
import { useSearchParams } from 'react-router-dom';
import axios from 'axios';
import qs from 'qs';

const SKELETON_COUNT = 6;

const CipherBrowsePage = () => {
  const getInitialChallengeType = () => {
    const type = searchParams.get('type');
    if (type === 'experimental') return 1;
    if (type === 'standard') return 0;
    return 0;
  };
  const [searchParams] = useSearchParams();
  const [activeFilters, setactiveFilters] = useState([]);
  const [ciphers, setCiphers] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [challengeType, setChallengeType] = useState(getInitialChallengeType);
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
   
   
  const fetchCiphers = async () => {
    setIsLoading(true);
    const path = import.meta.env.VITE_API_URL
    try {
      const res = await axios.get(`${path}/api/ciphers/all`, {
        withCredentials: true,
        params: {
          SearchTerm:  searchTerm,
          Tags:  selectedTags,
          ChallengeType: challengeType,
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

  useEffect(() => { 
    fetchCiphers(); }
  ,[sortBy]);

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
            fetchCiphers();
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