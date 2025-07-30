import { useState } from 'react';
import { useParams } from 'react-router-dom';
import './styles/ApproveForm.css'; // Assuming you have a CSS file for styling
import axios from 'axios';

export default function ApproveForm() {

  const [formData, setFormData] = useState({title: '', allowHint: false, allowSolution: false, tags: []});
 // const [loading, setLoading] = useState(true);
  const { id } = useParams();
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
    axios.put(`http://localhost:5115/api/cipherAdmin/approveCipher/${id}`, formData, { withCredentials: true }).then(res => {
      console.log("Cipher approved:", res.data);
  }).catch(err => {
      console.error("Error approving cipher:", err)})
      

  };
  //if (loading) {
   // return <h1>Loading form...</h1>;
  //}
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
      <input type="checkbox" id="allowSolution" value={formData.allowSolution} onChange={handleChange} name="allowSolution" />
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

  <button type="submit">Submit</button>
</form>
  );
}