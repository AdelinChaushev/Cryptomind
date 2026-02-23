import { useState } from "react";
import "../styles/cipher-solve.css";
import CipherHeader from "../components/CipherHeader";
import CipherTextDisplay from "../components/CipherTextDisplay";
import SolveForm from "../components/SolveForm";
import AIAssistant from "../components/AIAssistant";
import CipherMeta from "../components/CipherMeta";
import ActivityLog from "../components/ActivityLog";
import { useEffect } from "react";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import ExperimentalCipherPanel from "../components/ExperimentalCipherPanel";

function CipherSolvePage() {
    const navigation = useNavigate();
    const [answer,    setAnswer]    = useState("");
    const [attempts,  setAttempts]  = useState(0);
    const [result,    setResult]    = useState(null);
    const [aiMode,    setAiMode]    = useState(-1);
    const [aiText,    setAiText]    = useState("");
    const [aiLoading, setAiLoading] = useState(false);
    const [expDecryptedText, setExpDecryptedText] = useState("");
    const [expDescription,   setExpDescription]   = useState("");

    function handleSubmit() {
        if (!answer.trim() || attempts >= MAX_ATTEMPTS) return;
        setAttempts(prev => prev + 1);
        const isCorrect = answer.trim().toUpperCase() === CORRECT_ANSWER;
        setResult(isCorrect ? "correct" : "incorrect");
    }
    const [cipher,setCipher] = useState({
        allowsAnswer: false,
        allowsFullSolution: false,
        allowsHint: false,
        allowsSolutionHint: false,
        allowsTypeHint: false,
        alreadySolved: false,
        challengeTypeDisplay: "",
        cipherText: "",
        fullSolutionUsed: false,
        imageBase64: null,
        isImage: false,
        points: 0,
        previousHints: [],
        solutionHintUsed: false,
        solvedUsersCount: 0,
        title: "",
        typeHintUsed: false,
        DateSubmitted: "",
        successRate: 0,
        solveCount: 0,
        successfulSubmissions: 0,
        recentSolvers: [],
        challengeTypeDisplay : ""

    });
    const { id } = useParams();
    useEffect(() => {
       axios.get(`http://localhost:5115/api/ciphers/cipher/${id}` , {withCredentials : true})
       .then(c => {
       const data = c.data;

      setCipher({
        allowsAnswer: data.allowsAnswer,
        allowsFullSolution: data.allowsFullSolution,
        allowsHint: data.allowsHint,
        allowsSolutionHint: data.allowsSolutionHint,
        allowsTypeHint: data.allowsTypeHint,
        alreadySolved: data.alreadySolved,
        challengeTypeDisplay: data.challengeTypeDisplay,
        cipherText: data.cipherText,
        fullSolutionUsed: data.fullSolutionUsed,
        imageBase64: data.imageBase64,
        isImage: data.isImage,
        points: data.points,
        previousHints: data.previousHints ?? [],
        solutionHintUsed: data.solutionHintUsed,
        solvedUsersCount: data.solvedUsersCount,
        title: data.title,
        typeHintUsed: data.typeHintUsed,
        dateSubmitted: data.dateSubmitted,
        successRate : data.successRate,
        successfulSubmissions : data.successfulSubmissions,
        totalAttempts : data.allSubmissions,
        recentSolvers : data.recentSolvers,
        challengeTypeDisplay : data.challengeTypeDisplay
      });
      console.log("API response:", data); })
       .catch(c => navigation("*") )
       .finally()
    },[id])
    
    handleSubmit = () => {
        //http://localhost:5115/api/ciphers/cipher/1/solve
        if (cipher.challengeTypeDisplay === "Standard") {  
        axios.post(`http://localhost:5115/api/ciphers/cipher/${id}/solve`,{
            UserSolution : answer
        }, {withCredentials : true})
        .then(c => {
            setResult(c.data ? "correct" :"incorrect") 
            console.log(c.data)})
        }
        else if (cipher.challengeTypeDisplay === "Experimental") {
            axios.post(`http://localhost:5115/api/ciphers/cipher/${id}/suggest-answer`,{
                description : expDescription,
                decryptedText : expDecryptedText
            }, {withCredentials : true})
            .then(c => {
                console.log(c.data)})
        }

    }
    const timeAgo = (dateString) => {
  if (!dateString) return "—";

  const date = new Date(dateString);

  if (isNaN(date.getTime())) {
    console.warn("Invalid date passed to timeAgo:", dateString);
    return "—";
  }

  const now = new Date();
  const seconds = Math.floor((now - date) / 1000);

  if (!Number.isFinite(seconds)) return "—";

  const rtf = new Intl.RelativeTimeFormat("en", { numeric: "auto" });

  if (seconds < 60) return rtf.format(-seconds, "second");

  const minutes = Math.floor(seconds / 60);
  if (minutes < 60) return rtf.format(-minutes, "minute");

  const hours = Math.floor(minutes / 60);
  if (hours < 24) return rtf.format(-hours, "hour");

  const days = Math.floor(hours / 24);
  if (days < 7) return rtf.format(-days, "day");

  const weeks = Math.floor(days / 7);
  if (weeks < 4) return rtf.format(-weeks, "week");

  const months = Math.floor(days / 30);
  if (months < 12) return rtf.format(-months, "month");

  const years = Math.floor(days / 365);
  return rtf.format(-years, "year");
};

    async function requestAI(mode) {
        setAiMode(mode);
        setAiLoading(true);
        const endPoint = `http://localhost:5115/api/ciphers/cipher/${id}/hint`;
        try {
            axios.post(endPoint, {
                hintType : mode
            }, {withCredentials : true})
            .then(res => {
                setAiText(res.data.hintContent);
                console.log("Returned from the Back-End\r\n",res.data.hintContent);
            }).catch((c) => { setAiText("Failed to reach the AI assistant. Please try again."); console.log(c)})
            .finally(() => setAiLoading(false));
            
           
        } catch {
            setAiText("Failed to reach the AI assistant. Please try again.");
        } finally {
            setAiLoading(false);
        }
    }

    return (
        <>            
            <main className="solve-page">
                {/* <nav className="breadcrumb">
                    <a href="/">Home</a>
                    <span className="breadcrumb-sep">/</span>
                    <a href="/ciphers">Browse</a>
                    <span className="breadcrumb-sep">/</span>
                    <span className="breadcrumb-current">#{MOCK_CIPHER.id} {MOCK_CIPHER.title}</span>
                </nav> */}

                <div className="solve-grid">
                    <div className="solve-main">
                        <CipherHeader cipher={cipher}  />

                        <CipherTextDisplay
                            encryptedText={cipher.cipherText}
                            hasImage={cipher.isImage}
                            imageUrl={cipher.imageBase64}
                        />
                        {cipher.challengeTypeDisplay === "Standard" ? (
                        <SolveForm
                            answer={answer}
                            onAnswerChange={setAnswer}
                            onSubmit={handleSubmit}
                            attempts={attempts}
                            result={result}
                        />) : (<ExperimentalCipherPanel
                                 onSubmit={{
                                     decryptedText:        expDecryptedText,
                                     description:          expDescription,
                                     onDecryptedTextChange: setExpDecryptedText,
                                     onDescriptionChange:   setExpDescription,
                                     onSubmit:              handleSubmit,
                                 }}
                                   />)}

                       {cipher.challengeTypeDisplay === "Standard" &&  <AIAssistant
                            onTypeHint={() => requestAI(0)}                       
                            onSolutionHint={() => requestAI(1)}
                            onSolution={() => requestAI(2)}
                            aiMode={aiMode}
                            aiText={aiText}
                            aiLoading={aiLoading}
                        />}
                    </div>

                    <aside className="solve-sidebar">
                        <CipherMeta cipher={cipher} timeAgo={timeAgo}/>
                        <ActivityLog solvers={cipher.recentSolvers} timeAgo={timeAgo} />
                    </aside>
                </div>
            </main>
        </>
    );
}
export default CipherSolvePage;
