import '../styles/cipher-browse.css';
import BrowsePageHeader from './BrowsePageHeader';
import FilterSidebar    from './FilterSidebar';
import DailyChallengeTeaser from '../daily-challenge/DailyChallengeTeaser';
import ContentTopbar    from './ContentTopbar';
import CipherCard, { CipherCardSkeleton } from '../components/CipherCard';
import EmptyState       from './EmptyState';
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
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const toggleTag = (tagId) => {
    setSelectedTags(prev =>
      prev.includes(tagId) ? prev.filter(t => t !== tagId) : [...prev, tagId]
    );
  };

  const fetchCiphers = async (overrides = {}) => {
    setIsLoading(true);
    const path = import.meta.env.VITE_API_URL;
    try {
      const res = await axios.get(`${path}/api/ciphers/all`, {
        withCredentials: true,
        params: {
          SearchTerm:    overrides.searchTerm    !== undefined ? overrides.searchTerm    : searchTerm,
          Tags:          overrides.selectedTags  !== undefined ? overrides.selectedTags  : selectedTags,
          ChallengeType: overrides.challengeType !== undefined ? overrides.challengeType : challengeType,
          OrderTerm:     overrides.sortBy        !== undefined ? overrides.sortBy        : sortBy,
        },
        paramsSerializer: params => qs.stringify(params, { arrayFormat: 'repeat' })
      });
      setCiphers(res.data);
    } catch (err) {
      setCiphers([]);
      if (err.response?.status === 403) {
        setState({ roles: [], isLoggedIn: false, isBanned: true });
      }
    } finally {
      setIsLoading(false);
    }
  };

  const clearAll = () => {
    setSearchTerm('');
    setChallengeType(0);
    setSelectedTags([]);
    fetchCiphers({ searchTerm: '', challengeType: 0, selectedTags: [] });
  };

  useEffect(() => { fetchCiphers(); }, [sortBy]);

  return (
    <>
      <BrowsePageHeader
        totalCiphers={totalCiphers}
        activeSolvers={activeSolvers}
      />

      <main className="browse-layout">
        {isSidebarOpen && (
          <button
            className="sidebar-close-btn"
            onClick={() => setIsSidebarOpen(false)}
          >
            ФИЛТРИ
            <svg width="16" height="16" viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M3 3l10 10M13 3L3 13"/>
            </svg>
          </button>
        )}

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
          <DailyChallengeTeaser />
          <ContentTopbar
            isOpen={isSidebarOpen}
            resultsCount={Array.isArray(ciphers) ? ciphers.length : 0}
            activeFilters={activeFilters}
            sortBy={sortBy}
            onSortChange={setSortBy}
            activeFilterCount={activeFilters.length}
            onMobileFilterToggle={() => setIsSidebarOpen(prev => !prev)}
            isSidebarOpen={isSidebarOpen}
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
            ) : (
              !isLoading && <EmptyState onReset={clearAll} />
            )}
          </div>
        </section>
      </main>
    </>
  );
};

export default CipherBrowsePage;