import React from 'react';
import { createContext, useEffect,useState,useContext } from 'react';
import Home from './pages/Home';
import './styles/home.css';
import Login from './pages/Login.jsx';
import { Routes, Route } from 'react-router-dom';
import Layout from './Layout.jsx';
import axios from 'axios';
import Register from './pages/Register.jsx';
import RequireAuth from './RequireAuth.jsx';
import CipherBrowsePage from './cipher-browser/CipherBrowsePage.jsx';
import SubmitCipherPage from './submit-cipher/SubmitCipherPage.jsx';
import CipherSolvePage from './cipher-solve/pages/CipherSolvePage.jsx';
import MySubmissionsPage from './my-submissions/pages/MySubmissionsPage.jsx';
import AdminDashboard from './admin-pages/AdminDashboard.jsx';
import ManageApprovedCiphers from './admin-pages/ManageApprovedCiphers.jsx';
import PendingAnswers from './admin-pages/PendingAnswers.jsx';
import PendingCiphers from './admin-pages/PendingCiphers.jsx';
import CipherReview from './admin-pages/CipherReview.jsx';
import PendingAnswerApproval from './admin-pages/PendingAnswerApproval.jsx';
import UsersManagement from './admin-pages/UsersManagement.jsx';
import DeletedCiphers from './admin-pages/DeletedCiphers.jsx';
import Leaderboard from './pages/Leaderboard.jsx';
import CipherTool from './cipher-tool/CipherTool.jsx';
import CipherLibrary from './cipher-tool/CipherLibrary.jsx';
import About from './pages/About.jsx';
import NotFoundPage from './pages/NotFoundPage.jsx';
import BannedPage from './pages/BannedPage.jsx';
import RequireNotBanned from './RequiresNotBanned.jsx';
import NotificationsPage from './notifications/NotificationsPage.jsx';
import { useNotifications } from './notifications/UseNotifications';
import ToastContainer from './notifications/ToastContainer';
import { ErrorProvider } from "./ErrorContext";

import AccountInfo from './account-page/AccountInfo.jsx';
export const AuthorizationContext = createContext({roles : [], isLoggedIn: false});
export const NotificationContext = createContext(null);
export const useNotificationContext = () => useContext(NotificationContext);
function App() {
   const notifications = useNotifications();
   axios.defaults.withCredentials =  true
   axios.defaults.baseURL = 'http://localhost:5115';
   const [state, setState] = useState({roles : [], isLoggedIn: false, isBanned : false});
   const[loading, setLoading] = useState(true);

    useEffect(() => {
        if (state.isLoggedIn) {
            notifications.refetch();
        }
    }, [state.isLoggedIn]);
  
   useEffect(() => {
    axios.get('http://localhost:5115/api/user/get-roles').then(response => {
      setState({roles: response.data, isLoggedIn: true});
    }).catch(error => {
      console.log("Error fetching user roles:", error.response?.status);
     if (error.response?.status === 403) {
    setState({ roles: [], isLoggedIn: false, isBanned: true });
  } else {
    setState({ roles: [], isLoggedIn: false, isBanned: false });
  }
    }).finally(() => {setLoading(false);});
   },[]);
   
    if(loading) {
      return <h1></h1>

    }
    return (    
    <>
    <AuthorizationContext.Provider value={{ state, setState }}>
    <ErrorProvider>
    <NotificationContext.Provider value={notifications}>
    <Routes >
      <Route path="/banned" element={<RequireNotBanned><BannedPage/></RequireNotBanned>} />
      
      <Route path="/" element={<RequireNotBanned mustNotBeBanned={true}><Layout /></RequireNotBanned>}>
        <Route path="login" element={<RequireAuth mustNotBeLogged={true} ><Login /></RequireAuth>} />
        <Route path="register" element={<RequireAuth mustNotBeLogged={true}><Register /></RequireAuth>} />    
        <Route path="leaderboard" element={<Leaderboard />} /> 
        <Route path="cipher-tool" element={<CipherTool />} /> 
        <Route path="about" element={<About />} />
        <Route path="cipher-library" element={<CipherLibrary />} /> 
        <Route path="browse" element={<RequireAuth> <CipherBrowsePage /> </RequireAuth>} />   
        <Route path="account" element={<RequireAuth> <AccountInfo /> </RequireAuth>} />   
        <Route path="notifications" element={<RequireAuth> <NotificationsPage /> </RequireAuth>} /> 
        <Route path="submit" element={<RequireAuth><SubmitCipherPage /></RequireAuth>} />  
        <Route path="cipher/:id" element={<RequireAuth><CipherSolvePage /></RequireAuth>} />
        <Route path="my_submissions" element={<RequireAuth><MySubmissionsPage /></RequireAuth>} />
        <Route path="admin" element={<RequireAuth allowedRoles={["Admin"]}><AdminDashboard /></RequireAuth>} />
        <Route path="admin/pending-ciphers" element={<RequireAuth allowedRoles={["Admin"]}><PendingCiphers /></RequireAuth>} />
        <Route path="admin/cipher-review/:id" element={<RequireAuth allowedRoles={["Admin"]} ><CipherReview /></RequireAuth>} />
        <Route path="admin/manage-ciphers" element={<RequireAuth allowedRoles={["Admin"]} ><ManageApprovedCiphers /></RequireAuth>} />
        <Route path="admin/pending-answers" element={<RequireAuth allowedRoles={["Admin"]} ><PendingAnswers /></RequireAuth>} />
        <Route path="admin/answer-review/:id" element={<RequireAuth allowedRoles={["Admin"]} ><PendingAnswerApproval /></RequireAuth>} />
        <Route path="admin/users" element={<RequireAuth allowedRoles="Admin" ><UsersManagement /></RequireAuth>} />
        <Route path="admin/deleted-ciphers" element={<RequireAuth allowedRoles="Admin" ><DeletedCiphers /></RequireAuth>} />
        {!state.isLoggedIn ?(<Route index element={<Home/>} />) : (<Route index element={<RequireAuth ><CipherBrowsePage/></RequireAuth>}  />)}  
        <Route path="*" element={<NotFoundPage />} />
        
      </Route>
    </Routes>
    <ToastContainer
            toasts={notifications.toasts}
            onDismiss={notifications.dismissToast}
            onNotificationClick={notifications.handleNotificationClick}
        />
    </NotificationContext.Provider>
    </ErrorProvider>  
    </AuthorizationContext.Provider>
    </>
  
    );
}

export default App;
