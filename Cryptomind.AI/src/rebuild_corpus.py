from pathlib import Path
import random

def create_synthetic_corpus():
    word_lists = {
        'common': [
            'the', 'be', 'to', 'of', 'and', 'that', 'have', 'with',
            'for', 'not', 'but', 'from', 'they', 'one', 'all', 'there',
            'their', 'when', 'time', 'about', 'into', 'than', 'them',
            'these', 'other', 'then', 'been', 'after', 'more', 'through'
        ] * 100,

        'medium': [
            'which', 'people', 'could', 'some', 'only', 'over', 'know',
            'just', 'first', 'also', 'very', 'even', 'most', 'such',
            'because', 'make', 'should', 'before', 'where', 'between',
            'those', 'both', 'each', 'under', 'while', 'never', 'being'
        ] * 50,

        'content': [
            'system', 'information', 'computer', 'network', 'security',
            'process', 'development', 'education', 'research', 'analysis',
            'question', 'problem', 'example', 'important', 'different',
            'following', 'several', 'experience', 'knowledge', 'technology',
            'government', 'business', 'science', 'algorithm', 'function'
        ] * 20,

        'varied': [
            'structure', 'pattern', 'method', 'approach', 'concept',
            'result', 'evidence', 'theory', 'practice', 'application',
            'understanding', 'relationship', 'organization', 'management',
            'environment', 'perspective', 'strategy', 'principle', 'framework'
        ] * 10
    }

    word_pool = []
    for category, words in word_lists.items():
        word_pool.extend(words)

    sentences = []
    for i in range(10000):
        length = random.randint(5, 20)
        words = random.choices(word_pool, k=length)

        words[0] = words[0].capitalize()

        if random.random() < 0.3 and length > 8:
            comma_pos = random.randint(3, length - 3)
            words[comma_pos] += ','

        sentence = ' '.join(words) + '.'
        sentences.append(sentence)

    corpus = ' '.join(sentences)

    from src.feature_extraction import FeatureExtractor
    extractor = FeatureExtractor()

    ics = []
    for i in range(20):
        start = i * 5000
        if start + 1000 > len(corpus):
            break
        sample = corpus[start:start+1000]
        features = extractor.extract(sample)
        ic = features[26]
        ics.append(ic)

    avg_ic = sum(ics) / len(ics)

    print(f"Average IC: {avg_ic:.4f} (expected 0.060-0.070)")

    if avg_ic < 0.055:
        print("IC too low, word distribution needs adjustment")
        return False
    elif avg_ic > 0.075:
        print("IC too high, word distribution needs adjustment")
        return False

    output_dir = Path('data/corpus')
    output_dir.mkdir(parents=True, exist_ok=True)

    old_corpus = output_dir / 'corpus.txt'
    if old_corpus.exists():
        old_corpus.rename(output_dir / 'corpus_backup.txt')

    with open(output_dir / 'corpus.txt', 'w', encoding='utf-8') as f:
        f.write(corpus)

    return True

if __name__ == '__main__':
    create_synthetic_corpus()