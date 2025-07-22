
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import './styles/CipherDetails.css'; 
export default function CipherDetails() {
    const { id } = useParams();
    const [cipher, setCipher] = useState(null);
    const [loading, setLoading] = useState(true);
    
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
    if (loading) {
        return <h1>Loading cipher details...</h1>;}
return (
    <>
        {cipher.isImage && (
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
        )}
        {!cipher.isImage && (
            <div className="cipher-detail-container">
                <h1 className="cipher-title">{cipher.title}</h1>
                <p className="cipher-type"><strong>Type:</strong>{cipher.cipherTags}</p>
                <p className="cipher-decrypted"><strong>Encrypted Text:</strong>{cipher.cipherText}</p>
                <p className="cipher-points"><strong>Points:</strong>{cipher.points}</p>

                <div className="cipher-actions">
                    <button className="btn hint-btn">Request Hint</button>
                    <button className="btn solution-btn">Show Solution</button>
                </div>
            </div>
        )}
    </>
)

}