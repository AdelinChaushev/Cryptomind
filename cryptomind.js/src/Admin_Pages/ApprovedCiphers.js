import React, { useState, useEffect } from 'react';
import { Link } from "react-router-dom";
//import './styles/ApprovedCiphers.css'; // Assuming you have a CSS file for styling
import axios from 'axios';
export default function ApprovedCiphers() {
  const [ciphers, setCiphers] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    axios.get('http://localhost:5115/api/cipherAdmin/approvedCiphers', { withCredentials: true })
      .then(res => {
        setCiphers(res.data);
        console.log("Approved Ciphers:", res.data);
      })
      .catch(e => console.log(e))
      .finally(() => setLoading(false));
  }, []);
    const handleDelete = (cipherId) => {
         // Logic to approve the cipher
        
         axios.delete(`http://localhost:5115/api/cipherAdmin/deleteCipher/${cipherId}`, {}, { withCredentials: true })
         .then(res => {
           console.log("Cipher rejected:", res.data);
           setCiphers(ciphers.filter(cipher => cipher.id !== cipherId));
         })
         .catch(err => console.error("Error rejecting cipher:", err));
        }

  if (loading) {
    return <h1>Loading approved ciphers...</h1>;
  }

  return (
       ciphers.map(cipher => {
         // Skip already approved ciphers
         if(!cipher.isImage) {
         
         return (
              
           <div className="cipher-card" key={cipher.id}>
              <h3 className="cipher-title">{cipher.title}</h3>
              <p className="cipher-type">Type: <span>{cipher.cipherTags}</span></p>
              <p className="cipher-decrypted">Encrypted Text: {cipher.cipherText}</p>
                <p className="cipher-points">Decrypted text: {cipher.decryptedText}</p>
              <p className="cipher-points">Points: {cipher.points}</p>
            <Link to={`/alter/${cipher.id}`} className="btn approve-btn">Approve</Link>
              <button className="btn reject-btn" onClick={() => handleDelete(cipher.id)}>Delete</button>
            </div>
            );
          
        }
          if(cipher.isImage) {
         
         return (
              
           <div className="cipher-card" key={cipher.id}>
              <h3 className="cipher-title">{cipher.title}</h3>
              <p className="cipher-type">Type: <span>{cipher.cipherTags}</span></p>
              <img className="cipher-decrypted" src={cipher.cipherText}></img>
              <p className="cipher-points">Decrypted text: {cipher.decryptedText}</p>
              <p className="cipher-points">Points: {cipher.points}</p>
              <Link to={`/alter/${cipher.id}`} className="btn approve-btn">Edit</Link>
              <button className="btn reject-btn" onClick={() => handleDelete(cipher.id)}>Delete</button>
            </div>
            );
          
        }
      })
       
   
 )
}