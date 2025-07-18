import  { useState, useEffect, use } from 'react';
import './styles/GetPendingCiphers.css';
import { Link } from "react-router-dom";
import axios from 'axios';

export default function GetPendingCiphers() {
   const[ciphers, setCiphers] = useState([]);
   const[loading, setLoading] = useState(true);
   
       useEffect(() => {
       axios.get('http://localhost:5115/api/cipherAdmin/pendingCiphers', { withCredentials: true })
       .then(res => {
              setCiphers(res.data);
              console.log("Pending Ciphers:", res.data);
             
       }).catch(e => console.log(e) ).finally(e => setLoading(false))},[])
           
       const handleReject = (cipherId) => {
         // Logic to approve the cipher
        
         axios.delete(`http://localhost:5115/api/cipherAdmin/rejectCipher/${cipherId}`, {}, { withCredentials: true })
         .then(res => {
           console.log("Cipher rejected:", res.data);
           setCiphers(ciphers.filter(cipher => cipher.id !== cipherId));
         })
         .catch(err => console.error("Error rejecting cipher:", err));
        }
 
        if (loading) {
        return <h1>Loading pending ciphers...</h1>;} 
    return (
       ciphers.map(cipher => {
         const textCipher = ciphers.filter(c => c.$type === 'Cryptomind.Data.Entities.TextCipher, Cryptomind.Data');
         console.log("Text Ciphers:", textCipher);
         if(!cipher.isImage) {
         
         return (
              
           <div className="cipher-card" key={cipher.id}>
              <h3 className="cipher-title">{cipher.title}</h3>
              <p className="cipher-type">Type: <span>{cipher.cipherTags}</span></p>
              <p className="cipher-decrypted">Encrypted Text: {cipher.cipherText}</p>
                <p className="cipher-points">Decrypted text: {cipher.decryptedText}</p>
              <p className="cipher-points">Points: {cipher.points}</p>
            <Link to={`/approve/${cipher.id}`} className="btn approve-btn">Approve</Link>
              <button className="btn reject-btn" onClick={() => handleReject(cipher.id)}>Reject</button>
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
              <Link to={`/approve/${cipher.id}`} className="btn approve-btn">Approve</Link>
              <button className="btn reject-btn" onClick={() => handleReject(cipher.id)}>Reject</button>
            </div>
            );
          
        }
      })
       
   
 )
}