import NavigationBar from './Navigation_Bar/NavigationBar.js';
import Login from './Navigation_Bar/Login.js';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import Layout from './Layout.js';
import axios from 'axios';
import Register from './Navigation_Bar/Register.js';

import { createContext, useEffect,useState } from 'react';
import RequireAuth from './RequireAuth.js';
import Ciphers from './Cipher_Pages/Ciphers.js';
export const AuthorizationContext = createContext({roles : [], isLoggedIn: false});

function App() {
  axios.defaults.withCredentials =  true
  axios.defaults.baseURL = 'http://localhost:5115';
   const [state, setState] = useState({roles : [], isLoggedIn: false});
   useEffect(() => {
    axios.get('/api/User/getUserRoles').then(response => {
      setState({roles: response.data.roles, isLoggedIn: true});
    }).catch(error => {
      setState({roles: [], isLoggedIn: false});
    });
   },[]);

  return (
    <>
    <AuthorizationContext.Provider value={state}>
    <Routes >

      <Route path="/" element={<Layout />}>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="ciphers" element={<RequireAuth allowedRoles={[]}>
          <Ciphers/>
        </RequireAuth>} />
        <Route path="ciphers/:cipherId" element={<h1>Cipher Details Page</h1>} />
        <Route index element={<h1>Home Page</h1>} />
      </Route>
    </Routes>
    </AuthorizationContext.Provider>
    </>
  );
}

export default App;
