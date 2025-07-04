import { Link } from 'react-router-dom';
import './styles/NavigationBar.css';
export default function NavigationBar(){
  

    return(
      <>
      <div className="navbar-left">
    <Link to="/" className="navbar-logo">Cryptomind</Link>
    <div className="navbar-links">
      <Link to="/">Home</Link>    
    </div>
  </div>
  <div className="navbar-auth">
  <Link to="/login" className="auth-button">Log In</Link>
  <Link to="/register" className="auth-button register">Register</Link>
</div>
       </>
    )
    
}