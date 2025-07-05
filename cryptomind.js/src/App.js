import NavigationBar from './Navigation_Bar/NavigationBar.js';
import Login from './Navigation_Bar/Login.js';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import Layout from './Layout.js';
import axios from 'axios';
import Register from './Navigation_Bar/Register.js';

import { createContext, useEffect,useState } from 'react';
export const AuthroizationContext = createContext({roles : [], isLoggedIn: false});

function App() {
  axios.defaults.withCredentials =  true
  axios.defaults.baseURL = 'http://localhost:5115';
   const [state, setState] = useState({roles : [], isLoggedIn: false});
   useEffect(() => {
    axios.get('/api/User/getUserRoles').then(response => {
      setState({roles: response.data.roles, isLoggedIn: true});
    }).catch(error => {
      console.error("Error fetching login status:", error);
    });
   },[]);

  return (
    <>
    <AuthroizationContext.Provider value={state}>
    <Routes >

      <Route path="/" element={<Layout />}>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route index element={<h1>Home Page</h1>} />
      </Route>
    </Routes>
    </AuthroizationContext.Provider>
    </>
  );
}

export default App;
