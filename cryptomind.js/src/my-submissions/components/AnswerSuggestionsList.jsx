import React from 'react';
import AnswerCard from './AnswerCard';
import EmptyState from './EmptyState';

const LightbulbIcon = () => (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <line x1="9" y1="18" x2="15" y2="18"/>
        <line x1="10" y1="22" x2="14" y2="22"/>
        <path d="M15.09 14c.18-.98.65-1.74 1.41-2.5A4.65 4.65 0 0 0 18 8 6 6 0 0 0 6 8c0 1 .23 2.23 1.5 3.5A4.61 4.61 0 0 1 8.91 14"/>
    </svg>
);

const AnswerSuggestionsList = ({ answers = [], onViewCipher, onViewDetails }) => {
    if (answers.length === 0) {
        return (
            <EmptyState
                icon={<LightbulbIcon />}
                title="Няма предложени отговори"
                subtitle="Разгледайте експерименталните шифри и изпратете вашето решение за проверка."
                ctaLabel="Разгледай експерименталните"
                ctaHref="/?type=experimental"
            />
        );
    }

    return (
        <div className="submissions-list">
            {answers.map((answer, index) => (
                <AnswerCard
                    key={answer.id}
                    index={index}
                    cipherTitle={answer.cipherTitle}
                    status={answer.status.toLowerCase()}
                    suggestedAt={answer.submittedAt}
                    description={answer.suggestedAnswer}
                    pointsEarned={answer.pointsEarned}
                    onViewCipher={() => onViewCipher(answer.cipherId)}
                    onViewDetails={() => onViewDetails(answer.cipherId)}
                />
            ))}
        </div>
    );
};

export default AnswerSuggestionsList;