"""
Download and properly clean Gutenberg texts to maintain IC
"""
import requests
from pathlib import Path
import re

def download_and_clean_book(book_id, title):
    """Download a Gutenberg book and clean it MINIMALLY"""
    
    url = f"https://www.gutenberg.org/files/{book_id}/{book_id}-0.txt"
    
    print(f"Downloading: {title}...")
    
    try:
        response = requests.get(url, timeout=30)
        response.raise_for_status()
        text = response.text
    except:
        print(f"  ❌ Failed to download {title}")
        return ""
    
    # Find start and end markers (MINIMAL cleaning)
    start_markers = [
        "*** START OF THIS PROJECT GUTENBERG",
        "*** START OF THE PROJECT GUTENBERG",
        "*END*THE SMALL PRINT"
    ]
    
    end_markers = [
        "*** END OF THIS PROJECT GUTENBERG",
        "*** END OF THE PROJECT GUTENBERG"
    ]
    
    # Find actual content
    start_idx = 0
    for marker in start_markers:
        idx = text.find(marker)
        if idx != -1:
            start_idx = text.find('\n', idx) + 1
            break
    
    end_idx = len(text)
    for marker in end_markers:
        idx = text.find(marker)
        if idx != -1:
            end_idx = idx
            break
    
    text = text[start_idx:end_idx]
    
    # MINIMAL cleaning - preserve as much text as possible
    # Only remove obvious non-content
    lines = text.split('\n')
    cleaned_lines = []
    
    for line in lines:
        line = line.strip()
        
        # Skip empty lines
        if not line:
            continue
        
        # Skip chapter headings (all caps, short)
        if line.isupper() and len(line) < 50:
            continue
        
        # Skip lines that are mostly numbers or symbols
        alpha_count = sum(c.isalpha() for c in line)
        if alpha_count < len(line) * 0.5:
            continue
        
        cleaned_lines.append(line)
    
    cleaned_text = ' '.join(cleaned_lines)
    
    # MINIMAL normalization
    # Just collapse multiple spaces
    cleaned_text = re.sub(r'\s+', ' ', cleaned_text)
    
    print(f"  ✅ Downloaded {len(cleaned_text):,} characters")
    
    return cleaned_text

def create_proper_corpus():
    """Create corpus from multiple Gutenberg books"""
    
    books = [
        (1342, "Pride and Prejudice"),
        (1661, "Sherlock Holmes"),
        (11, "Alice in Wonderland"),
        (84, "Frankenstein"),
        (345, "Dracula"),
        (98, "A Tale of Two Cities"),
        (1952, "The Yellow Wallpaper"),
        (2701, "Moby Dick"),
        (16, "Peter Pan"),
        (174, "The Picture of Dorian Gray")
    ]
    
    print("="*70)
    print("DOWNLOADING GUTENBERG CORPUS")
    print("="*70)
    
    all_text = []
    
    for book_id, title in books:
        text = download_and_clean_book(book_id, title)
        if text:
            all_text.append(text)
    
    # Combine
    corpus = ' '.join(all_text)
    
    print(f"\n{'='*70}")
    print(f"CORPUS STATISTICS")
    print(f"{'='*70}")
    print(f"Total characters: {len(corpus):,}")
    print(f"Books included: {len(all_text)}")
    
    # Verify IC
    from src.feature_extraction import FeatureExtractor
    extractor = FeatureExtractor()
    
    print(f"\nTesting IC on 10 random samples...")
    ics = []
    import random
    for i in range(10):
        start = random.randint(0, len(corpus) - 1000)
        sample = corpus[start:start+1000]
        features = extractor.extract(sample)
        ics.append(features[26])
        print(f"  Sample {i+1}: IC = {features[26]:.4f}")
    
    avg_ic = sum(ics) / len(ics)
    print(f"\nAverage IC: {avg_ic:.4f}")
    print(f"Expected IC: 0.065-0.068")
    
    if avg_ic < 0.055:
        print(f"⚠️  WARNING: IC still too low! Corpus may have issues.")
    elif avg_ic > 0.075:
        print(f"⚠️  WARNING: IC too high!")
    else:
        print(f"✅ IC is in acceptable range!")
    
    # Save
    output_dir = Path('data/corpus')
    output_dir.mkdir(parents=True, exist_ok=True)
    
    output_file = output_dir / 'corpus_proper.txt'
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(corpus)
    
    print(f"\n✅ Corpus saved to {output_file}")
    print(f"Size: {len(corpus) / 1024 / 1024:.1f} MB")

if __name__ == '__main__':
    create_proper_corpus()