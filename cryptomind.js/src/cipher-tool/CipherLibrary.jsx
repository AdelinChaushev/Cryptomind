import { useState, useEffect } from "react";
import"../styles/cipher-library.css";
import CipherCard from "./CipherCard";

// ─── CIPHER DATA ─────────────────────────────────────────────────────────────
const FAMILIES = [
  {
    id: "substitution",
    label: "Substitution Ciphers",
    color: "#fbbf24",
    tagline: "Replace each letter with a different one",
    description: "Substitution ciphers work by mapping each letter of the plaintext to a different letter (or symbol) in the ciphertext. The positions of letters stay the same — only their identities change. This means letter frequency patterns are preserved, which is the main weakness of this family.",
    ciphers: [
      {
        id: "caesar",
        label: "Caesar",
        color: "#fbbf24",
        era: "Ancient Rome, ~60 BC",
        difficulty: "Beginner",
        keyType: "Shift number (1–25)",
        example: { plain: "HELLO", key: "shift 3", cipher: "KHOOR" },
        how: "Every letter in the plaintext is shifted a fixed number of positions forward in the alphabet. With a shift of 3, A becomes D, B becomes E, and so on. When you reach Z, it wraps around back to the start.",
        formula: "(position + shift) mod 26",
        weakness: "Only 25 possible keys. A brute-force attack tries all of them in seconds. Frequency analysis also immediately reveals the shift since the most common letter in the ciphertext is likely the encrypted version of E.",
        funFact: "Julius Caesar reportedly used a shift of 3 to communicate with his generals. His nephew Augustus used a shift of 1, and apparently never bothered shifting back past Z — he just wrote AA for X.",
        tags: ["monoalphabetic", "symmetric", "historical"],
      },
      {
        id: "rot13",
        label: "ROT13",
        color: "#22d3ee",
        era: "Early internet, 1980s",
        difficulty: "Beginner",
        keyType: "None (fixed shift of 13)",
        example: { plain: "HELLO", key: "—", cipher: "URYYB" },
        how: "ROT13 is a Caesar cipher locked permanently at a shift of 13. Because the alphabet has 26 letters, applying ROT13 twice returns the original text — it is its own inverse. Encryption and decryption are the same operation.",
        formula: "(position + 13) mod 26",
        weakness: "Provides zero cryptographic security. It was never intended to hide information from a determined reader — it was used on early internet forums to hide spoilers and offensive jokes from casual view.",
        funFact: "ROT13 was the default spoiler-hiding mechanism on Usenet newsgroups in the 1980s and 90s. If you wanted to discuss the ending of a movie, you'd ROT13 it so readers had to actively choose to decode it.",
        tags: ["monoalphabetic", "self-inverse", "internet"],
      },
      {
        id: "atbash",
        label: "Atbash",
        color: "#a78bfa",
        era: "Ancient Hebrew, ~600 BC",
        difficulty: "Beginner",
        keyType: "None (fixed mirror mapping)",
        example: { plain: "HELLO", key: "—", cipher: "SVOOL" },
        how: "Atbash maps the alphabet to its mirror image. A maps to Z, B maps to Y, C maps to X, and so on. Like ROT13, it is its own inverse — encrypting twice returns the original. The name comes from the first two and last two letters of the Hebrew alphabet: Aleph-Tav-Beth-Shin.",
        formula: "25 − position",
        weakness: "Only one possible key — the mapping is fixed. Frequency analysis trivially breaks it since you just need to know which is the most common letter in the ciphertext and map it to E.",
        funFact: "Atbash appears in the Bible — the Book of Jeremiah uses it to encode the name 'Babylon' as 'Sheshach'. Scholars debated for centuries whether this was intentional or coincidence until the cipher was identified.",
        tags: ["monoalphabetic", "self-inverse", "biblical"],
      },
      {
        id: "simpleSubstitution",
        label: "Simple Substitution",
        color: "#fb923c",
        era: "Medieval Europe",
        difficulty: "Intermediate",
        keyType: "A full 26-letter scrambled alphabet",
        example: { plain: "HELLO", key: "QWERTYUIOPASDFGHJKLZXCVBNM", cipher: "ITSSG" },
        how: "A full permutation of the alphabet is used as the key — every letter maps to a unique different letter. Unlike Caesar, the mapping is completely arbitrary rather than a simple shift. The key space is 26! (factorial) ≈ 4 × 10²⁶ possible keys.",
        formula: "table[position]",
        weakness: "Despite the enormous key space, frequency analysis destroys it. The letter that appears most often in the ciphertext is almost certainly E. Second most common is likely T. Combined with common bigrams (TH, HE, IN) and word patterns, a skilled analyst can crack it in minutes.",
        funFact: "Edgar Allan Poe was obsessed with simple substitution ciphers. In 1839 he publicly challenged readers of a magazine to send him ciphertexts he couldn't solve. He broke nearly every submission and wrote 'The Gold-Bug', a short story about solving one.",
        tags: ["monoalphabetic", "key-based", "frequency-vulnerable"],
      },
    ],
  },
  {
    id: "polyalphabetic",
    label: "Polyalphabetic Ciphers",
    color: "#10b981",
    tagline: "Use multiple substitution alphabets to flatten frequencies",
    description: "Polyalphabetic ciphers defeat simple frequency analysis by using a different substitution alphabet at each position. The same plaintext letter can encrypt to different ciphertext letters depending on where it appears. This makes the frequency distribution of the ciphertext much flatter and harder to analyse.",
    ciphers: [
      {
        id: "vigenere",
        label: "Vigenère",
        color: "#10b981",
        era: "16th century, Blaise de Vigenère",
        difficulty: "Intermediate",
        keyType: "A keyword (repeated)",
        example: { plain: "HELLO", key: "KEY", cipher: "RIJVS" },
        how: "A keyword is repeated to match the length of the plaintext. Each letter of the keyword determines a Caesar shift for the corresponding plaintext letter. K means shift 10, E means shift 4, Y means shift 24, then K again, and so on. The same plaintext letter encrypts differently depending on which keyword letter aligns with it.",
        formula: "(plainPos + keyPos) mod 26",
        weakness: "The Kasiski test (1863) finds repeated sequences in the ciphertext which reveal the keyword length. Once the length is known, the message splits into groups where each group was encrypted with the same Caesar shift — and each group is trivially broken by frequency analysis.",
        funFact: "Vigenère was called 'le chiffre indéchiffrable' — the indecipherable cipher — for over 300 years. Charles Babbage (inventor of the mechanical computer) cracked it in the 1840s but kept it secret to help British intelligence during the Crimean War.",
        tags: ["polyalphabetic", "keyword", "repeating"],
      },
      {
        id: "autokey",
        label: "Autokey",
        color: "#34d399",
        era: "16th century, Blaise de Vigenère",
        difficulty: "Advanced",
        keyType: "A short keyword primer, then plaintext extends the key",
        example: { plain: "HELLO", key: "KEY → KEYHE", cipher: "RIJPS" },
        how: "Autokey starts like Vigenère — a keyword primer provides the first few shifts. But once the primer runs out, instead of repeating the keyword, the plaintext letters themselves become the key. This means the key is as long as the message and never repeats, defeating the Kasiski test.",
        formula: "(plainPos + keyPos) mod 26  where key = primer + plaintext",
        weakness: "Because the key is derived from the plaintext, a known-plaintext attack is powerful. If you know even a portion of the plaintext, you can recover the key used for that segment and work outward. Mutual index of coincidence analysis can also detect the primer length.",
        funFact: "Vigenère actually invented Autokey first and considered it his true cipher. The simpler repeating-keyword variant that became famous under his name was described by Giovan Bellaso decades earlier — one of cryptography's great attribution mistakes.",
        tags: ["polyalphabetic", "self-extending", "advanced"],
      },
      {
        id: "trithemius",
        label: "Trithemius",
        color: "#6ee7b7",
        era: "1508, Johannes Trithemius",
        difficulty: "Intermediate",
        keyType: "None (shift increases by 1 each position)",
        example: { plain: "HELLO", key: "0,1,2,3,4", cipher: "HFNOS" },
        how: "The shift value increases by exactly 1 at each position. The first letter uses shift 0 (unchanged), the second uses shift 1, the third uses shift 2, and so on. Once the shift reaches 25, it wraps back to 0. There is no configurable key — the schedule is completely fixed.",
        formula: "(plainPos + letterIndex) mod 26",
        weakness: "Because the shift schedule is fully deterministic and there is no secret key, anyone who knows the cipher was Trithemius can immediately decrypt it. It offers only slightly more resistance than a Caesar cipher, but as part of a layered system it adds modest confusion.",
        funFact: "Johannes Trithemius described this cipher in his book 'Polygraphia' (1508), the first printed book on cryptography. He also wrote 'Steganographia', a book about hiding messages in text, which was so mysterious that it was placed on the Catholic Church's Index of Forbidden Books for 90 years.",
        tags: ["polyalphabetic", "no-key", "tabula-recta"],
      },
    ],
  },
  {
    id: "transposition",
    label: "Transposition Ciphers",
    color: "#60a5fa",
    tagline: "Scramble the positions of letters without changing them",
    description: "Transposition ciphers don't change what letters are used — they change where they appear. All the original letters are present in the ciphertext, just in a different order. This means letter frequency analysis reveals nothing useful. The statistical signature of a transposition cipher is identical to plaintext: the index of coincidence remains ~0.065.",
    ciphers: [
      {
        id: "railFence",
        label: "Rail Fence",
        color: "#60a5fa",
        era: "19th century (American Civil War)",
        difficulty: "Beginner",
        keyType: "Number of rails (2–N)",
        example: { plain: "HELLO WORLD", key: "3 rails", cipher: "HOLELWRDLO" },
        how: "The plaintext is written diagonally across N horizontal rails in a zigzag pattern — descending until the bottom rail, then ascending back up, and so on. The ciphertext is produced by reading each rail from left to right, top rail first.",
        formula: "Zigzag assignment: rail direction flips at rail 0 and rail N-1",
        weakness: "Only as many keys as there are rail counts worth trying (typically 2–10). Each can be tested instantly. The distinctive index of coincidence (~0.065) immediately identifies it as a transposition cipher, narrowing the attack.",
        funFact: "The name comes from the zigzag pattern resembling a wooden split-rail fence. It was reportedly used by Confederate forces during the American Civil War — though military historians note it was never very secure even then.",
        tags: ["transposition", "geometric", "zigzag"],
      },
      {
        id: "columnar",
        label: "Columnar",
        color: "#818cf8",
        era: "World War I & II",
        difficulty: "Intermediate",
        keyType: "A keyword (determines column read order)",
        example: { plain: "HELLO WORLD", key: "CODE", cipher: "LWOEHORLLD" },
        how: "The plaintext is written row by row into a grid with as many columns as the keyword has letters. The columns are then read out in the alphabetical order of the keyword letters. If the keyword is CODE, column C (position 2 alphabetically) is read first, then D, then E, then O.",
        formula: "Column read order = sorted positions of keyword letters",
        weakness: "The number of columns is revealed by the ciphertext length factoring cleanly. Combined with a known-plaintext attack, the column order can be recovered. Double transposition (applying columnar twice with different keys) is significantly stronger.",
        funFact: "The German Army used double columnar transposition (ADFGVX cipher) extensively in World War I. French cryptanalyst Georges Painvin broke it in June 1918, allowing the Allies to intercept a critical supply order just before a major offensive.",
        tags: ["transposition", "keyword", "grid"],
      },
      {
        id: "route",
        label: "Route",
        color: "#c084fc",
        era: "19th century",
        difficulty: "Intermediate",
        keyType: "Route pattern (spiral, boustrophedon, etc.)",
        example: { plain: "HELLO WORLD", key: "clockwise spiral", cipher: "HLODEROLWL" },
        how: "The plaintext fills a rectangular grid row by row. It is then read out following a specific route through the grid — in this implementation, a clockwise spiral starting from the top-left corner, moving right across the top, down the right side, left across the bottom, up the left side, and inward.",
        formula: "Clockwise spiral: top→right→bottom→left, inward",
        weakness: "The route pattern must be known to both parties and provides no key flexibility in the basic form. However, combined with a second transposition or a substitution layer it becomes considerably harder to break.",
        funFact: "Route ciphers were popular with both sides during the American Civil War. The Union Army favored a variant where the route was chosen from a codebook, and decoy words were inserted at specific grid positions to further confuse interceptors.",
        tags: ["transposition", "geometric", "spiral"],
      },
    ],
  },
  {
    id: "encoding",
    label: "Encodings",
    color: "#f472b6",
    tagline: "Represent data in a different format — not truly secret",
    description: "Encodings are not ciphers in the cryptographic sense — they have no secret key and any trained eye can reverse them instantly. They transform data from one representation to another for practical reasons: transmitting binary over text channels, compact storage, or international standardization. Understanding them is essential in computing and cybersecurity.",
    ciphers: [
      {
        id: "morse",
        label: "Morse Code",
        color: "#f472b6",
        era: "1837, Samuel Morse",
        difficulty: "Beginner",
        keyType: "None (fixed international standard)",
        example: { plain: "SOS", key: "—", cipher: "... --- ..." },
        how: "Each letter and digit maps to a unique sequence of dots and dashes. Short signals are dots, long signals are dashes. Letters are separated by a short pause, words by a longer pause. The mapping was designed so common letters (E = ·, T = −) have short codes.",
        formula: "Lookup table: letter → dot/dash sequence",
        weakness: "Provides no security. The encoding table is universally known. Anyone who recognises dots and dashes — or sees a pattern of short and long characters — can decode it immediately.",
        funFact: "The SOS distress signal (···−−−···) was chosen in 1906 not because it stands for 'Save Our Ship' — that backronym came later. It was chosen purely because it's easy to send and impossible to mistake: three short, three long, three short.",
        tags: ["encoding", "telegraph", "no-security"],
      },
      {
        id: "binary",
        label: "Binary",
        color: "#fb7185",
        era: "Foundation of computing",
        difficulty: "Beginner",
        keyType: "None (ASCII standard)",
        example: { plain: "HI", key: "—", cipher: "01001000 01001001" },
        how: "Each character is converted to its ASCII code number, then that number is written in base 2 (binary). Standard ASCII uses 7 bits; extended ASCII and UTF-8 use 8 bits. The letter H has ASCII code 72, which in binary is 01001000.",
        formula: "char → ASCII decimal → 8-bit binary",
        weakness: "Provides zero security. Anyone who sees a string of 0s and 1s in groups of 8 will immediately recognise it as binary-encoded ASCII and decode it in seconds.",
        funFact: "The ASCII standard was published in 1963 and was designed to be compatible with telegraph systems. It includes 33 non-printing control characters like carriage return (13) and bell (7) — which would literally ring a bell on a teletype machine.",
        tags: ["encoding", "ASCII", "binary", "computing"],
      },
      {
        id: "hex",
        label: "Hexadecimal",
        color: "#fda4af",
        era: "Foundation of computing",
        difficulty: "Beginner",
        keyType: "None (ASCII standard)",
        example: { plain: "HI", key: "—", cipher: "48 49" },
        how: "Each character is converted to its ASCII code number, then written in base 16 (hexadecimal). Base 16 uses digits 0–9 and letters A–F. Each character takes exactly two hex digits. H is ASCII 72, which in hex is 48. I is ASCII 73, which is 49.",
        formula: "char → ASCII decimal → 2-digit hex",
        weakness: "No security. Hex is instantly recognisable by its character set (0-9, A-F) and the fact that printable ASCII always produces values between 20 and 7E. Any programmer will decode it on sight.",
        funFact: "Hexadecimal is used everywhere in computing because 1 hex digit represents exactly 4 bits (a nibble), so 2 hex digits represent exactly 1 byte. This is why memory addresses, color codes (#FF5733), and file hashes are all written in hex.",
        tags: ["encoding", "ASCII", "hexadecimal", "computing"],
      },
      {
        id: "base64",
        label: "Base64",
        color: "#fca5a5",
        era: "1987, MIME email standard",
        difficulty: "Intermediate",
        keyType: "None (RFC 4648 standard)",
        example: { plain: "HI!", key: "—", cipher: "SEkh" },
        how: "Base64 takes 3 bytes (24 bits) of input at a time and splits them into four 6-bit groups. Each 6-bit value (0–63) maps to a character from the Base64 alphabet: A–Z, a–z, 0–9, +, /. If the input isn't divisible by 3, padding characters (=) are added. The result is always 4/3 the size of the input.",
        formula: "3 bytes → 4 × 6-bit values → 4 Base64 characters",
        weakness: "No security at all — but it's frequently misunderstood as encryption by beginners. In web development you'll see it constantly: image data URIs, JWT tokens, HTTP Basic Auth headers, and email attachments all use Base64.",
        funFact: "Base64 was invented to solve a specific problem: early email systems could only transmit 7-bit ASCII text, but attachments contain arbitrary binary data. Base64 converts binary to a safe ASCII-only representation that survived the 7-bit limitation.",
        tags: ["encoding", "MIME", "web", "binary-safe"],
      },
    ],
  },
];



export default function CipherLibrary() {
  const [openMap, setOpenMap] = useState({});
  const [activeFamily, setActiveFamily] = useState("substitution");
  const [scrollPct, setScrollPct] = useState(0);

  // scroll progress bar + active family tracking
  useEffect(() => {
    const handler = () => {
      const el = document.documentElement;
      const pct = (el.scrollTop / (el.scrollHeight - el.clientHeight)) * 100;
      setScrollPct(pct);

      // find which family section is in view
      for (const fam of FAMILIES) {
        const el2 = document.getElementById(`fam-${fam.id}`);
        if (el2) {
          const rect = el2.getBoundingClientRect();
          if (rect.top <= 120) setActiveFamily(fam.id);
        }
      }
    };
    window.addEventListener("scroll", handler, { passive: true });
    return () => window.removeEventListener("scroll", handler);
  }, []);

  // open cipher if URL hash matches on load
  useEffect(() => {
    const hash = window.location.hash.replace("#", "");
    if (hash) {
      setOpenMap(m => ({ ...m, [hash]: true }));
      setTimeout(() => {
        const el = document.getElementById(hash);
        if (el) el.scrollIntoView({ behavior: "smooth", block: "start" });
      }, 100);
    }
  }, []);

  const toggle = (id) => setOpenMap(m => ({ ...m, [id]: !m[id] }));

  const scrollToFamily = (famId) => {
    const el = document.getElementById(`fam-${famId}`);
    if (el) el.scrollIntoView({ behavior: "smooth", block: "start" });
  };

  return (
    <>
      {/* scroll progress */}
      <div className="cl-scroll-bar" style={{ width: `${scrollPct}%` }} />

      <div className="cl">
        {/* HERO */}
        <div className="cl-hero">
          <div className="cl-hero-tag">// cipher library</div>
          <h1>THE COMPLETE GUIDE TO<br/><span>CLASSICAL CIPHERS</span></h1>
          <p className="cl-hero-desc">
            14 cipher types across 4 families — their history, mechanics, mathematical
            foundations, and cryptographic weaknesses.
          </p>
        </div>

        {/* STICKY FAMILY NAV */}
        <nav className="cl-nav">
          <div className="cl-nav-inner">
            {FAMILIES.map(f => (
              <button key={f.id}
                className={`cl-nav-btn ${activeFamily===f.id?"act":""}`}
                style={{ "--nc": f.color, "--nb": f.color + "15" }}
                onClick={() => scrollToFamily(f.id)}>
                {f.label.toUpperCase()}
              </button>
            ))}
          </div>
        </nav>

        {/* FAMILY SECTIONS */}
        {FAMILIES.map(fam => (
          <section key={fam.id} id={`fam-${fam.id}`} className="cl-family"
            style={{ "--fc": fam.color, "--fb": fam.color + "12" }}>
            <div className="cl-fam-header">
              <div className="cl-fam-eyebrow">// {fam.ciphers.length} cipher types</div>
              <div className="cl-fam-title">{fam.label}</div>
              <div className="cl-fam-tagline">{fam.tagline}</div>
              <div className="cl-fam-desc">{fam.description}</div>
            </div>

            {fam.ciphers.map(cipher => (
              <CipherCard
                key={cipher.id}
                cipher={cipher}
                familyColor={fam.color}
                isOpen={!!openMap[cipher.id]}
                onToggle={() => toggle(cipher.id)}
              />
            ))}
          </section>
        ))}

        <div className="cl-bottom" />
      </div>
    </>
  );
}