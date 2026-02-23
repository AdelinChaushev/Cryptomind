import { Link } from 'react-router-dom';
import { useState } from 'react';
import { useNavigate} from 'react-router-dom';
import axios from 'axios';
import '../styles/login.css';
import { AuthorizationContext } from '../App.jsx'; 
import { useContext } from 'react';

import { useError } from '../ErrorContext.jsx';

export default function Login() {
    const navigate = useNavigate();
    const [data,setData] = useState({email: '', password:''})
    const {state, setState} = useContext(AuthorizationContext);
    const { setError } = useError();
   const  onChangeState = (e) =>  {
       setData({...data,[e.target.name]:e.target.value})
   }
   const handleSubmit = (e) => {
    e.preventDefault()
    console.log("IN submit")
    axios.post('http://localhost:5115/api/auth/login',{
        email: data.email,
        password: data.password,
    }).then(res => {
      setState({isLoggedIn: true,roles: res.data});
     setTimeout(() => {
            navigate('/');
          }, 0); 
    } ).catch(e =>{ 
        if(e.response.status == 403) {
        setState({isLoggedIn: false, roles: [], isBanned: true})
        
        }
         setState({isLoggedIn: false, roles: []})
         setError(e.response?.data?.title || 'Login failed. Please check your credentials and try again.');
         }
    );   
    
   }
    return (
        <>
            <div className="bg-grid" aria-hidden="true" />
            <div className="bg-vignette" aria-hidden="true" />

            <main className="auth-main">
                <div className="auth-card">

                    <div className="card-top-bar" aria-hidden="true" />

                    <header className="auth-header">
                        <a href="/" className="logo-link">
                            <div className="logo">
                                <svg
                                    className="logo-icon"
                                    viewBox="0 0 40 40"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                    aria-hidden="true"
                                >
                                    <rect x="10" y="18" width="20" height="16" rx="2" stroke="currentColor" strokeWidth="2" />
                                    <path d="M14 18V13a6 6 0 0 1 12 0v5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                    <circle cx="20" cy="26" r="2.5" fill="currentColor" />
                                    <line x1="20" y1="28.5" x2="20" y2="31" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                                </svg>
                                <h1 className="logo-text">CRYPTO<span>MIND</span></h1>
                            </div>
                        </a>
                        <p className="auth-tagline">CLASSICAL CIPHER INTELLIGENCE PLATFORM</p>
                        <div className="cipher-divider" aria-hidden="true">
                            <span className="divider-line" />
                            <span className="divider-icon">⊕</span>
                            <span className="divider-line" />
                        </div>
                    </header>

                    <div className="form-title-block">
                        <h2 className="form-title">AUTHENTICATE</h2>
                        <p className="form-subtitle">Access your cipher dashboard</p>
                    </div>

                   <form className="auth-form" onSubmit={handleSubmit} noValidate>

                        <div className="form-group">
                            <label htmlFor="identifier" className="form-label">
                                <span className="label-prefix">01</span>
                                IDENTIFIER
                            </label>
                            <div className="input-wrapper">
                                {/* <span className="input-icon" aria-hidden="true">
                                    <svg viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <circle cx="10" cy="7" r="4" stroke="currentColor" strokeWidth="1.5" />
                                        <path d="M3 17c0-3.866 3.134-7 7-7s7 3.134 7 7" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
                                    </svg>
                                </span> */}
                                <input
                                    type="text"
                                    id="identifier"
                                    name="email"
                                    className="form-input"
                                    placeholder="Username or email address"
                                    autoComplete="username"
                                    value={data.email} 
                                    onChange={onChangeState}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="password" className="form-label">
                                <span className="label-prefix">02</span>
                                SECRET KEY
                            </label>
                            <div className="input-wrapper">
                                {/* <span className="input-icon" aria-hidden="true">
                                    <svg viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M5 10V7a5 5 0 0 1 10 0v3" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" />
                                        <rect x="3" y="10" width="14" height="9" rx="2" stroke="currentColor" strokeWidth="1.5" />
                                        <circle cx="10" cy="14.5" r="1.5" fill="currentColor" />
                                    </svg>
                                </span> */}
                                <input
                                    type="password"
                                    id="password"
                                    name="password"
                                    className="form-input"
                                    placeholder="Enter your password"
                                    autoComplete="current-password"
                                    value={data.password} 
                                    onChange={onChangeState}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        {/* <div className="form-options">
                            <label className="checkbox-label">
                                <input type="checkbox" name="rememberMe" className="checkbox-input" />
                                <span className="checkbox-custom" aria-hidden="true" />
                                <span className="checkbox-text">Remember session</span>
                            </label>
                            <a href="/forgot-password" className="forgot-link">Forgot key?</a>
                        </div> */}

                        <button type="submit" className="btn-submit">
                            <span className="btn-text">DECRYPT &amp; ENTER</span>
                            <span className="btn-arrow" aria-hidden="true">→</span>
                            <span className="btn-glow" aria-hidden="true" />
                        </button>

                    </form>

                    <footer className="auth-card-footer">
                        <p className="footer-text">
                            No account yet?
                            <Link to="/register" className="footer-link">Create one now</Link>
                        </p>
                    </footer>

                </div>
            </main>
        </>
    );
}
