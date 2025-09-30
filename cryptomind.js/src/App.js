import Login from './Navigation_Bar/Login.js';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import Layout from './Layout.js';
import axios from 'axios';
import Register from './Navigation_Bar/Register.js';

import { createContext, useEffect,useState } from 'react';
import RequireAuth from './RequireAuth.js';
import Ciphers from './Cipher_Pages/Ciphers.js';
import CipherDetails from './Cipher_Pages/CipherDetails.js';
import SubmitCipher from './Cipher_Pages/SubmitCipher.js';
import GetPendingCiphers from './Admin_Pages/GetPendingCiphers.js';
import AlterForm from './Admin_Pages/AlterForm.js';
import ApprovedCiphers from './Admin_Pages/ApprovedCiphers.js';
export const AuthorizationContext = createContext({roles : [], isLoggedIn: false});

function App() {
  axios.defaults.withCredentials =  true
  axios.defaults.baseURL = 'http://localhost:5115';
   const [state, setState] = useState({roles : [], isLoggedIn: false});
   const[loading, setLoading] = useState(true);
   useEffect(() => {
    axios.get('/api/User/getUserRoles').then(response => {
      setState({roles: response.data.roles, isLoggedIn: true});
    }).catch(error => {
      setState({roles: [], isLoggedIn: false});
    }).finally(() => {setLoading(false);});
   },[]);
    if(loading) {
      return <h1>Loading...</h1>
    }
  return (
    <>
    <AuthorizationContext.Provider value={state}>
    <Routes >

      <Route path="/" element={<Layout />}>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="ciphers" element={
          <RequireAuth>
           <Ciphers/>
          </RequireAuth>} />
        <Route path="cipher/:id" element={<RequireAuth >
          <CipherDetails/>
        </RequireAuth>} />
        <Route path='submit-cipher' element={<RequireAuth>
          <SubmitCipher/>
        </RequireAuth>} />
        <Route path="admin" element={
          <RequireAuth roles={['Admin']}>
          < GetPendingCiphers />
          </RequireAuth>} />
       
        <Route path="alter/:id" element={
          <RequireAuth roles={['Admin']}>
          < AlterForm />
          </RequireAuth>} />
       
           <Route path="approvedCiphers" element={
          <RequireAuth roles={['Admin']}>
          < ApprovedCiphers />
          </RequireAuth>} />
        <Route index element={<h1>Home Page</h1>} />
      </Route>
    </Routes>
    </AuthorizationContext.Provider>
    </>
  );
}

export default App;
