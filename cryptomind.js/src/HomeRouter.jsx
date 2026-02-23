import { useContext } from "react";
import { AuthorizationContext } from "./App";
import Home from "./pages/Home";
import CipherBrowsePage from "./cipher-browser/CipherBrowsePage";

export default function HomeRouter() {
  const { state ,setState } = useContext(AuthorizationContext);

  return state.isLoggedIn
    ? <CipherBrowsePage />
    : <Home />;
}