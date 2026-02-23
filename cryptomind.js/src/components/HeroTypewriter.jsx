import React, { useState, useEffect } from 'react';

const HeroTypewriter = () => {
    const [displayText, setDisplayText] = useState('');
    const [charIndex, setCharIndex] = useState(0);
    const [isDeleting, setIsDeleting] = useState(false);

    const fullText = "Decode the Past,\nMaster Cryptography";
    const typingSpeed = 180;
    const deletingSpeed = 150;
    const pauseEnd = 100;
    const pauseStart = 100;

    useEffect(() => {
        let timeout;

        const type = () => {
            const currentLength = isDeleting ? charIndex - 1 : charIndex + 1;
            const newText = fullText.substring(0, currentLength);

            setDisplayText(newText);
            setCharIndex(currentLength);

            if (!isDeleting && currentLength === fullText.length) {
                timeout = setTimeout(() => {
                    setIsDeleting(true);
                }, pauseEnd);
                return;
            }

            if (isDeleting && currentLength === 0) {
                setIsDeleting(false);
                timeout = setTimeout(() => {}, pauseStart);
                return;
            }

            const speed = isDeleting ? deletingSpeed : typingSpeed;
            timeout = setTimeout(type, speed);
        };

        timeout = setTimeout(type, pauseStart);

        return () => clearTimeout(timeout);
    }, [charIndex, isDeleting]);

    const formattedText = displayText.split('\n').map((line, index, arr) => (
        <React.Fragment key={index}>
            {line}
            {index < arr.length - 1 && <br />}
        </React.Fragment>
    ));

    return (
        <h1 id="hero-title">
            <span className="typed-text">{formattedText}</span>
            <span className="cursor">|</span>
        </h1>
    );
};

export default HeroTypewriter;
