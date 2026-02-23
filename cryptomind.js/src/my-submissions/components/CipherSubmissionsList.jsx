import React from 'react';
import SubmissionCard from './SubmissionCard';
import EmptyState from './EmptyState';

const CipherSubmissionsList = ({ submissions = [], onViewCipher}) => {
    if (submissions.length === 0) {
        return (
            <EmptyState
                icon="🔐"
                title="No Cipher Submissions Yet"
                subtitle="Submit your first cipher and it will appear here once reviewed."
                ctaLabel="Submit a Cipher"
                ctaHref="/submit"
            />
        );
    }

    return (
        <div className="submissions-list">
            {submissions.map((submission) => (
                <SubmissionCard
                    key={submission.id}
                    title={submission.title}
                    status={submission.status}
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
