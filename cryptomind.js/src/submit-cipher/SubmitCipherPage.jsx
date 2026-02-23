import { useState } from 'react';
import PageHeader    from './PageHeader';
import SubmitForm    from './SubmitForm';
import SubmitActions from './SubmitActions';
import Sidebar       from './Sidebar';
import '../styles/submit-cipher.css';
import axios from 'axios';

const SubmitCipherPage = () => {

    const [useImage, setUseImage] = useState(false);

    const [fields, setFields] = useState({
        title:         '',
        decryptedText: '',
        encryptedText: '',
        image:         null,
        cipherType:    '',
        cipherDefinition:    '',
        allowHints:    false,
        allowAnswer:   false,
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
        formData.append("EncryptedText", fields.encryptedText); // required
        formData.append("CipherType", fields.cipherType.toString());
        formData.append("CipherDefinition", (fields.image != null ? 1 : 0).toString() );// integer or string matching enum
        console.log(fields.image != null ? 1 : 0)
        if (fields.image) {
            formData.append("Image", fields.image); // file object
        }
        /* TODO: build FormData and POST to /api/ciphers/submit */
        axios.post('http://localhost:5115/api/ciphers/submit',formData
            ,{
            withCredentials: true,
            headers: {
                'Content-Type': 'multipart/form-data'
            }}).then
        console.log('payload:', fields, 'useImage:', useImage);
    };

    const handleCancel = () => window.history.back();

    return (
        <>
            <PageHeader />

            <div className="main-layout">

                <SubmitForm
                    fields={fields}
                    useImage={useImage}
                    onToggle={handleToggle}
                    onFieldChange={handleFieldChange}
                />

                <div className="sidebar">
                    <SubmitActions onSubmit={handleSubmit} onCancel={handleCancel} />
                    <Sidebar />
                </div>

            </div>
        </>
    );
};

export default SubmitCipherPage;
