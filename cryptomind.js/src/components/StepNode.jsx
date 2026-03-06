import React from 'react';

const StepNode = ({ number, title, description, isLast }) => {
    return (
        <div className="step-node">
            <div className="node-circle">
                <span className="node-number">{number}</span>
            </div>
            <div className="node-content">
                <h3 className="node-title">{title}</h3>
                <p className="node-description">{description}</p>
            </div>
            {!isLast && <div className="node-connector"></div>}
        </div>
    );
};

export default StepNode;
