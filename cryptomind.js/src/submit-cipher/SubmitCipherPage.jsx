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

    const handleSubmit = () => {
        const formData = new FormData();
        formData.append("Title", fields.title);
        formData.append("DecryptedText", fields.decryptedText || "");
        formData.append("EncryptedText", fields.encryptedText);
        formData.append("CipherType", fields.cipherType.toString());
        formData.append("CipherDefinition", (fields.image != null ? 1 : 0).toString());
        console.log(fields.image != null ? 1 : 0);
        if (fields.image) {
            formData.append("Image", fields.image);
        }

        axios.post('http://localhost:5115/api/ciphers/submit', formData, {
            withCredentials: true,
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
        .then(c => setSubmitted(true))
        .catch(e => {
            console.log('Submission error:', e);
            console.log('Server Response Data:', e.response?.data);
            setError(e.response?.data?.title || e.response.data.error);
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