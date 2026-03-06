const StatusBadge = ({ status }) => {
    const classMap = {
        Pending:  'badge-pending',
        Approved: 'badge-approved',
        Rejected: 'badge-rejected',
    };

    const labelMap = {
        Pending:  'Очаква преглед',
        Approved: 'Одобрен',
        Rejected: 'Отхвърлен',
    };

    return (
        <span className={`status-badge ${classMap[status] ?? 'badge-pending'}`}>
            {labelMap[status] ?? status}
        </span>
    );
};

export default StatusBadge;