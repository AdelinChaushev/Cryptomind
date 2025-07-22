import React, { useState, useEffect } from 'react';
import './styles/SubmitCipher.css'; // Assuming you have a CSS file for styling
import axios from 'axios';


export default function SubmitCipher() {
 const [contentType, setContentType] = useState('text');
    const [formData, setFormData] = useState({Title: '', DecryptedText: '', EncryptedText: '', 
        Image: '', Type: '0'});
        const handleChange = (e) => {
        setFormData({...formData, [e.target.name]: e.target.value});
    }

   const handleSubmit = (e) => {
  e.preventDefault();

  const data = new FormData();
  data.append("Title", formData.Title);
  data.append("DecryptedText", formData.DecryptedText);
  data.append("Type", formData.Type);
  data.append("CipherDefinition", contentType === "image" ? 1 : 0);

  if (contentType === "image") {
    data.append("Image", formData.Image); // this must be a File object
  } else {
    data.append("EncryptedText", formData.EncryptedText);
  }

  axios.post("http://localhost:5115/api/ciphers/submitCipher", data, {
    withCredentials: true,
    headers: {
      "Content-Type": "multipart/form-data",
    },
  })
  .then(res => console.log("Success:", res.data))
  .catch(err => console.error("Error submitting cipher:", err));
};
    return (
    <> 
  <div class="form-container" >
    <h2>Create a Cipher</h2>
    <form id="cipherForm" onSubmit={handleSubmit} >
      <div class="form-group">
        <label for="title">Title:</label>
        <input type="text" id="title" name="Title" value={formData.Title} onChange={handleChange} required/>
      </div>

      <div class="form-group">
        <label for="decryptedText">Decrypted Text:</label>
        <input type="text" id="decryptedText" name="DecryptedText" value={formData.DecryptedText} onChange={handleChange} required/>
      </div>

      <div class="form-group">
        <label>Content Type:</label>
        <div class="radio-group">
          <label><input type="radio" name="contentType" value="text" onChange={() => setContentType('text')}/> Text</label>
          <label><input type="radio" name="contentType" value="image" onChange={() => setContentType('image')} /> Image</label>
        </div>
      </div>
      { contentType === 'text' && 
      <div class="form-group" id="textInputGroup">
        <label for="encryptedText">Encrypted Text:</label>
        <input type="text" id="encryptedText" name="EncryptedText" value={formData.EncryptedText} onChange={handleChange} />
      </div>}
      { contentType === 'image' && 
      <div class="form-group" id="imageInputGroup" >
        <label for="imagePath">Image File:</label>
        <input type="file" name="Image" accept="image/*" onChange={(e) => {
    setFormData({...formData, Image: e.target.files[0]});
}} />
      </div>}

      <div class="form-group">
        <label for="type">Cipher Type:</label>
        <select id="type" name="Type" value={formData.Type} onChange={handleChange}>
          <option value="1">Caesar</option>
          <option value="2">Substitution</option>
          <option value="3">Vigenère</option>
        </select>
      </div>

      <div class="form-actions">
        <button type="submit">Submit Cipher</button>
      </div>
    </form>
  </div>
     </>  
        )
}