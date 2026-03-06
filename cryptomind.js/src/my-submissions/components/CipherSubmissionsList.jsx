import SubmissionCard from './SubmissionCard';
import EmptyState from './EmptyState';

const CipherSubmissionsList = ({ submissions = [], onViewCipher}) => {
    if (submissions.length === 0) {
        return (
            <EmptyState
                icon="🔐"
                title="Няма предложени шифри"
                subtitle="Предложете първия си шифър и той ще се появи тук след преглед."
                ctaLabel="Предложи шифър"
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