import React from 'react';
import HeroTypewriter from './HeroTypewriter';

const Hero = () => {
    return (
        <section className="hero">
            <HeroTypewriter />
            <p>
                Challenge yourself with classical ciphers, learn from AI-powered hints,
                and compete with cryptographers worldwide. From Caesar to Vigenere,
                unlock the secrets of encryption.
            </p>
            <div className="hero-buttons">
                <a href="/register" className="btn btn-primary btn-hero">
                    Start Solving Now
                </a>
                <a href="#browse" className="btn btn-secondary btn-hero">
                    Browse Ciphers
                </a>
            </div>
        </section>
    );
};

export default Hero;
