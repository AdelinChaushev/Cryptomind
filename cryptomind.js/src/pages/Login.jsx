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
            navigate('/banned');
          }, 0); 
    } ).catch(error => {  
    if (error.response?.status === 403) {
        setState({ isLoggedIn: false, roles: [], isBanned: true, bannedMessage: error.response?.data?.error });
        navigate("/banned");
        return;
    }
    setState({ isLoggedIn: false, roles: [] });
    setError(error.response?.data?.title || 'Влизането е неуспешно. Моля, проверете данните си и опитайте отново.');
});
    
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
                        <p className="auth-tagline">ПЛАТФОРМА ЗА КЛАСИЧЕСКИ ШИФРИ С ИЗКУСТВЕН ИНТЕЛЕКТ</p>
                        <div className="cipher-divider" aria-hidden="true">
                            <span className="divider-line" />
                            <span className="divider-icon">⊕</span>
                            <span className="divider-line" />
                        </div>
                    </header>

                    <div className="form-title-block">
                        <h2 className="form-title">УДОСТОВЕРЯВАНЕ</h2>
                        <p className="form-subtitle">Достъп до вашето табло за шифри</p>
                    </div>

                   <form className="auth-form" onSubmit={handleSubmit} noValidate>

                        <div className="form-group">
                            <label htmlFor="identifier" className="form-label">
                                <span className="label-prefix">01</span>
                                ИДЕНТИФИКАТОР
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="text"
                                    id="identifier"
                                    name="email"
                                    className="form-input"
                                    placeholder="Имейл адрес"
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
                                ТАЕН КЛЮЧ
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="password"
                                    id="password"
                                    name="password"
                                    className="form-input"
                                    placeholder="Въведете вашата парола"
                                    autoComplete="current-password"
                                    value={data.password} 
                                    onChange={onChangeState}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        <button type="submit" className="btn-submit">
                            <span className="btn-text">ДЕШИФРИРАЙ &amp; ВЛЕЗ</span>
                            <span className="btn-arrow" aria-hidden="true">→</span>
                            <span className="btn-glow" aria-hidden="true" />
                        </button>

                    </form>

                    <footer className="auth-card-footer">
                        <p className="footer-text">
                            Нямате акаунт?
                            <Link to="/register" className="footer-link">Създайте един сега</Link>
                        </p>
                    </footer>

                </div>
            </main>
        </>
    );
}