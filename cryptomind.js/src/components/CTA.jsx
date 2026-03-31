import React from 'react';
import { Link } from 'react-router-dom';

const CTA = () => {
    return (
        <section className="home-section cta-section">
            <div className="home-wrap">
                <div className="cta-layout">
                    <div className="cta-text">
                        <div className="cta-overline">Присъединете се</div>
                        <h2 className="cta-headline">
                            Готови ли сте<br />
                            да&nbsp;<em>започнете?</em>
                        </h2>
                        <p className="cta-body">
                            Присъединете се към криптографското ни общество.
                            Решавайте шифри, изкачвайте класацията и
                            овладейте изкуството на криптоанализа.
                        </p>
                    </div>

                    <div className="cta-action">
                        <Link to="/register" className="btn btn-primary">
                            Създай акаунт
                            <span className="btn-arrow">→</span>
                        </Link>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default CTA;
