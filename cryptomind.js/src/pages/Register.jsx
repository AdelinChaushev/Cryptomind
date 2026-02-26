import axios from "axios"
import { useState } from "react"

import '../styles/register.css'
import { useNavigate } from "react-router-dom"
import { useContext } from "react";
import { AuthorizationContext } from "../App.jsx";
import { useError } from '../ErrorContext.jsx';
const errorTranslations = {
    "The Email field is not a valid e-mail address.": "Имейлът не е валиден.",
    "The Password field is required.": "Паролата е задължителна.",
    "The field Password must be a string or array type with a minimum length of '8'.": "Паролата трябва да е поне 8 символа.",
    "The Username field is required.": "Потребителското име е задължително.",
    "The field ConfirmPassword must be a string or array type with a minimum length of '8'.": "Потвърждението на паролата трябва да е поне 8 символа.",
    "User with this email already exists": "Потребител с този имейл вече съществува.",
    "Keep the username constraints": "Потребителското име трябва да е между 3 и 32 символа.",
    "Keep the password constraints": "Паролата трябва да е поне 8 символа.",
    "User creation failed": "Създаването на акаунт е неуспешно.",
};
export default function Register() {
     const navigate = useNavigate();
    const [data,setData] = useState({username: '',email:'',password:'',confirmPassword:''})
    const {state, setState} = useContext(AuthorizationContext);
    const { setError } = useError();
    const handleChange= (e) => {
      setData({...data,[e.target.name]:e.target.value})
    }
    const handleSubmit = (e) =>{
        e.preventDefault();
         if(data.password != data.confirmPassword){
          setError("Паролата и потвърждението на паролата трябва да съвпадат")
          return;
         }
         
         axios.post('http://localhost:5115/api/auth/register',{
          username: data.username,
          email : data.email,
          password: data.password,
          confirmPassword : data.confirmPassword
         }).then(e => 
          {console.log(e.data)
          navigate('/');
          setState({roles: ["User"], isLoggedIn: true})
         }).catch(e => {
    const data = e.response?.data;

    if (data?.errors) {
        const raw = Object.values(data.errors)[0][0];
        setError(errorTranslations[raw] || raw);
        return;
    }

    const raw =
        (typeof data === 'string' ? data : null) ||
        data?.error ||
        'Регистрацията е неуспешна. Моля, проверете въведените данни.';
    setError(errorTranslations[raw] || raw);
})// Reload the page to reflect the new registration
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
                        <h2 className="form-title">СЪЗДАВАНЕ НА АКАУНТ</h2>
                        <p className="form-subtitle">Присъединете се към криптографската мрежа</p>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit} noValidate>

                        <div className="form-group">
                            <label htmlFor="username" className="form-label">
                                <span className="label-prefix">01</span>
                                ПСЕВДОНИМ
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="text"
                                    id="username"
                                    name="username"
                                    className="form-input"
                                    placeholder="Изберете потребителско име"
                                    autoComplete="username"
                                    minLength={3}
                                    maxLength={32}
                                    value={data.username}
                                    onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                            <span className="field-hint">3–16 символа.</span>
                        </div>

                        <div className="form-group">
                            <label htmlFor="email" className="form-label">
                                <span className="label-prefix">02</span>
                                АДРЕС ЗА ВРЪЗКА
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="email"
                                    id="email"
                                    name="email"
                                    className="form-input"
                                    placeholder="вашият@имейл.com"
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
                                КЛЮЧ ЗА КРИПТИРАНЕ
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="password"
                                    id="password"
                                    name="password"
                                    className="form-input"
                                    placeholder="Създайте силна парола"
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
                                <span className="strength-label" id="strength-label">Мин. 8 символа</span>
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="confirmPassword" className="form-label">
                                <span className="label-prefix">04</span>
                                ПОТВЪРДИ КЛЮЧА
                            </label>
                            <div className="input-wrapper">
                                <input
                                    type="password"
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    className="form-input"
                                    placeholder="Въведете паролата отново"
                                    autoComplete="new-password"
                                    value={data.confirmPassword}
                                    onChange={handleChange}
                                    required
                                />
                                <span className="input-focus-bar" aria-hidden="true" />
                            </div>
                        </div>

                        <button type="submit" className="btn-submit">
                            <span className="btn-text">СЪЗДАЙ АКАУНТ</span>
                            <span className="btn-arrow" aria-hidden="true">→</span>
                            <span className="btn-glow" aria-hidden="true" />
                        </button>

                    </form>

                    <footer className="auth-card-footer">
                        <p className="footer-text">
                            Вече имате акаунт?{' '}
                            <a href="/login" className="footer-link">Влезте тук</a>
                        </p>
                    </footer>

                </div>
            </main>
        </>
    );
}