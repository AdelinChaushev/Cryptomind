// hooks/useLogout.js
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useError } from "../ErrorContext.jsx";
import { useContext } from "react";
import { AuthorizationContext } from '../App.jsx' ; // example (see below)

export default function useLogout() {
    const navigate = useNavigate();
    const { setError } = useError();
    const {state,setState} = useContext(AuthorizationContext)
     // your auth state provider

    const logout = async () => {
        try {
            await axios.post("/api/auth/logout", {}, {
                withCredentials: true
            });

            console.log("Logged out successfully");

            setState({ isLoggedIn: false, roles: [] })
            window.location.href = "/";

        } catch (error) {
            setError(error.response?.data?.error ?? "Logout failed");
        }
    };

    return logout;
}