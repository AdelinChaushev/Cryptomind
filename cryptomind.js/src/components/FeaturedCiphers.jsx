import React from 'react';
import CipherCard from './CipherCard';

const ciphers = [
    {
        difficulty: 'Easy',
        type: 'Caesar',
        title: 'Ancient Roman Secret',
        description: "A message from Julius Caesar himself. Can you shift the letters to reveal the hidden meaning?",
        solves: 156
    },
    {
        difficulty: 'Medium',
        type: 'Vigenere',
        title: 'Encrypted Diary',
        description: "A polyalphabetic cipher with a secret keyword. Frequency analysis won't help you here.",
        solves: 67
    },
    {
        difficulty: 'Hard',
        type: 'Columnar',
        title: 'Scrambled Message',
        description: 'Letters rearranged in a complex pattern. Can you find the original order?',
        solves: 23
    },
    {
        difficulty: 'Easy',
        type: 'Atbash',
        title: 'Mirror Writing',
        description: 'The alphabet reversed. A becomes Z, B becomes Y. Simple but elegant.',
        solves: 198
    }
];

const FeaturedCiphers = () => {
    return (
        <section className="featured-section">
            <div className="section-header">
                <h2>Featured Challenges</h2>
                <p>Try these popular ciphers and test your skills</p>
            </div>
            <div className="cipher-grid">
                {ciphers.map((cipher, index) => (
                    <CipherCard key={index} {...cipher} />
                ))}
            </div>
        </section>
    );
};

export default FeaturedCiphers;
