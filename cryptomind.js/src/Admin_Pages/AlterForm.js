import { useState,useEffect } from 'react';
import { useParams } from 'react-router-dom';
import './styles/ApproveForm.css'; // Assuming you have a CSS file for styling
import axios from 'axios';

export default function AlterForm() {

  const [formData, setFormData] = useState({title: '', allowHint: false, allowSolution: false, tags: [],isApproved: false});
  const { id } = useParams();
  useEffect(() => {
        axios.get(`http://localhost:5115/api/admin/cipher/${id}`, { withCredentials: true })
            .then(response => {
                setFormData(response.data);
            })},[])
 // const [loading, setLoading] = useState(true);
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value
    });
  };
  const handleSubmit = (e) => {
    e.preventDefault();
    // Logic to submit the form data
    let urlEndpoint = `approve`;
    console.log(formData.isApproved);
    if(formData.isApproved) {
     urlEndpoint ='update'
    }
    console.log("Submitting form data:", formData);
    console.log("URL Endpoint:", urlEndpoint);
    axios.put(`http://localhost:5115/api/admin/cipher/${id}/${urlEndpoint}`, {
      title : formData.title,
      allowHint: formData.allowHint,
      allowSolution: formData.allowSolution,
      tags: formData.tags,
    }, { withCredentials: true }).then(res => {
      console.log("Cipher approved:", res.data);
  }).catch(err => {
      console.error("Error approving cipher:", err)})
  }
  const handleClassify = (e) => {
    e.preventDefault();

    axios.get(`http://localhost:5115/api/ciphers/classify`,
     { cipherText: formData.cipherText }
    , { withCredentials: true }).then(res => {
      console.log("Cipher classified:", res.data)}).catch(err => {
      console.error("Error classifying cipher:", err)});

  };
 if (!formData.isApproved) {
  return (
    <form id="cipher-form" method="post" onSubmit={handleSubmit}>
  <div class="form-group">
    <label for="title">Title</label>
    <input type="text" id="title" name="title" value={formData.title} onChange={handleChange} required />
  </div>

  <div class="form-group checkbox-group">
    <label>
      <input type="checkbox" id="allowHint" value={formData.allowHint} onChange={handleChange} name="allowHint" />
      Allow Hint
    </label>
    <label>
      <input type="checkbox" id="allowSolution" value={formData.allowSolution} onChange={handleChange} name="allowSolution"></input>
      Allow Solution
    </label>
  </div>

  <div class="form-group">
    <label for="tags">Tags</label>
    <select id="tags" name="tags" multiple>
      <option value="1">Logic</option>
      <option value="2">Visual</option>
      <option value="3">Wordplay</option>
      <option value="4">Math</option>
    </select>
  </div>

  <button type="submit">Approve</button>
</form>
  );
}
 return (
    <form id="cipher-form" method="post" onSubmit={handleSubmit}>
  <div class="form-group">
    <label for="title">Title</label>
    <input type="text" id="title" name="title" value={formData.title} onChange={handleChange} required />
  </div>

  <div class="form-group checkbox-group">
    <label>
      <input type="checkbox" id="allowHint" value={formData.allowHint} onChange={handleChange} name="allowHint" />
      Allow Hint
    </label>
    <label>
      <input type="checkbox" id="allowSolution" value={formData.allowSolution} onChange={handleChange} name="allowSolution"></input>
      Allow Solution
    </label>
  </div>

  <div class="form-group">
    <label for="tags">Tags</label>
    <select id="tags" name="tags" multiple>
      <option value="1">Logic</option>
      <option value="2">Visual</option>
      <option value="3">Wordplay</option>
      <option value="4">Math</option>
    </select>
  </div>

  <button type="submit">Edit</button>
  <button onClick={}>Edit</button>
</form>
  );
  
}
