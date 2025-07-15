import React, { useState, useEffect, use } from 'react';
import './styles/GetPendingCiphers.css'; // Assuming you have a CSS file for styling
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
              <p className="cipher-points">Points: {cipher.points}</p>
              <button className="btn approve-btn">Approve</button>
              <button className="btn reject-btn">Reject</button>
            </div>
            );
          
        }
          if(cipher.isImage) {
         
         return (
              
           <div className="cipher-card" key={cipher.id}>
              <h3 className="cipher-title">{cipher.title}</h3>
              <p className="cipher-type">Type: <span>{cipher.cipherTags}</span></p>
              <img className="cipher-decrypted" src={cipher.cipherText}></img>
              <p className="cipher-points">Points: {cipher.points}</p>
              <button className="btn approve-btn">Approve</button>
              <button className="btn reject-btn">Reject</button>
            </div>
            );
          
        }
      })
       
   
 )
}