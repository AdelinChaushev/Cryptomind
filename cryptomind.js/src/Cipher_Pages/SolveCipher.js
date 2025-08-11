
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import './styles/CipherDetails.css'; 
export default function SolveCipher() {
    const { id } = useParams();
    const [cipher, setCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    const [input, setInput] = useState('');
    
    useEffect(() => {
        axios.get(`http://localhost:5115/api/ciphers/cipher/${id}`, { withCredentials: true })
            .then(response => {
                setCipher(response.data);
            })
            .catch(error => {
                console.error("Error fetching cipher details:", error);
            }).finally(() => {
                setLoading(false);
            });
    }, [id]);
    const handleSubmit = (e) => {
        
        e.preventDefault();
        // Logic to submit the input
        console.log("Submitting input:", input);
        axios.post(`http://localhost:5115/api/ciphers/answerCipher/${id}`, JSON.stringify( input)      
        , { withCredentials: true,
            headers: {
                "Content-Type": "application/json",
            }

        })
            .then(res => {
                console.log("Cipher solved:", res.data);
            })
            .catch(err => {
                console.error("Error solving cipher:", err);
            });
    };
    const handleChange = (e) => {
        setInput(e.target.value);
    };
    if (loading) {
        return <h1>Loading cipher details...</h1>;}
return (
    <>
        {/* {cipher.isImage && (
            <div className="cipher-detail-container">
                <h1 className="cipher-title">{cipher.title}</h1>
                <p className="cipher-type"><strong>Type:</strong>{cipher.cipherTags}</p>
                <img className="cipher-decrypted" src={cipher.cipherText} ></img>
                <p className="cipher-points"><strong>Points:</strong>{cipher.points}</p>
                
                <div className="cipher-actions">
                    <button className="btn hint-btn">Request Hint</button>
                    <button className="btn solution-btn">Show Solution</button>
                </div>
            </div>
        )} */}
       
            <div className="cipher-detail-container">
                <h1 className="cipher-title">{cipher.title}</h1>
                <p className="cipher-type"><strong>Type:</strong>{cipher.cipherTags}</p>

             { !cipher.isImage && (<p className="cipher-decrypted"><strong>Encrypted Text:</strong>{cipher.cipherText}</p>)}
            {cipher.isImage && ( <img className="cipher-decrypted" src={cipher.cipherText} ></img>)}
                <p className="cipher-points"><strong>Points:</strong>{cipher.points}</p>
               <form id="cipher-form" method="post" onSubmit={handleSubmit}>
                <div class="form-group">
                  <label for="title">Title</label>
                  <input type="text" id="title" name="title" value={input} onChange={handleChange} required />
                   <div class="form-actions">
                     <button type="submit">Submit Answer</button>
                   </div>
               </div>
               </form>
                <div className="cipher-actions">
                    <button className="btn hint-btn">Request Hint</button>
                    <button className="btn solution-btn">Show Solution</button>
                </div>
            </div>
        
    </>
)

}