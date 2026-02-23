import axios from "axios"
import { useState } from "react"

import '../styles/register.css'
import { useNavigate } from "react-router-dom"
import { useContext } from "react";
import { AuthorizationContext } from "../App.jsx";

export default function Register() {
     const navigate = useNavigate();
    const [data,setData] = useState({username: '',email:'',password:'',confirmPassword:''})
    const {state, setState} = useContext(AuthorizationContext);
    const handleChange= (e) => {
      setData({...data,[e.target.name]:e.target.value})
    }
    const handleSubmit = (e) =>{
         if(data.password != data.confirmPassword){
          alert("Password and confirm password must be the same")
          return
         }
         e.preventDefault()
         axios.post('http://localhost:5115/api/auth/register',{
          username: data.username,
          email : data.email,
          password: data.password,
          confirmPassword : data.confirmPassword
         }).then(e => 
          {console.log(e.data)
          navigate('/');
          setState({roles: ["User"], isLoggedIn: true})
         }).catch(e => console.log(e))        // Reload the page to reflect the new registration
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
                        <h2 className="form-title">CREATE ACCOUNT</h2>
                        <p className="form-subtitle">Join the intelligence network</p>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit} noValidate>

                        <div className="form-group">
                            <label htmlFor="username" className="form-label">
                                <span className="label-prefix">01</span>
                                CODENAME
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
                                    id="username"
                                    name="username"
                                    className="form-input"
                                    placeholder="Choose your username"
                                    autoComplete="username"
                                    minLength={3}
                                    maxLength={32}
                                    value={data.username}
                                     onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                            <span className="field-hint">3–32 characters. Letters, numbers, underscores.</span>
                        </div>

                        <div className="form-group">
                            <label htmlFor="email" className="form-label">
                                <span className="label-prefix">02</span>
                                TRANSMISSION ADDRESS
                            </label>
                            <div className="input-wrapper">
                                {/* <span className="input-icon" aria-hidden="true">
                                    <svg viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <rect x="2" y="5" width="16" height="12" rx="2" stroke="currentColor" strokeWidth="1.5" />
                                        <path d="M2 7l8 5 8-5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
                                    </svg>
                                </span> */}
                                <input
                                    type="email"
                                    id="email"
                                    name="email"
                                    className="form-input"
                                    placeholder="your@email.com"
                                    autoComplete="email"
                                    value={data.email} 
                                    onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="password" className="form-label">
                                <span className="label-prefix">03</span>
                                ENCRYPTION KEY
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
                                    placeholder="Create a strong password"
                                    autoComplete="new-password"
                                    minLength={8}
                                    value={data.password}
                                    onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                            <div className="strength-bar" aria-hidden="true">
                                <div className="strength-track">
                                    <div className="strength-fill" id="strength-fill" />
                                </div>
                                <span className="strength-label" id="strength-label">Min. 8 characters</span>
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="confirmPassword" className="form-label">
                                <span className="label-prefix">04</span>
                                CONFIRM KEY
                            </label>
                            <div className="input-wrapper">
                                {/* <span className="input-icon" aria-hidden="true">
                                    <svg viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M4 10l5 5L17 5" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
                                    </svg>
                                </span> */}
                                <input
                                    type="password"
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    className="form-input"
                                    placeholder="Re-enter your password"
                                    autoComplete="new-password"
                                    value={data.confirmPassword}
                                    onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        <div className="form-group terms-group">
                            <label className="checkbox-label">
                                <input type="checkbox" name="agreeTerms" className="checkbox-input" required />
                                <span className="checkbox-custom" aria-hidden="true" />
                                <span className="checkbox-text">
                                    I accept the{' '}
                                    <a href="/terms" className="terms-link">Terms of Service</a>
                                    {' '}and{' '}
                                    <a href="/privacy" className="terms-link">Privacy Policy</a>
                                </span>
                            </label>
                        </div>

                        <button type="submit" className="btn-submit">
                            <span className="btn-text">INITIALIZE ACCOUNT</span>
                            <span className="btn-arrow" aria-hidden="true">→</span>
                            <span className="btn-glow" aria-hidden="true" />
                        </button>

                    </form>

                    <footer className="auth-card-footer">
                        <p className="footer-text">
                            Already an operative?{' '}
                            <a href="/login" className="footer-link">Sign in here</a>
                        </p>
                    </footer>

                </div>
            </main>
        </>
    );
}
