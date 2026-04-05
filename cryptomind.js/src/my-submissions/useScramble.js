import { useState, useEffect } from 'react';

const CHARS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#!@$%&*?<>';

export const useScramble = (text, delay = 0) => {
    const [display, setDisplay] = useState(text);

    useEffect(() => {
        if (!text) return;

        let startId;
        let frameId;
        let frame = 0;
        const TOTAL_FRAMES = 22;
        const FRAME_MS = 28;

        const tick = () => {
            frame++;
            const progress = frame / TOTAL_FRAMES;

            const scrambled = text
                .split('')
                .map((char, i) => {
                    if (char === ' ' || char === ':' || char === '-') return char;
                    const charProgress = i / Math.max(text.length - 1, 1);
                    if (charProgress < progress) return char;
                    return CHARS[Math.floor(Math.random() * CHARS.length)];
                })
                .join('');

            setDisplay(scrambled);

            if (frame < TOTAL_FRAMES) {
                frameId = setTimeout(tick, FRAME_MS);
            } else {
                setDisplay(text);
            }
        };

        startId = setTimeout(tick, delay);

        return () => {
            clearTimeout(startId);
            clearTimeout(frameId);
        };
    }, [text, delay]);

    return display || text;
};
