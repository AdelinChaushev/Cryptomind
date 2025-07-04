import NavigationBar from './Navigation_Bar/NavigationBar.js';
import Login from './Navigation_Bar/Login.js';
import './App.css';
import { Routes, Route } from 'react-router-dom';
import Layout from './Layout.js';
import axios from 'axios';
import Register from './Navigation_Bar/Register.js';
function App() {
  axios.defaults.withCredentials =  true
  return (
    <>
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route index element={<h1>Home Page</h1>} />
      </Route>
    </Routes>
    </>
  );
}

export default App;
