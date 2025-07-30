import axios from "axios"
import { useState } from "react"

import './styles/Register.css'
import { useNavigate } from "react-router-dom"

export default function Register(){
    const navigate = useNavigate();
    const [state,setState] = useState({username: '',email:'',password:'',confirmPassword:''})
    const handleChange= (e) => {
      setState({...state,[e.target.name]:e.target.value})
    }
    const handleSubmit = (e) =>{
         if(state.password != state.confirmPassword){
          alert("Password and confirm password must be the same")
          return
         }
         e.preventDefault()
         axios.post('http://localhost:5115/api/User/register',{
          username: state.username,
          email : state.email,
          password: state.password,
          confirmPassword : state.confirmPassword
         }).then(e => 
          {console.log(e.data)
          navigate('/');
         }).catch(e => console.log(e))
         // Reload the page to reflect the new registration
    }
    return(
  <div class="register-container">
   <form class="register-form" onSubmit={handleSubmit} >
    <h2>Register</h2>
    <input type="text" name="username" placeholder="Username" value={state.username} onChange={handleChange} required />
    <input type="email" name="email" placeholder="Email" value={state.email} onChange={handleChange} required />
    <input type="password" name="password" placeholder="Password" value={state.password} onChange={handleChange} required />
    <input type="password" name="confirmPassword" placeholder="Confirm Password" value={state.confirmPassword} onChange={handleChange} required />
    <button type="submit">Register</button>
   </form>
  </div>
    )
}