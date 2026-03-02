import { useState } from 'react';
import PageHeader    from './PageHeader';
import SubmitForm    from './SubmitForm';
import SubmitActions from './SubmitActions';
import Sidebar       from './Sidebar';
import '../styles/submit-cipher.css';
import axios from 'axios';
import { useError } from '../ErrorContext.jsx';
import CipherTypesPanel from './CipherTypesPanel';
const API_BASE = import.meta.env.VITE_API_URL
const SubmitCipherPage = () => {

    const [useImage, setUseImage] = useState(false);
    const [submitted, setSubmitted] = useState(false);
    const [ocrText, setOcrText] = useState('');
    const [ocrFailed, setOcrFailed] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
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
        setOcrFailed(false);

        if (!file) return;

        setOcrLoading(true);
        const preview = new FormData();
        preview.append('image', file);

        axios.post(`${API_BASE}/api/ciphers/ocr-preview`, preview, {
            withCredentials: true,
            headers: { 'Content-Type': 'multipart/form-data' }
        })
        .then(res => {
            setOcrText(res.data.extractedText);
            setOcrFailed(false);
        })
        .catch(e => {
            setError(e.response?.data?.error || 'OCR се провали');
            setOcrFailed(true);
        })
        .finally(() => setOcrLoading(false));
    };

    const handleSubmit = () => {
        setSubmitted(false);
        setError(null);
        setIsSubmitting(true);
        const formData = new FormData();
        formData.append("Title", fields.title);
        formData.append("DecryptedText", fields.decryptedText || "");
        formData.append("EncryptedText", fields.encryptedText);
        formData.append("CipherType", fields.cipherType.toString());
        formData.append("CipherDefinition", (fields.image != null ? 1 : 0).toString());

        if (fields.image) {
            formData.append("Image", fields.image);
            formData.append("ReviewedText", ocrText);
        }

        axios.post(`${API_BASE}/api/ciphers/submit`, formData, {
            withCredentials: true,
            headers: { 'Content-Type': 'multipart/form-data' }
        })
        .then(() => {
            setFields({
                title:             '',
                decryptedText:     '',
                encryptedText:     '',
                image:             null,
                cipherType:        '',
                cipherDefinition:  '',
                allowHints:        false,
                allowAnswer:       false,
            });
            setOcrText('');
            setOcrFailed(false);
            setUseImage(false);
            setSubmitted(true);
            setTimeout(() => setIsSubmitting(false), 4000);
        })
        .catch(e => {
            setSubmitted(false);
            console.error('Грешка при изпращане:', e.response?.data);
            const errorMsg = e.response?.data?.error
                          || e.response?.data?.errors.Title == "The Title field is required." && "Полетo заглавие е задължително."
                          || "Възникна неочаквана грешка.";
                          
            setError(errorMsg);
        }).finally(() => setIsSubmitting(false));
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
                    ocrFailed={ocrFailed}
                    ocrLoading={ocrLoading}
                    onOcrTextChange={setOcrText}
                    onImageChange={handleImageChange}
                />

                <div className="sidebar">
                    {isSubmitting && (
                        <div className="submit-pending">
                            <div className="submit-pending__spinner" />
                            <span className="submit-pending__text">Изпращане и анализиране</span>
                        </div>
                    )}
                    {submitted && !isSubmitting && (
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