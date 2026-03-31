import React from 'react';

const FeatureCard = ({ index, title, description }) => {
    return (
        <div className="feature-card">
            <div className="feature-amber-bar" />
            <div className="feature-index">{index}</div>
            <h3 className="feature-title">{title}</h3>
            <p className="feature-description">{description}</p>
        </div>
    );
};

export default FeatureCard;
