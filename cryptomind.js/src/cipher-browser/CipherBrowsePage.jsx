import '../styles/cipher-browse.css';
import BrowsePageHeader from './BrowsePageHeader';
import FilterSidebar    from './FilterSidebar';
import ContentTopbar    from './ContentTopbar';
import CipherCard, { CipherCardSkeleton } from '../components/CipherCard';
import EmptyState       from './EmptyState';
import Pagination       from './Pagination';
import { useState,useEffect, useContext } from 'react';
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
   console.log('Fetching ciphers with filters:', {
      searchTerm: filtersParam.searchTerm ?? searchTerm,
      selectedTags: filtersParam.selectedTags ?? selectedTags,
      challengeType: filtersParam.challengeType ?? challengeType
    });
    const res = await axios.get('http://localhost:5115/api/ciphers/all', {
      withCredentials: true,
      params: {
        SearchTerm: filtersParam.searchTerm ?? searchTerm,
        Tags: filtersParam.selectedTags ?? selectedTags,
        ChallengeType: filtersParam.challengeType ?? challengeType,
        OrderTerm: sortBy
      },
       paramsSerializer: params => {
        // This forces arrays to be serialized as multiple query params
        return qs.stringify(params, { arrayFormat: 'repeat' });
      }
    }).catch(error =>{
       if (error.response?.status === 403) {
          setState({ roles: [], isLoggedIn: false, isBanned: true });
       }
    });

    console.log('API response:', res.data); 
    setCiphers(res.data);
    setTotalPages(res.data.totalPages);
  } catch (err) {
    console.error(err);
    setCiphers([]);
  } finally {
    setIsLoading(false);
  }
};

useEffect(() => {fetchCiphers({searchTerm : null,tags:null, challengeType: null})}, []);

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
                        // Update parent filter state
                        setSearchTerm(filters.searchTerm);
                        setChallengeType(filters.challengeType);
                        setSelectedTags(filters.selectedTags);
                        setCurrentPage(1); // optional: reset page
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
                            // Show skeletons while loading
                            Array.from({ length: SKELETON_COUNT }).map((_, i) => (
                                <CipherCardSkeleton key={i} />
                            ))
                        ) : Array.isArray(ciphers) && ciphers.length > 0  ? (
                            ciphers.map((cipher) => (
                                <CipherCard key={cipher.id} cipher={cipher} />
                            ))
                        ) : null}
                    </div>

                    {isLoading && Array.isArray(ciphers) && ciphers.length > 0 && (
                        <EmptyState onReset={clearAll} />
                    )}

                <Pagination
                currentPage={currentPage}
                totalPages={totalPages}
                onPageChange={setCurrentPage}/>

                </section>
            </main>
        </>
    );
};

export default CipherBrowsePage;
