"""
Generate synthetic English corpus with proper IC
Uses weighted word sampling to match English letter frequencies
"""
from pathlib import Path
import random

def create_synthetic_corpus():
    """Generate corpus with proper English statistics"""
    
    print("="*70)
    print("CREATING SYNTHETIC CORPUS")
    print("="*70)
    
    # English words weighted by letter frequency
    # This will produce IC ≈ 0.065
    word_lists = {
        # High-frequency words (use more E, T, A, O, I, N)
        'common': [
            'the', 'be', 'to', 'of', 'and', 'that', 'have', 'with',
            'for', 'not', 'but', 'from', 'they', 'one', 'all', 'there',
            'their', 'when', 'time', 'about', 'into', 'than', 'them',
            'these', 'other', 'then', 'been', 'after', 'more', 'through'
        ] * 100,  # Use 100x more
        
        # Medium frequency
        'medium': [
            'which', 'people', 'could', 'some', 'only', 'over', 'know',
            'just', 'first', 'also', 'very', 'even', 'most', 'such',
            'because', 'make', 'should', 'before', 'where', 'between',
            'those', 'both', 'each', 'under', 'while', 'never', 'being'
        ] * 50,
        
        # Content words
        'content': [
            'system', 'information', 'computer', 'network', 'security',
            'process', 'development', 'education', 'research', 'analysis',
            'question', 'problem', 'example', 'important', 'different',
            'following', 'several', 'experience', 'knowledge', 'technology',
            'government', 'business', 'science', 'algorithm', 'function'
        ] * 20,
        
        # Less common but add variety
        'varied': [
            'structure', 'pattern', 'method', 'approach', 'concept',
            'result', 'evidence', 'theory', 'practice', 'application',
            'understanding', 'relationship', 'organization', 'management',
            'environment', 'perspective', 'strategy', 'principle', 'framework'
        ] * 10
    }
    
    # Flatten into weighted pool
    word_pool = []
    for category, words in word_lists.items():
        word_pool.extend(words)
    
    print(f"Word pool size: {len(word_pool)} weighted words")
    print(f"Generating sentences...")
    
    # Generate sentences
    sentences = []
    for i in range(10000):
        # Vary sentence length
        length = random.randint(5, 20)
        words = random.choices(word_pool, k=length)
        
        # Capitalize first word
        words[0] = words[0].capitalize()
        
        # Add punctuation
        if random.random() < 0.3 and length > 8:
            comma_pos = random.randint(3, length - 3)
            words[comma_pos] += ','
        
        sentence = ' '.join(words) + '.'
        sentences.append(sentence)
        
        if (i + 1) % 2000 == 0:
            print(f"  {i + 1} sentences...")
    
    corpus = ' '.join(sentences)
    
    print(f"\n{'='*70}")
    print("VERIFYING IC")
    print("="*70)
    
    # Test IC
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
        if i < 10:
            print(f"  Sample {i+1:2d}: IC = {ic:.4f}")
    
    avg_ic = sum(ics) / len(ics)
    
    print(f"\nAverage IC: {avg_ic:.4f}")
    print(f"Expected: 0.060-0.070")
    
    if avg_ic < 0.055:
        print("❌ IC still too low - word distribution needs adjustment")
        return False
    elif avg_ic > 0.075:
        print("⚠️ IC too high - word distribution needs adjustment")
        return False
    else:
        print("✅ IC IS CORRECT!")
    
    # Save
    output_dir = Path('data/corpus')
    output_dir.mkdir(parents=True, exist_ok=True)
    
    old_corpus = output_dir / 'corpus.txt'
    if old_corpus.exists():
        backup = output_dir / 'corpus_broken_backup.txt'
        old_corpus.rename(backup)
        print(f"\n📦 Old corpus backed up")
    
    new_corpus = output_dir / 'corpus.txt'
    with open(new_corpus, 'w', encoding='utf-8') as f:
        f.write(corpus)
    
    print(f"✅ Corpus saved: {len(corpus) / 1024 / 1024:.1f} MB")
    
    return True

if __name__ == '__main__':
    success = create_synthetic_corpus()
    
    if success:
        print("\n" + "="*70)
        print("NEXT STEPS")
        print("="*70)
        print("1. python train_all.py")
        print("2. python test_pipeline.py")
        print("3. Expected: 90-95% accuracy")