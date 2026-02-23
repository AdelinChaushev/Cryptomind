import React from 'react';

const Sidebar = () => {
    return (
        <>
            {/* What happens after submit */}
            <div className="info-card">
                <p className="info-card-title">WHAT HAPPENS NEXT</p>
                <div className="info-card-body">
                    <div className="steps">
                        <div className="step">
                            <span className="step-num">1</span>
                            <div className="step-body">
                                <p className="step-title">ML ANALYSIS</p>
                                <p className="step-desc">The neural network classifies the cipher type and outputs a confidence score.</p>
                            </div>
                        </div>
                        <div className="step">
                            <span className="step-num">2</span>
                            <div className="step-body">
                                <p className="step-title">ADMIN REVIEW</p>
                                <p className="step-desc">An admin checks the result. If confidence is below 85% they run the LLM assistant.</p>
                            </div>
                        </div>
                        <div className="step">
                            <span className="step-num">3</span>
                            <div className="step-body">
                                <p className="step-title">GOES LIVE</p>
                                <p className="step-desc">Approved as Standard (known answer) or Experimental (community solves it).</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Tips */}
            <div className="info-card">
                <p className="info-card-title">TIPS</p>
                <div className="info-card-body">
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>150+ characters</strong> — the ML classifier needs enough text. 200–400 is the sweet spot.</p>
                    </div>
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>English only</strong> — the system is trained on English text.</p>
                    </div>
                    <div className="info-row">
                        <span className="info-dot"></span>
                        <p className="info-text"><strong>Single cipher</strong> — don't nest or combine ciphers. One cipher per submission.</p>
                    </div>
                </div>
            </div>

            {/* Cipher reference */}
            <div className="info-card">
                <p className="info-card-title">SUPPORTED TYPES</p>
                <div className="info-card-body">
                    <div className="cipher-ref">
                        <div>
                            <p className="cipher-family-name">SUBSTITUTION</p>
                            <div className="cipher-tags">
                                <span className="cipher-tag sub">Caesar</span>
                                <span className="cipher-tag sub">Atbash</span>
                                <span className="cipher-tag sub">ROT13</span>
                                <span className="cipher-tag sub">Simple Subst.</span>
                            </div>
                        </div>
                        <div>
                            <p className="cipher-family-name">POLYALPHABETIC</p>
                            <div className="cipher-tags">
                                <span className="cipher-tag poly">Vigenere</span>
                                <span className="cipher-tag poly">Autokey</span>
                                <span className="cipher-tag poly">Trithemius</span>
                            </div>
                        </div>
                        <div>
                            <p className="cipher-family-name">TRANSPOSITION</p>
                            <div className="cipher-tags">
                                <span className="cipher-tag trans">Rail Fence</span>
                                <span className="cipher-tag trans">Columnar</span>
                                <span className="cipher-tag trans">Route</span>
                            </div>
                        </div>
                        <div>
                            <p className="cipher-family-name">ENCODING</p>
                            <div className="cipher-tags">
                                <span className="cipher-tag enc">Base64</span>
                                <span className="cipher-tag enc">Morse</span>
                                <span className="cipher-tag enc">Binary</span>
                                <span className="cipher-tag enc">Hex</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Sidebar;
