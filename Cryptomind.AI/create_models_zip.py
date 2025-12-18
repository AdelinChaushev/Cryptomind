"""
Create ZIP with ONLY the trained models for Google Drive
Partner downloads this separately
"""
import zipfile
import os
from pathlib import Path
from datetime import datetime

print("="*70)
print("CREATING MODELS PACKAGE FOR GOOGLE DRIVE")
print("="*70)

# Only include models folder
models_dir = Path('models')

if not models_dir.exists():
    print("❌ Error: models/ folder not found!")
    exit(1)

# Create zip filename
timestamp = datetime.now().strftime('%Y%m%d')
zip_filename = f'Cryptomind_Models_{timestamp}.zip'

print(f"\n📦 Creating: {zip_filename}")
print("-"*70)

file_count = 0
total_size = 0
with zipfile.ZipFile(zip_filename, 'w', zipfile.ZIP_DEFLATED) as zipf:
    for file_path in models_dir.rglob('*'):
        if file_path.is_file():
            print(f"  Adding: {file_path}")
            zipf.write(file_path, file_path)
            file_count += 1
            total_size += file_path.stat().st_size

print("-"*70)
print(f"\n✅ Models package created: {zip_filename}")
print(f"📦 Files included: {file_count}")

# Get file size
file_size_mb = os.path.getsize(zip_filename) / (1024 * 1024)
print(f"📏 Package size: {file_size_mb:.2f} MB")
print(f"📏 Uncompressed size: {total_size / (1024 * 1024):.2f} MB")

print("\n📤 Next steps:")
print("  1. Upload this ZIP to Google Drive")
print("  2. Get shareable link")
print("  3. Add link to GitHub README.md")
print("  4. Partner downloads models separately")
print("="*70)