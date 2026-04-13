import React from 'react';

const StepNode = ({ number, title, description, isLast }) => {
    return (
        <div className={`step-node${isLast ? ' step-node--last' : ''}`}>
            <div className="step-left">
                <div className="node-circle">
                    <span className="node-number">{number}</span>
                </div>
                {!isLast && <div className="node-connector" />}
            </div>
            <div className="node-content">
                <h3 className="node-title">{title}</h3>
                <p className="node-description">{description}</p>
            </div>
        </div>
    );
};

export default StepNode;
