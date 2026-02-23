import React from 'react';
import AnswerCard from './AnswerCard';
import EmptyState from './EmptyState';

/* Props:
   answers : array of answer objects (fetched from API)
   Each object shape:
   {
     id           : number | string,
     cipherTitle  : string,
     status       : 'approved' | 'pending' | 'rejected',
     suggestedAt  : string,
     description  : string,
     pointsEarned : number | null,
   }
   onViewCipher  : (id) => void  — passed from parent
   onViewDetails : (id) => void  — passed from parent
*/
const AnswerSuggestionsList = ({ answers = [], onViewCipher, onViewDetails }) => {
    if (answers.length === 0) {
        return (
            <EmptyState
                icon="💡"
                title="No Answer Suggestions Yet"
                subtitle="Browse experimental ciphers and submit your solution to have it verified."
                ctaLabel="Browse Experimental"
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
                    status={answer.status}
                    suggestedAt={answer.submittedAt}
                    description={answer.suggestedAnswer}
                    pointsEarned={answer.pointsEarned}
                    onViewCipher={() => onViewCipher(answer.id)}
                    onViewDetails={() => onViewDetails(answer.id)}
                />
            ))}
        </div>
    );
};

export default AnswerSuggestionsList;
