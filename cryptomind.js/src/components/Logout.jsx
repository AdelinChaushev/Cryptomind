import { useNavigate } from "react-router-dom";
import axios from "axios";
import { useError } from "../ErrorContext.jsx";
import { useContext } from "react";
import { AuthorizationContext } from '../App.jsx';

export default function useLogout() {
    useNavigate();
    const { setError } = useError();
    useContext(AuthorizationContext)

    const logout = async () => {
        try {
            await axios.post("/api/auth/logout", {}, {
                withCredentials: true
            });

            console.log("Logged out successfully");

            window.location.href = "/";

        } catch (error) {
            setError(error.response?.data?.error ?? "Неуспешно излизане");
        }
    };

    return logout;
}