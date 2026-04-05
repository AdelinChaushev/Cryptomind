# Cryptomind

An educational web platform for exploring, learning, and solving classical ciphers. Cryptomind combines a modern full-stack architecture with ML-powered cipher classification and real-time multiplayer features.

**Live demo:** https://cryptomind.techlab.cloud

---

## Features

- **Cipher Encyclopedia** — Learn how 14+ classical cipher types work with detailed explanations
- **Encode & Decode** — Encrypt or decrypt messages using any supported cipher
- **ML Cipher Identifier** — Paste a ciphertext and let the AI model classify its type (~90% accuracy)
- **OCR Input** — Upload an image of a ciphertext and extract the text automatically
- **Daily Challenge** — A new cipher puzzle every day, drawn from 125 curated quotes
- **Race Room** — Real-time 1v1 multiplayer mode: both players receive the same ciphertext and race to identify its type first; first to win 2 rounds wins the match
- **Leaderboard** — Global ranking based on solved challenges
- **Achievements & Badges** — Earn badges for milestones and streaks
- **Notifications** — In-app notification system

## Supported Cipher Types

| Category | Ciphers |
|---|---|
| Substitution | Caesar, ROT13, Atbash, Simple Substitution |
| Polyalphabetic | Vigenère, Autokey, Trithemius |
| Transposition | Rail Fence, Columnar Transposition, Route Cipher |
| Other | Playfair, Beaufort, Polybius Square, and more |

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core 8.0 |
| Frontend | React 19 + Vite |
| Database | SQL Server + Entity Framework Core 8 |
| Real-time | SignalR |
| ML / AI | Python (scikit-learn) |
| OCR | Custom OCR module |
| Authentication | JWT Bearer |
| Deployment | Docker / Docker Compose |

## Architecture

The solution is split into focused projects:

```
Cryptomind/
├── Cryptomind/           # ASP.NET Core API (controllers, middleware)
├── Cryptomind.Core/      # Business logic (services, SignalR hubs, contracts)
├── Cryptomind.Data/      # Data access (EF Core, repositories, migrations)
├── Cryptomind.Common/    # Shared DTOs, enums, helpers, cipher utilities
├── Cryptomind.AI/        # Python ML module for cipher classification
├── Cryptomind.OCR/       # OCR module for image-to-text extraction
├── Cryptomind.Tests/     # Unit and integration tests
└── cryptomind.js/        # React frontend (Vite)
```

## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/) & Docker Compose
- (Optional, for local dev) .NET 8 SDK, Node.js 20+, Python 3.10+

### Run with Docker

1. Clone the repository:
   ```bash
   git clone https://github.com/AdelinChaushev/Cryptomind.git
   cd Cryptomind
   ```

2. Copy the environment template and fill in your values:
   ```bash
   cp .env.example .env
   ```

3. Start all services:
   ```bash
   docker-compose up --build
   ```

4. Open `http://localhost:3000` in your browser.

### Local Development

**Backend:**
```bash
cd Cryptomind
dotnet restore
dotnet run
```

**Frontend:**
```bash
cd cryptomind.js
npm install
npm run dev
```

**ML service:**
```bash
cd Cryptomind.AI
pip install -r requirements.txt
python app.py
```

## Authors

- **Samuil Gigov** — [samuilgigov@gmail.com](mailto:samuilgigov@gmail.com)
- **Adelin Chaushev** — [adelinchaushev@gmail.com](mailto:adelinchaushev@gmail.com)

PPMG "Nikola Obreshtkov", Kazanlak, Bulgaria

## License

See [LICENSE.txt](LICENSE.txt) for details.
