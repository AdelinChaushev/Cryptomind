import React from 'react';

// status: 'pending' | 'approved' | 'rejected'
const StatusBadge = ({ status }) => {
    const classMap = {
        Pending:  'badge-pending',
        Approved: 'badge-approved',
        Rejected: 'badge-rejected',
    };

    const labelMap = {
        Pending:  'Pending',
        Approved: 'Approved',
        Rejected: 'Rejected',
    };

    return (
        <span className={`status-badge ${classMap[status] ?? 'badge-pending'}`}>
            {labelMap[status] ?? status}
        </span>
    );
};

export default StatusBadge;
