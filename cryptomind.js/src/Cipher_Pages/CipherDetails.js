
import { useParams } from 'react-router-dom';
export default function CipherDetails() {
    const { id } = useParams();
    
    return(
        <div className="cipher-detail-container">
  <h1 className="cipher-title">Caesar Cipher</h1>
  <p className="cipher-type"><strong>Type:</strong> Substitution</p>
  <p className="cipher-decrypted"><strong>Decrypted Text:</strong> HELLO WORLD</p>
  <p className="cipher-points"><strong>Points:</strong> 10</p>

   <div className="cipher-actions">
    <button className="btn hint-btn">Request Hint</button>
    <button className="btn solution-btn">Show Solution</button>
     </div>
   </div>

    )

}