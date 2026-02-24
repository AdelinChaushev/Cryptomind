import { useState } from 'react';
import PageHeader    from './PageHeader';
import SubmitForm    from './SubmitForm';
import SubmitActions from './SubmitActions';
import Sidebar       from './Sidebar';
import '../styles/submit-cipher.css';
import axios from 'axios';
import { useError } from '../ErrorContext.jsx';
import CipherTypesPanel from './CipherTypesPanel';

const SubmitCipherPage = () => {

    const [useImage, setUseImage] = useState(false);
    const [submitted, setSubmitted] = useState(false);
    const [ocrText, setOcrText] = useState('');
    const [ocrLoading, setOcrLoading] = useState(false);
    const { setError } = useError();
    const [fields, setFields] = useState({
        title:             '',
        decryptedText:     '',
        encryptedText:     '',
        image:             null,
        cipherType:        '',
        cipherDefinition:  '',
        allowHints:        false,
        allowAnswer:       false,
    });

    const handleFieldChange = (name, value) => {
        setFields(prev => ({ ...prev, [name]: value }));
    };

    const handleToggle = () => {
        setUseImage(prev => !prev);
        if (!useImage) setFields(prev => ({ ...prev, encryptedText: '' }));
        else           setFields(prev => ({ ...prev, image: null }));
    };

    const handleImageChange = (file) => {
        handleFieldChange('image', file);
        setOcrText('');

        if (!file) return;

        setOcrLoading(true);
        const preview = new FormData();
        preview.append('image', file);

        axios.post('http://localhost:5115/api/ciphers/ocr-preview', preview, {
            withCredentials: true,
            headers: { 'Content-Type': 'multipart/form-data' }
        })
        .then(res => setOcrText(res.data.extractedText))
        .catch(e => setError(e.response?.data?.error || 'OCR failed'))
        .finally(() => setOcrLoading(false));
    };

   // 1. REMOVE: import { response } from 'express'; 

const handleSubmit = () => {
    // Reset states at the start of a new attempt
    setSubmitted(false);
    setError(null);

    const formData = new FormData();
    formData.append("Title", fields.title);
    formData.append("DecryptedText", fields.decryptedText || "");
    formData.append("EncryptedText", fields.encryptedText);
    formData.append("CipherType", fields.cipherType.toString());
    formData.append("CipherDefinition", (fields.image != null ? 1 : 0).toString());
    formData.append("ReviewedText", ocrText);

    if (fields.image) {
        formData.append("Image", fields.image);
    }

    axios.post('http://localhost:5115/api/ciphers/submit', formData, {
        withCredentials: true,
        headers: { 'Content-Type': 'multipart/form-data' }
    })
    .then(() => {
      
        setSubmitted(true);
        setTimeout(() => setSubmitted(false), 1200);
    })
    .catch(e => {
        // triggers on 400, 401, 404, 500, etc.
        setSubmitted(false); // Force hide the success message
        
        console.error('Submission error:', e.response?.data);

        // Safe extraction
        const errorMsg = e.response?.data?.error 
                      || e.response?.data?.title 
                      || "An unexpected error occurred.";
        
        setError(errorMsg);
    });
};

    const handleCancel = () => window.history.back();

    return (
        <>
            <PageHeader />

            <div className="main-layout">
                <CipherTypesPanel />

               <SubmitForm
                    fields={fields}
                    useImage={useImage}
                    onToggle={handleToggle}
                    onFieldChange={handleFieldChange}
                    ocrText={ocrText}
                    ocrLoading={ocrLoading}
                    onOcrTextChange={setOcrText}
                    onImageChange={handleImageChange}
                />

                <div className="sidebar">
                    {submitted && (
                        <p className="submit-success">
                            ✓ Шифърът е изпратен успешно. Ще бъде прегледан от администратор.
                        </p>
                    )}
                    <SubmitActions onSubmit={handleSubmit} onCancel={handleCancel} />
                    <Sidebar />
                </div>
            </div>
        </>
    );
};

export default SubmitCipherPage;