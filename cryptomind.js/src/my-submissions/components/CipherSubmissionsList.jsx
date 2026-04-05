import React from 'react';
import SubmissionCard from './SubmissionCard';
import EmptyState from './EmptyState';

const LockIcon = () => (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
        <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
    </svg>
);

const CipherSubmissionsList = ({ submissions = [], onViewCipher}) => {
    if (submissions.length === 0) {
        return (
            <EmptyState
                icon={<LockIcon />}
                title="Няма предложени шифри"
                subtitle="Предложете първия си шифър и той ще се появи тук след преглед."
                ctaLabel="Предложи шифър"
                ctaHref="/submit"
            />
        );
    }

    return (
        <div className="submissions-list">
            {submissions.map((submission, index) => (
                <SubmissionCard
                    key={submission.id}
                    index={index}
                    title={submission.title}
                    status={submission.status.toLowerCase()}
                    submittedAt={submission.submittedTime}
                    cipherType={submission.cipherType}
                    definition={submission.definition}
                    description={submission.cipherText}
                    rejectionReason={submission.rejectionReason}
                    onViewCipher={() => onViewCipher(submission.id)}
                />
            ))}
        </div>
    );
};

export default CipherSubmissionsList;