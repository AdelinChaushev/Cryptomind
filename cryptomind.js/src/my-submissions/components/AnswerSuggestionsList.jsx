import React from 'react';
import AnswerCard from './AnswerCard';
import EmptyState from './EmptyState';

const AnswerSuggestionsList = ({ answers = [], onViewCipher, onViewDetails }) => {
    if (answers.length === 0) {
        return (
            <EmptyState
                icon="💡"
                title="Няма предложени отговори"
                subtitle="Разгледайте експерименталните шифри и изпратете вашето решение за проверка."
                ctaLabel="Разгледай експерименталните"
                ctaHref="/browse?type=experimental"
            />
        );
    }

    return (
        <div className="submissions-list">
            {answers.map((answer) => (
                <AnswerCard
                    key={answer.id}
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