import axios from "axios";
import { useState } from "react";
import './styles/Ciphers.css';
import { Link } from "react-router-dom";
export default function Ciphers() {
    const [state,setState] = useState({ciphers: [], searchTerm: '', tags: []});
    const onChangeState = (e) => {
        setState({...state,[e.target.name]:e.target.value});
    }
    const handleSearch = (event) => {
        event.preventDefault();
        axios.get('http://localhost:5115/api/ciphers/all', {
          params: {
            searchTerm: state.searchTerm,
            Tags: state.tags
        },
        withCredentials: true
      }).then(response => {
            setState({...state, ciphers: response.data});
        }).catch(error => {
            console.error("Error fetching ciphers:", error);
    })}
    const handleClick = (cipherId) => {

    }
    return (    
      <>
   <div class="filter-container">
    <form class="filter-form" onSubmit={handleSearch}>
      <h2>Filter Ciphers</h2>
      <input type="text" name="searchTerm" placeholder="Search term..." value={state.searchTerm} onChange={onChangeState} />
      <button type="submit">Apply Filter</button>
    </form>
  </div>

  {
    state.ciphers.length === 0 && (
      <h1>No ciphers found</h1>
    )

  }
  <div class="cards-container">
    {state.ciphers.map((cipher) => (
      <div class="cipher-card" key={cipher.id}>
        <h3 class="cipher-title">{cipher.title}</h3>
        <p class="cipher-type">Type: <span>{cipher.cipherTags}</span></p>
        <Link to={`/cipher/${cipher.id}`}>Solve</Link>
      </div>
    ))}

  </div>
</> 
    );
}



