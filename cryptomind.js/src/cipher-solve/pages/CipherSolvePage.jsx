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
import { useError } from "../../ErrorContext.jsx";

function CipherSolvePage() {
    const navigation = useNavigate();
    const [answer,    setAnswer]    = useState("");
    const [attempts,  setAttempts]  = useState(0);
    const [result,    setResult]    = useState(false);
    const [aiMode,    setAiMode]    = useState(-1);
    const [aiText,    setAiText]    = useState("");
    const [aiLoading, setAiLoading] = useState(false);
    const [expDecryptedText, setExpDecryptedText] = useState("");
    const [expDescription,   setExpDescription]   = useState("");
    const [tags,setTags] = useState([])
    const { setError } = useError();
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
        tags : []

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
        challengeTypeDisplay : data.challengeTypeDisplay,
        tags : data.tags

      });
       }
    )
       .catch(c => navigation("*") )
       .finally(console.log(cipher))
    },[id])
    
   const handleSubmit = async () => {
    if (cipher.challengeTypeDisplay === "Standard") {
        if (!answer.trim()) return false;

        try {
            const res = await axios.post(`http://localhost:5115/api/ciphers/cipher/${id}/solve`, {
                UserSolution: answer
            }, { withCredentials: true });
            
            setResult(res.data ? 'correct' : 'incorrect');
            return true; 
        } catch (e) {
            setError(e.response?.data?.error || 'Failed to submit answer.');
            return false;
        }
    } 
    else if (cipher.challengeTypeDisplay === "Experimental") {
        try {
            await axios.post(`http://localhost:5115/api/ciphers/cipher/${id}/suggest-answer`, {
                description: expDescription,
                decryptedText: expDecryptedText
            }, { withCredentials: true });
            
            return true; // SUCCESS
        } catch (e) {
            console.log(e);
            setError(e.response?.data?.error || e.response?.data?.title || 'Failed to submit answer.');
            return false; // FAILURE
        }
    }
    return false;
};
        const timeAgo = (dateValue) => {
    if (!dateValue) return "—";

    const rtf = new Intl.RelativeTimeFormat("bg", { numeric: "auto" });

    // 1. Handle duration format (HH:mm:ss)
    if (
        typeof dateValue === "string" &&
        dateValue.includes(":") &&
        !dateValue.includes("-")
    ) {
        const parts = dateValue.split(":");

        if (parts.length === 3) {
            const hours = parseInt(parts[0], 10);
            const minutes = parseInt(parts[1], 10);
            const seconds = Math.floor(parseFloat(parts[2]));

            if (hours > 0) return rtf.format(-hours, "hour");
            if (minutes > 0) return rtf.format(-minutes, "minute");
            return rtf.format(-seconds, "second");
        }
    }

    // 2. Handle Date / timestamp
    const date = dateValue instanceof Date
        ? dateValue
        : new Date(dateValue);

    if (isNaN(date.getTime())) {
        console.warn("Invalid date:", dateValue);
        return "—";
    }

    const seconds = Math.floor((Date.now() - date.getTime()) / 1000);

    if (seconds < 5) return "току-що";

    const intervals = [
        { label: "year", seconds: 31536000 },
        { label: "month", seconds: 2592000 },
        { label: "week", seconds: 604800 },
        { label: "day", seconds: 86400 },
        { label: "hour", seconds: 3600 },
        { label: "minute", seconds: 60 },
        { label: "second", seconds: 1 }
    ];

    for (const interval of intervals) {
        const count = Math.floor(seconds / interval.seconds);
        if (count >= 1) {
            return rtf.format(-count, interval.label);
        }
    }

    return "—";
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
            }).catch((err) => { setAiText(err.response.data.error); console.log(c)})
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

                       {cipher.challengeTypeDisplay === "Standard" 
                       &&
                        (cipher.allowsTypeHint || cipher.allowsSolutionHint || cipher.allowsFullSolution)
                       &&  <AIAssistant
                            onTypeHint={() => requestAI(0)}                       
                            onSolutionHint={() => requestAI(1)}
                            onSolution={() => requestAI(2)}
                            aiMode={aiMode}
                            aiText={aiText}
                            aiLoading={aiLoading}
                            allowTypeHint={cipher.allowsTypeHint }
                            allowSolutionHint={cipher.allowsSolutionHint}
                            allowSolution={cipher.allowsFullSolution}
                        />}
                    </div>
                    { cipher.challengeTypeDisplay === "Standard" &&
                    <aside className="solve-sidebar">
                        <CipherMeta cipher={cipher} timeAgo={timeAgo}/>
                        <ActivityLog solvers={cipher.recentSolvers} timeAgo={timeAgo} />
                    </aside>
                    }
                </div>
            </main>
        </>
    );
}
export default CipherSolvePage;
