import { useLocation } from "react-router-dom";
import "./styles/page-transition.css";

export default function PageTransition({ children }) {
  const location = useLocation();

  return (
    <div key={location.key} className="page-transition">
      {children}
    </div>
  );
}