import React, { useEffect,useState } from 'react';
import SubmissionsPageHeader from './components/SubmissionsPageHeader';
import SubmissionsStatsStrip from './components/SubmissionsStatsStrip';
import SubmissionsTabs from './components/SubmissionsTabs';
import CipherSubmissionsList from './components/CipherSubmissionsList';
import AnswerSuggestionsList from './components/AnswerSuggestionsList';
import '../styles/my-submissions.css';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useLocation } from 'react-router-dom';
const MySubmissionsPage = () => {
    const navigate = useNavigate();
    const [submissions,setSubmissions ] = useState({ciphers : [], suggestions : []});
    useEffect(() => {
    
    axios.get(`${import.meta.env.VITE_API_URL}/api/submissions`,{withCredentials : true})
    .then(response => {
        setSubmissions({
            ciphers : response.data.ciphers,
            suggestions : response.data.answers
        });
        console.log('Fetched submissions:', response.data);
    })
    .catch(error => {
        console.error('Error fetching submissions:', error);
    });
}, []);
const location = useLocation();

     const getInitialTab = () => {
    const params = new URLSearchParams(location.search);
    const tab = params.get('tab');
    return tab === 'answers' ? 'answers' : 'ciphers';
    };
    const [activeTab,setActiveTab] = useState(getInitialTab); 

    const cipherSubmissions = submissions.ciphers;
    const answerSuggestions = submissions.suggestions;

    const stats = {
        totalSubmissions: cipherSubmissions.length  + answerSuggestions.length,
        approved: cipherSubmissions.filter(s => s.status === 'Approved').length + answerSuggestions.filter(s => s.status === 'Approved').length,
        pending:  cipherSubmissions.filter(s => s.status === 'Pending').length + answerSuggestions.filter(s => s.status === 'Pending').length,
        rejected: cipherSubmissions.filter(s => s.status === 'Rejected').length + answerSuggestions.filter(s => s.status === 'Rejected').length,
        deleted : cipherSubmissions.filter(c => c.status === 'CipherDeleted').length + answerSuggestions.filter(s => s.status === 'CipherDeleted').length
    };

    const handleViewCipher  = (id) => { navigate(`/cipher/${id}`); };

    const handleTabChange   = (tab) => { setActiveTab(tab); };

    return (
        <>
            <main className="page-wrapper">
                <SubmissionsPageHeader />
                <SubmissionsStatsStrip
                    totalSubmissions={stats.totalSubmissions}
                    approved={stats.approved}
                    pending={stats.pending}
                    rejected={stats.rejected}
                    deleted={stats.deleted}
                />
                <SubmissionsTabs
                    activeTab={activeTab}
                    onTabChange={handleTabChange}
                    cipherCount={cipherSubmissions ? cipherSubmissions.length : 0}
                    answerCount={answerSuggestions? answerSuggestions.length : 0}
                />
             
                <div className={`tab-content ${activeTab === 'ciphers' ? 'active' : ''}`}>
                    <CipherSubmissionsList
                        submissions={cipherSubmissions}
                        onViewCipher={handleViewCipher}

                    />
                </div>
               
                <div className={`tab-content ${activeTab === 'answers' ? 'active' : ''}`}>
                    <AnswerSuggestionsList
                        answers={answerSuggestions}
                        onViewCipher={handleViewCipher}
                    />
                </div>
            </main>
        </>
    );
};

export default MySubmissionsPage;

