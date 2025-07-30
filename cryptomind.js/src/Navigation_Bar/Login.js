import React, { useState } from 'react';
import axios from 'axios';
import './styles/Login.css';
import { useNavigate } from 'react-router-dom';
export default function Login () {
  const navigate = useNavigate();
    const [state,setState] = useState({email: '', password:''})
   const  onChangeState = (e) =>  {
       setState({...state,[e.target.name]:e.target.value})
   }
   const handleSubmit = (e) => {
    e.preventDefault()
    console.log("IN submit")
    axios.post('http://localhost:5115/api/User/login',{
        email: state.email,
        password: state.password,


    }).then(res => {
      console.log(res.data)
      navigate('/', { replace: true }); // Redirect to ciphers page after successful login
    } ).catch(e => console.log(e));   
    
   }

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit}>
        <h2>Log In</h2>
        <input type="email" name="email" placeholder="Email" value={state.email} onChange={onChangeState} required />
        <input type="password" name="password" placeholder="Password" value={state.password} onChange={onChangeState} required />

        <button type="submit"    >Login</button>

      </form>
    </div>
  );
};

