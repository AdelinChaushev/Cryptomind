import { useState, useEffect, useRef, useCallback, useMemo } from "react";
import "../styles/cipher-tool.css";
// ─── CONSTANTS ───────────────────────────────────────────────────────────────
const ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
const SIMPLE_SUB_KEY = "QWERTYUIOPASDFGHJKLZXCVBNM"; // fixed educational mapping

const MORSE = {
  A:".-",B:"-...",C:"-.-.",D:"-..",E:".",F:"..-.",G:"--.",H:"....",I:"..",J:".---",
  K:"-.-",L:".-..",M:"--",N:"-.",O:"---",P:".--.",Q:"--.-",R:".-.",S:"...",T:"-",
  U:"..-",V:"...-",W:".--",X:"-..-",Y:"-.--",Z:"--..",
  "0":"-----","1":".----","2":"..---","3":"...--","4":"....-","5":".....",
  "6":"-....","7":"--...","8":"---..","9":"----."," ":"/"
};

const PRESETS = [
  { label:"HELLO WORLD",         text:"HELLO WORLD"         },
  { label:"THE QUICK BROWN FOX", text:"THE QUICK BROWN FOX" },
  { label:"CRYPTOGRAPHY",        text:"CRYPTOGRAPHY"        },
  { label:"ATTACK AT DAWN",      text:"ATTACK AT DAWN"      },
  { label:"SECRET MESSAGE",      text:"SECRET MESSAGE"      },
];

const FAMILIES = [
  {
    id:"substitution", label:"Substitution",
    ciphers:[
      { id:"caesar",            label:"Caesar",             color:"#fbbf24", desc:"Shift each letter by a fixed amount",     tip:"Each letter shifts forward N positions in the alphabet. Only 25 possible keys — the simplest cipher to break."  },
      { id:"rot13",             label:"ROT13",              color:"#22d3ee", desc:"Caesar locked at shift 13",               tip:"A Caesar cipher permanently fixed at shift 13. Applying it twice returns the original — encryption and decryption are identical." },
      { id:"atbash",            label:"Atbash",             color:"#a78bfa", desc:"Mirror the alphabet (A↔Z)",              tip:"A mirror mapping: A→Z, B→Y, C→X… The oldest known cipher, used in the Hebrew Bible around 600 BC." },
      { id:"simpleSubstitution",label:"SimpleSubstitution", color:"#fb923c", desc:"Each letter maps to a fixed replacement",tip:"A scrambled alphabet replaces each letter. 26! possible keys, but frequency analysis breaks it in minutes." },
    ]
  },
  {
    id:"polyalphabetic", label:"Polyalphabetic",
    ciphers:[
      { id:"vigenere",  label:"Vigenère",  color:"#10b981", desc:"Keyword determines each letter's shift",       tip:"A repeated keyword gives each position a different Caesar shift, flattening frequency patterns. Called 'indecipherable' for 300 years." },
      { id:"autokey",   label:"Autokey",   color:"#34d399", desc:"Key primer, then plaintext extends the key",   tip:"Starts with a keyword primer, then the plaintext itself becomes the key. Never repeats, defeating the Kasiski test." },
      { id:"trithemius",label:"Trithemius",color:"#6ee7b7", desc:"Shift increases by 1 at each position",       tip:"No configurable key — shift simply increments by 1 at each letter position, wrapping at 26. Described in the first printed cryptography book (1508)." },
    ]
  },
  {
    id:"transposition", label:"Transposition",
    ciphers:[
      { id:"railFence",label:"Rail Fence",color:"#60a5fa", desc:"Zigzag across N rails, read row by row",        tip:"Letters are written diagonally across N rails in a zigzag, then read row by row. Letter frequencies are preserved — identical to plaintext." },
      { id:"columnar", label:"Columnar",  color:"#818cf8", desc:"Fill columns, read them in keyword order",      tip:"Text fills a grid row by row. Columns are read out in alphabetical order of the keyword letters. Used by both sides in WWI and WWII." },
      { id:"route",    label:"Route",     color:"#c084fc", desc:"Fill a grid, read in a clockwise spiral path",  tip:"Text fills a grid, then is read following a route — here a clockwise spiral. Favored in the American Civil War with decoy words inserted into the grid." },
    ]
  },
  {
    id:"encoding", label:"Encoding",
    ciphers:[
      { id:"morse",  label:"Morse",  color:"#f472b6", desc:"Dots and dashes representing each letter",  tip:"Not a cipher — a standardized signal encoding. Common letters get short codes (E = ·). Provides zero security." },
      { id:"binary", label:"Binary", color:"#fb7185", desc:"8-bit binary code per character",           tip:"Each character's ASCII code written in base 2. H = ASCII 72 = 01001000. Universally recognisable — no security." },
      { id:"hex",    label:"Hex",    color:"#fda4af", desc:"Two hex digits per character",              tip:"Each character's ASCII code in base 16. H = 48, I = 49. Used everywhere in computing for memory addresses and color codes." },
      { id:"base64", label:"Base64", color:"#fca5a5", desc:"3-byte groups encoded as 4 ASCII chars",    tip:"Converts binary data to safe ASCII text. Takes 3 bytes → 4 characters. Used in email attachments, JWT tokens, and image data URIs." },
    ]
  },
];

// ─── CIPHER ENGINES ──────────────────────────────────────────────────────────
function isLetter(ch) { return /[A-Za-z]/.test(ch); }
function ai(ch) { return ALPHA.indexOf(ch.toUpperCase()); }

// Sub step object
function ss(ch, out, formula, keyInfo = null) {
  return { mode:"sub", in:ch.toUpperCase(), out, formula, keyInfo, letter:isLetter(ch) };
}

function stepCaesar(ch, shift) {
  const i = ai(ch); if (i===-1) return ss(ch,ch,null);
  const o=(i+shift+26)%26;
  return ss(ch,ALPHA[o],`pos(${ch.toUpperCase()}) = ${i}  →  (${i} + ${shift}) mod 26 = ${o}  →  ${ALPHA[o]}`);
}
function stepAtbash(ch) {
  const i=ai(ch); if(i===-1) return ss(ch,ch,null);
  return ss(ch,ALPHA[25-i],`pos(${ch.toUpperCase()}) = ${i}  →  25 − ${i} = ${25-i}  →  ${ALPHA[25-i]}`);
}
function stepSimpleSub(ch) {
  const i=ai(ch); if(i===-1) return ss(ch,ch,null);
  return ss(ch,SIMPLE_SUB_KEY[i],`'${ch.toUpperCase()}' at position ${i}  →  mapped to  '${SIMPLE_SUB_KEY[i]}'`);
}
function stepVigenere(ch, keyCh, pos) {
  const i=ai(ch), k=ai(keyCh); if(i===-1) return ss(ch,ch,null);
  const o=(i+k)%26;
  return ss(ch,ALPHA[o],
    `pos(${ch.toUpperCase()}) = ${i}  +  key[${pos}](${keyCh.toUpperCase()}) = ${k}  →  (${i}+${k}) mod 26 = ${o}  →  ${ALPHA[o]}`,
    {keyChar:keyCh.toUpperCase(), keyPos:pos});
}
function stepTrithemius(ch, pos) {
  const i=ai(ch); if(i===-1) return ss(ch,ch,null);
  const shift=pos%26, o=(i+shift)%26;
  return ss(ch,ALPHA[o],
    `pos(${ch.toUpperCase()}) = ${i}  +  shift[${pos}] = ${shift}  →  (${i}+${shift}) mod 26 = ${o}  →  ${ALPHA[o]}`,
    {shift, pos});
}

// ─── BUILD VIZ DATA ──────────────────────────────────────────────────────────
function buildVizData(text, cipher, params) {
  const T = text.toUpperCase();

  // ── SUBSTITUTION ──
  if (["caesar","rot13","atbash","simpleSubstitution"].includes(cipher)) {
    const steps = T.split("").map(ch => {
      if (cipher==="caesar")            return stepCaesar(ch, params.shift);
      if (cipher==="rot13")             return stepCaesar(ch, 13);
      if (cipher==="atbash")            return stepAtbash(ch);
      if (cipher==="simpleSubstitution")return stepSimpleSub(ch);
    });
    return { type:"sub", steps };
  }

  // ── POLYALPHABETIC ──
  if (["vigenere","autokey","trithemius"].includes(cipher)) {
    const kw = (params.keyword||"KEY").replace(/[^A-Za-z]/g,"").toUpperCase()||"KEY";
    const steps=[];
    let lIdx=0;
    let akExt=""; // autokey: plaintext letters seen so far
    for (const ch of T) {
      if (cipher==="trithemius") {
        if (isLetter(ch)) { steps.push(stepTrithemius(ch,lIdx)); lIdx++; }
        else steps.push(ss(ch,ch,null));
        continue;
      }
      if (!isLetter(ch)) { steps.push(ss(ch,ch,null)); continue; }
      let keyCh;
      if (cipher==="vigenere") {
        keyCh = kw[lIdx % kw.length];
      } else {
        // autokey: primer + plaintext so far
        const fullKey = kw + akExt;
        keyCh = lIdx < fullKey.length ? fullKey[lIdx] : kw[lIdx % kw.length];
      }
      const s = stepVigenere(ch, keyCh, lIdx);
      if (cipher==="autokey") akExt += ch;
      steps.push({ ...s, autokeySource: cipher==="autokey" && lIdx >= kw.length ? "plaintext" : "keyword" });
      lIdx++;
    }
    return { type:"poly", steps, keyword:kw };
  }

  // ── RAIL FENCE ──
  if (cipher==="railFence") {
    const rails = params.rails||3;
    const assign=[];
    let rail=0, dir=1;
    for (let i=0;i<T.length;i++) {
      assign.push({ char:T[i], origPos:i, rail, col:i });
      if (rail===0) dir=1;
      else if (rail===rails-1) dir=-1;
      rail+=dir;
    }
    const grid=Array.from({length:rails},()=>[]);
    for (const a of assign) grid[a.rail].push(a);
    const readOrder=grid.flat();
    return { type:"trans", subtype:"railFence", grid, readOrder, totalCols:T.length, rails };
  }

  // ── COLUMNAR ──
  if (cipher==="columnar") {
    const kw=(params.colKey||params.keyword||"CODE").replace(/[^A-Za-z]/g,"").toUpperCase().slice(0,8)||"CODE";
    const cols=kw.length, rows=Math.ceil(T.length/cols);
    const grid2d=[];
    for (let r=0;r<rows;r++) {
      const row=[];
      for (let c=0;c<cols;c++) {
        const idx=r*cols+c;
        row.push({ char:idx<T.length?T[idx]:null, origPos:idx, row:r, col:c });
      }
      grid2d.push(row);
    }
    const colOrder=[...kw].map((ch,i)=>({ch,i}))
      .sort((a,b)=>a.ch<b.ch?-1:a.ch>b.ch?1:a.i-b.i).map(x=>x.i);
    const readOrder=[];
    for (const c of colOrder)
      for (let r=0;r<rows;r++) { const cell=grid2d[r][c]; if(cell.char!==null) readOrder.push(cell); }
    return { type:"trans", subtype:"columnar", grid:grid2d, readOrder, cols, rows, keyword:kw, colOrder };
  }

  // ── ROUTE ──
  if (cipher==="route") {
    const len=T.length;
    const cols=Math.ceil(Math.sqrt(len)), rows=Math.ceil(len/cols);
    const grid2d=[];
    for (let r=0;r<rows;r++) {
      const row=[];
      for (let c=0;c<cols;c++) {
        const idx=r*cols+c;
        row.push({ char:idx<len?T[idx]:null, origPos:idx, row:r, col:c });
      }
      grid2d.push(row);
    }
    // clockwise spiral
    const readOrder=[];
    let top=0,bottom=rows-1,left=0,right=cols-1;
    while(top<=bottom&&left<=right) {
      for(let c=left;c<=right;c++)          if(grid2d[top][c].char!==null) readOrder.push(grid2d[top][c]); top++;
      for(let r=top;r<=bottom;r++)           if(grid2d[r][right].char!==null) readOrder.push(grid2d[r][right]); right--;
      if(top<=bottom) for(let c=right;c>=left;c--) if(grid2d[bottom][c].char!==null) readOrder.push(grid2d[bottom][c]); bottom--;
      if(left<=right) for(let r=bottom;r>=top;r--) if(grid2d[r][left].char!==null) readOrder.push(grid2d[r][left]); left++;
    }
    return { type:"trans", subtype:"route", grid:grid2d, readOrder, cols, rows };
  }

  // ── ENCODING ──
  if (cipher==="morse")
    return { type:"encode", subtype:"morse",
      steps:T.split("").map(ch=>({ in:ch, code:MORSE[ch]??"?", label:ch===" "?"(word break)":null })) };
  if (cipher==="binary")
    return { type:"encode", subtype:"binary",
      steps:T.split("").map(ch=>({ in:ch, code:ch.charCodeAt(0).toString(2).padStart(8,"0"), label:`ASCII ${ch.charCodeAt(0)}` })) };
  if (cipher==="hex")
    return { type:"encode", subtype:"hex",
      steps:T.split("").map(ch=>({ in:ch, code:ch.charCodeAt(0).toString(16).toUpperCase().padStart(2,"0"), label:`ASCII ${ch.charCodeAt(0)}` })) };
  if (cipher==="base64") {
    const groups=[];
    for(let i=0;i<T.length;i+=3) {
      const chunk=T.slice(i,i+3);
      groups.push({ in:chunk, code:btoa(chunk), start:i, end:Math.min(i+3,T.length) });
    }
    return { type:"encode", subtype:"base64", steps:groups };
  }

  return { type:"sub", steps:[] };
}

// ─── THEME VARS ───────────────────────────────────────────────────────────────
const BG0="#020617",BG1="#0c1428",BG2="#1e293b",BGI="#0a1120";
const T1="#f8fafc",T2="#cbd5e1",T3="#94a3b8",TD="#475569";
const EM="#10b981",EMD="rgba(16,185,129,0.12)";
const BD="rgba(203,213,225,0.08)",BDH="rgba(203,213,225,0.15)",BDY="rgba(251,191,36,0.28)";
const MONO="'Space Mono',monospace",BODY="'Sora',sans-serif",DISP="'Orbitron',monospace";

// ─── CSS ─────────────────────────────────────────────────────────────────────

// ─── VISUALIZATION COMPONENTS ─────────────────────────────────────────────────

function LetterRow({ steps, cur, mode }) {
  // mode: 'plain' | 'cipher'
  return (
    <div className="ct-letter-row">
      {steps.map((s, i) => {
        const ch   = mode === "plain" ? s.in : s.out;
        const isSp = ch === " " || ch === "/";
        const isAct = i === cur;
        const plain_done  = mode === "plain"  && !isAct && i < cur;
        const cipher_done = mode === "cipher" && i <= cur;
        return (
          <div key={i} className={`ct-lb ${isSp?"spc":""} ${isAct?"act":""} ${!isAct&&plain_done?"done":""} ${!isAct&&cipher_done?"comp":""}`}>
            {mode==="cipher" ? (i <= cur ? ch : "") : ch}
          </div>
        );
      })}
    </div>
  );
}

function TransformCard({ step }) {
  if (!step) return null;
  if (!step.letter) return (
    <div className="ct-tc" style={{ boxShadow:"none", border:`1px solid rgba(203,213,225,0.08)` }}>
      <div className="ct-tc-letters">
        <span className="ct-big" style={{color:TD}}>{step.in}</span>
        <span className="ct-arrow">→</span>
        <span className="ct-big" style={{color:TD}}>{step.out}</span>
      </div>
      <div className="ct-formula" style={{color:TD}}>non-letter — passed through unchanged</div>
    </div>
  );
  return (
    <div className="ct-tc">
      {step.keyInfo && (
        <div className="ct-key-badge">
          {step.keyInfo.keyChar && `KEY LETTER: ${step.keyInfo.keyChar}`}
          {step.keyInfo.shift !== undefined && `SHIFT: ${step.keyInfo.shift}  (position ${step.keyInfo.pos})`}
          {step.autokeySource && <span style={{opacity:.7,marginLeft:5}}>({step.autokeySource})</span>}
        </div>
      )}
      <div className="ct-tc-letters">
        <span className="ct-big" style={{color:"var(--cc)"}}>{step.in}</span>
        <span className="ct-arrow">→</span>
        <span className="ct-big" style={{color:EM}}>{step.out}</span>
      </div>
      <div className="ct-formula">{step.formula}</div>
    </div>
  );
}

function AlphabetRuler({ cipher, shift, cur, steps }) {
  const s   = cur >= 0 && cur < steps.length ? steps[cur] : null;
  const phi = s ? ai(s.in)  : -1;   // plain highlight index
  const chi = s ? ai(s.out) : -1;   // cipher highlight index

  if (cipher === "simpleSubstitution") {
    return (
      <div className="ct-ruler">
        <div className="ct-ruler-inner">
          <div className="ct-ruler-tag">PLAIN</div>
          <div className="ct-ruler-row">
            {ALPHA.split("").map((l,i)=><div key={i} className={`ct-ac ${i===phi?"phi":""}`}>{l}</div>)}
          </div>
          <div className="ct-ruler-row">
            {SIMPLE_SUB_KEY.split("").map((l,i)=>{
              const isHi = s && s.letter && SIMPLE_SUB_KEY[phi]===l && i===phi;
              return <div key={i} className={`ct-ac ${isHi?"chi":""}`}>{l}</div>;
            })}
          </div>
          <div className="ct-ruler-tag">CIPHER (fixed mapping)</div>
        </div>
      </div>
    );
  }

  const cipherAlpha = ALPHA.split("").map((_,i) => {
    if (cipher==="caesar") return ALPHA[(i+shift+26)%26];
    if (cipher==="rot13")  return ALPHA[(i+13)%26];
    if (cipher==="atbash") return ALPHA[25-i];
    return ALPHA[i];
  });

  return (
    <div className="ct-ruler">
      <div className="ct-ruler-inner">
        <div className="ct-ruler-tag">PLAIN ALPHABET</div>
        <div className="ct-ruler-row">
          {ALPHA.split("").map((l,i)=><div key={i} className={`ct-ac ${i===phi?"phi":""}`}>{l}</div>)}
        </div>
        <div className="ct-ruler-row">
          {ALPHA.split("").map((_,i)=>(
            <div key={i} className={`ct-ruler-arrow ${i===phi?"on":""}`}>{i===phi?"↓":"·"}</div>
          ))}
        </div>
        <div className="ct-ruler-row">
          {cipherAlpha.map((l,i)=><div key={i} className={`ct-ac ${i===phi?"chi":""}`}>{l}</div>)}
        </div>
        <div className="ct-ruler-tag">CIPHER ALPHABET (shift {cipher==="rot13"?13:cipher==="atbash"?"mirror":shift})</div>
      </div>
    </div>
  );
}

function RailFenceViz({ meta, cur }) {
  const { grid, readOrder, totalCols, rails } = meta;
  const active = cur>=0 && cur<readOrder.length ? readOrder[cur] : null;
  const usedSet = new Set(readOrder.slice(0,cur).map(c=>`${c.rail}-${c.col}`));
  return (
    <div className="ct-trans-wrap">
      {grid.map((railCells,r)=>(
        <div key={r} className="ct-rail-row">
          <div className="ct-rail-label">Rail {r}</div>
          <div style={{display:"flex",gap:5}}>
            {Array.from({length:totalCols},(_,c)=>{
              const cell=railCells.find(x=>x.col===c);
              if(!cell) return <div key={c} className="ct-gc empty" style={{background:"transparent",border:"1px dashed rgba(203,213,225,0.04)"}}>·</div>;
              const isAct=active&&active.col===c&&active.rail===r;
              const isUsed=usedSet.has(`${r}-${c}`);
              const order=readOrder.findIndex(x=>x.col===c&&x.rail===r);
              return (
                <div key={c} className={`ct-gc ${isAct?"act":""} ${!isAct&&isUsed?"used":""}`}>
                  {cell.char}
                  <span className="ct-gc-num">{order+1}</span>
                </div>
              );
            })}
          </div>
        </div>
      ))}
      <div style={{marginTop:20}}>
        <div className="ct-row-tag">// output (read each rail left to right)</div>
        <div className="ct-letter-row">
          {readOrder.map((cell,i)=>(
            <div key={i} className={`ct-lb ${i===cur?"act":i<cur?"comp":""}`}>{i<=cur?cell.char:""}</div>
          ))}
        </div>
      </div>
    </div>
  );
}

function ColumnarViz({ meta, cur }) {
  const { grid, readOrder, cols, keyword, colOrder } = meta;
  const active = cur>=0 && cur<readOrder.length ? readOrder[cur] : null;
  const usedSet = new Set(readOrder.slice(0,cur).map(c=>`${c.row}-${c.col}`));
  const activeCol = active?.col ?? -1;
  return (
    <div className="ct-trans-wrap">
      <div className="ct-col-hdr">
        {keyword.split("").map((ch,c)=>(
          <div key={c} className={`ct-col-hdr-cell ${c===activeCol?"a":""}`}>
            {ch}{c===activeCol?" ▼":""}
          </div>
        ))}
      </div>
      <div className="ct-grid-outer">
        <div className="ct-grid" style={{gridTemplateColumns:`repeat(${cols},42px)`}}>
          {grid.flat().map((cell,idx)=>{
            const isAct=active&&cell.row===active.row&&cell.col===active.col;
            const isUsed=usedSet.has(`${cell.row}-${cell.col}`);
            const order=readOrder.findIndex(x=>x.row===cell.row&&x.col===cell.col);
            return (
              <div key={idx} className={`ct-gc ${cell.char===null?"empty":""} ${isAct?"act":""} ${!isAct&&isUsed?"used":""}`}>
                {cell.char??"·"}
                {cell.char&&<span className="ct-gc-num">{order+1}</span>}
              </div>
            );
          })}
        </div>
      </div>
      <div style={{marginTop:16}}>
        <div className="ct-row-tag">// output (columns read in alphabetical key order)</div>
        <div className="ct-letter-row">
          {readOrder.map((cell,i)=>(
            <div key={i} className={`ct-lb ${i===cur?"act":i<cur?"comp":""}`}>{i<=cur?cell.char:""}</div>
          ))}
        </div>
      </div>
    </div>
  );
}

function RouteViz({ meta, cur }) {
  const { grid, readOrder, cols } = meta;
  const active = cur>=0 && cur<readOrder.length ? readOrder[cur] : null;
  const usedSet = new Set(readOrder.slice(0,cur).map(c=>`${c.row}-${c.col}`));
  return (
    <div className="ct-trans-wrap">
      <div className="ct-grid-outer">
        <div className="ct-grid" style={{gridTemplateColumns:`repeat(${cols},42px)`}}>
          {grid.flat().map((cell,idx)=>{
            const isAct=active&&cell.row===active.row&&cell.col===active.col;
            const isUsed=usedSet.has(`${cell.row}-${cell.col}`);
            const order=readOrder.findIndex(x=>x.row===cell.row&&x.col===cell.col);
            return (
              <div key={idx} className={`ct-gc ${cell.char===null?"empty":""} ${isAct?"act":""} ${!isAct&&isUsed?"used":""}`}>
                {cell.char??"·"}
                {cell.char&&<span className="ct-gc-num">{order+1}</span>}
              </div>
            );
          })}
        </div>
      </div>
      <div style={{marginTop:16}}>
        <div className="ct-row-tag">// output (clockwise spiral reading order)</div>
        <div className="ct-letter-row">
          {readOrder.map((cell,i)=>(
            <div key={i} className={`ct-lb ${i===cur?"act":i<cur?"comp":""}`}>{i<=cur?cell.char:""}</div>
          ))}
        </div>
      </div>
    </div>
  );
}

function EncodingViz({ meta, cur }) {
  const { steps, subtype } = meta;
  const step = cur>=0 && cur<steps.length ? steps[cur] : null;
  const sep  = subtype==="morse" ? "  " : " ";
  const outSoFar = steps.slice(0,cur+1).map(s=>s.code).join(sep);
  return (
    <div className="ct-enc-wrap">
      <div className="ct-row-tag">// input</div>
      <div className="ct-letter-row" style={{marginBottom:16}}>
        {steps.map((s,i)=>(
          <div key={i} className={`ct-lb ${i===cur?"act":i<cur?"done":""}`}
            style={{width:subtype==="base64"?Math.max(36,s.in.length*13):36}}>
            {s.in}
          </div>
        ))}
      </div>
      <div className="ct-tz">
        {step ? (
          <div className="ct-enc-card">
            <div className="ct-enc-big">{step.in}</div>
            <div className="ct-enc-down">↓</div>
            <div className="ct-enc-code">{step.code}</div>
            {step.label && <div className="ct-enc-sub">{step.label}</div>}
          </div>
        ) : (
          <div className="ct-idle">{cur<0?"press PLAY or NEXT to begin →":"encoding complete"}</div>
        )}
      </div>
      {cur>=0 && (
        <>
          <div className="ct-row-tag">// encoded output</div>
          <div className="ct-enc-out">{outSoFar}</div>
        </>
      )}
    </div>
  );
}

// ─── MAIN ────────────────────────────────────────────────────────────────────
export default function CipherTool() {
  const [family,   setFamily]   = useState("substitution");
  const [cipher,   setCipher]   = useState("caesar");
  const [presetIdx,setPresetIdx]= useState(0);
  const [shift,    setShift]    = useState(3);
  const [keyword,  setKeyword]  = useState("KEY");
  const [colKey,   setColKey]   = useState("CODE");
  const [rails,    setRails]    = useState(3);
  const [cur,      setCur]      = useState(-1);
  const [playing,  setPlaying]  = useState(false);
  const [speed,    setSpeed]    = useState(700);
  const timer = useRef(null);

  const text    = PRESETS[presetIdx].text;
  const famDef  = FAMILIES.find(f=>f.id===family);
  const cipDef  = FAMILIES.flatMap(f=>f.ciphers).find(c=>c.id===cipher);
  const params  = { shift, keyword, colKey, rails };

  const viz = useMemo(()=>buildVizData(text,cipher,params),[text,cipher,shift,keyword,colKey,rails]);
  const total = viz.type==="trans" ? viz.readOrder.length : (viz.steps?.length??0);

  useEffect(()=>{
    if(playing){
      timer.current=setInterval(()=>{
        setCur(c=>{ if(c+1>=total){ setPlaying(false); return total-1; } return c+1; });
      },speed);
    }
    return ()=>clearInterval(timer.current);
  },[playing,speed,total]);

  const reset    = useCallback(()=>{ setPlaying(false); setCur(-1); },[]);
  const fwd      = useCallback(()=>setCur(c=>Math.min(c+1,total-1)),[total]);
  const bck      = useCallback(()=>setCur(c=>Math.max(c-1,-1)),[]);
  const toggle   = useCallback(()=>{
    if(cur>=total-1){ setCur(-1); setPlaying(true); }
    else setPlaying(p=>!p);
  },[cur,total]);

  useEffect(()=>reset(),[presetIdx,cipher,shift,keyword,colKey,rails]);

  const cc = cipDef?.color??"#fbbf24";
  const cb = cc+"18";
  const cg = cc+"40";
  const prog = cur<0?0:((cur+1)/total)*100;
  const done = cur===total-1&&total>0;

  const doneOutput = useMemo(()=>{
    if(!done) return "";
    if(viz.type==="trans")  return viz.readOrder.map(c=>c.char).join("");
    if(viz.type==="encode") return viz.steps.map(s=>s.code).join(viz.subtype==="morse"?"  ":" ");
    return viz.steps?.map(s=>s.out).join("")??"";
  },[done,viz]);

  const curStep = (viz.type==="sub"||viz.type==="poly") && cur>=0 && cur<viz.steps.length ? viz.steps[cur] : null;

  return (
    <>
      <div className="ct" style={{"--cc":cc,"--cb":cb,"--cg":cg}}>

        {/* HEADER */}
        <div className="ct-hdr">
          <div className="ct-hdr-tag">// interactive cipher tool</div>
          <h1>ENCRYPTION STEP-BY-STEP</h1>
          <p>Watch how each character transforms through the cipher — one step at a time.</p>
        </div>

        {/* CIPHER PICKER */}
        <div className="ct-panel">
          <div className="ct-ptag">// 01 — select cipher</div>
          <div className="ct-fam-tabs">
            {FAMILIES.map(f=>{
              const fc=f.ciphers[0].color;
              return (
                <button key={f.id} className={`ct-fam-tab ${family===f.id?"act":""}`}
                  style={{"--cc":fc,"--cb":fc+"18"}}
                  onClick={()=>{ setFamily(f.id); setCipher(f.ciphers[0].id); }}>
                  {f.label.toUpperCase()}
                </button>
              );
            })}
          </div>
          <div className="ct-cipher-grid">
            {famDef?.ciphers.map(c=>(
              <div key={c.id} className="ct-cb-wrap" style={{"--cc":c.color}}>
                {/* tooltip */}
                <div className="ct-cb-tip">{c.tip}</div>
                <button className={`ct-cb ${cipher===c.id?"act":""}`}
                  style={{"--cc":c.color,"--cb":c.color+"18"}}
                  onClick={()=>setCipher(c.id)}>
                  <div className="ct-cb-name">{c.label}</div>
                  <div className="ct-cb-desc">{c.desc}</div>
                  <div className="ct-cb-footer">
                    <a className="ct-learn"
                      href={`/cipher-library#${c.id}`}
                      onClick={e=>e.stopPropagation()}>
                      Learn more →
                    </a>
                  </div>
                </button>
              </div>
            ))}
          </div>
        </div>

        {/* CONFIGURE */}
        <div className="ct-panel">
          <div className="ct-ptag">// 02 — configure</div>
          <div className="ct-ctrl-row">
            <div className="ct-field">
              <span className="ct-lbl">Preset text</span>
              <select className="ct-sel" value={presetIdx} onChange={e=>setPresetIdx(+e.target.value)}>
                {PRESETS.map((p,i)=><option key={i} value={i}>{p.label}</option>)}
              </select>
            </div>

            {cipher==="caesar" && (
              <div className="ct-field">
                <span className="ct-lbl">Shift</span>
                <div className="ct-shift-row">
                  <input type="range" min={1} max={25} value={shift} onChange={e=>setShift(+e.target.value)}/>
                  <span className="ct-shift-num">{shift}</span>
                </div>
              </div>
            )}
            {(cipher==="vigenere"||cipher==="autokey") && (
              <div className="ct-field">
                <span className="ct-lbl">Keyword</span>
                <input className="ct-inp" value={keyword}
                  onChange={e=>setKeyword(e.target.value.replace(/[^A-Za-z]/g,"").toUpperCase().slice(0,10))}
                  maxLength={10} placeholder="KEY"/>
              </div>
            )}
            {cipher==="railFence" && (
              <div className="ct-field">
                <span className="ct-lbl">Rails</span>
                <div className="ct-shift-row">
                  <input type="range" min={2} max={5} value={rails} onChange={e=>setRails(+e.target.value)}/>
                  <span className="ct-shift-num">{rails}</span>
                </div>
              </div>
            )}
            {cipher==="columnar" && (
              <div className="ct-field">
                <span className="ct-lbl">Keyword</span>
                <input className="ct-inp" value={colKey}
                  onChange={e=>setColKey(e.target.value.replace(/[^A-Za-z]/g,"").toUpperCase().slice(0,8))}
                  maxLength={8} placeholder="CODE"/>
              </div>
            )}
            {["rot13","atbash","trithemius","simpleSubstitution","route","morse","binary","hex","base64"].includes(cipher) && (
              <div className="ct-field">
                <span className="ct-lbl">Parameters</span>
                <div style={{fontFamily:MONO,fontSize:12,color:cc,padding:"8px 0",letterSpacing:1}}>
                  {cipher==="rot13"             ?"Fixed shift: 13"
                  :cipher==="atbash"            ?"Rule: A ↔ Z, B ↔ Y…"
                  :cipher==="trithemius"        ?"Shift = letter position"
                  :cipher==="simpleSubstitution"?"Fixed 26-char mapping"
                  :cipher==="route"             ?"Clockwise spiral"
                  :"No parameters"}
                </div>
              </div>
            )}
          </div>
        </div>

        {/* PLAYBACK */}
        <div className="ct-panel">
          <div className="ct-ptag">// 03 — playback</div>
          <div className="ct-pb">
            <button className="ct-btn" onClick={bck}   disabled={cur<0}>◀ BACK</button>
            <button className="ct-btn pri" onClick={toggle}>
              {playing?"⏸ PAUSE":cur>=total-1?"↺ REPLAY":cur<0?"▶ PLAY":"▶ RESUME"}
            </button>
            <button className="ct-btn" onClick={fwd}   disabled={cur>=total-1}>NEXT ▶</button>
            <button className="ct-btn" onClick={reset}>↺ RESET</button>
            <div className="ct-prog"><div className="ct-prog-fill" style={{width:`${prog}%`}}/></div>
            <span className="ct-step-count">{cur+1} / {total}</span>
            <select className="ct-sel" style={{padding:"5px 8px",fontSize:11}} value={speed} onChange={e=>setSpeed(+e.target.value)}>
              <option value={1200}>SLOW</option>
              <option value={700}>NORMAL</option>
              <option value={350}>FAST</option>
              <option value={120}>TURBO</option>
            </select>
          </div>
        </div>

        {/* VISUALIZATION */}
        <div className="ct-viz">

          {/* SUB / POLY */}
          {(viz.type==="sub"||viz.type==="poly") && (<>
            <div className="ct-row-tag">// plaintext</div>
            <LetterRow steps={viz.steps} cur={cur} mode="plain"/>
            <div className="ct-tz">
              {cur<0 ? <div className="ct-idle">press PLAY or NEXT to begin →</div>
                     : <TransformCard step={curStep}/>}
            </div>
            <div className="ct-row-tag">// ciphertext</div>
            <LetterRow steps={viz.steps} cur={cur} mode="cipher"/>
            {viz.type==="sub" && cur>=0 && (
              <AlphabetRuler cipher={cipher} shift={shift} cur={cur} steps={viz.steps}/>
            )}
            {viz.type==="poly" && cur>=0 && cipher!=="trithemius" && (
              <div className="ct-panel" style={{marginTop:20}}>
                <div className="ct-ptag">// key schedule — {cipher==="autokey"?`primer: ${viz.keyword}  →  plaintext extends key`:`keyword: ${viz.keyword}  →  repeating`}</div>
                <div style={{display:"flex",gap:5,flexWrap:"wrap",justifyContent:"center"}}>
                  {viz.steps.filter(s=>s.letter).map((s,i)=>{
                    const lettersBefore = viz.steps.filter((x,xi)=>x.letter&&xi<=cur).length;
                    const filled = i < lettersBefore;
                    return (
                      <div key={i} style={{background:filled?EMD:BG2,border:`1px solid ${filled?"rgba(16,185,129,.3)":BD}`,borderRadius:6,padding:"5px 9px",textAlign:"center",minWidth:38}}>
                        <div style={{fontFamily:MONO,fontSize:10,color:T3}}>{s.in}</div>
                        <div style={{fontFamily:MONO,fontSize:10,color:EM}}>
                          {s.keyInfo?.keyChar??"?"}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}
          </>)}

          {/* TRANSPOSITION */}
          {viz.type==="trans" && (<>
            {cur<0 && <div className="ct-idle">press PLAY or NEXT — each step reveals one character being read from the grid</div>}
            {viz.subtype==="railFence" && <RailFenceViz meta={viz} cur={cur}/>}
            {viz.subtype==="columnar"  && <ColumnarViz  meta={viz} cur={cur}/>}
            {viz.subtype==="route"     && <RouteViz     meta={viz} cur={cur}/>}
          </>)}

          {/* ENCODING */}
          {viz.type==="encode" && <EncodingViz meta={viz} cur={cur}/>}

          {/* DONE BANNER */}
          {done && (
            <div className="ct-done">
              <div className="ct-done-tag">// encryption complete</div>
              <div className="ct-done-txt">{doneOutput}</div>
            </div>
          )}

        </div>
      </div>
    </>
  );
}