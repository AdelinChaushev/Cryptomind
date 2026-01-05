import React, { useEffect, useState } from "react";
import axios from "axios";
import "./styles/AccountInfo.css";

export default function AccountInfo() {
  const [account, setAccount] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    axios
      .get("http://localhost:5115/api/user/getAccountInfo", { withCredentials: true })
      .then((response) => {
        setAccount(response.data);
      })
      .catch((error) => {
        console.error("Error fetching account info:", error);
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="account-loading">Loading account...</div>;
  if (!account) return <div className="account-error">Unable to load account info.</div>;

  return (
    <div className="account-container">
      <h1 className="account-title">Account Information</h1>

      <div className="account-card">
        <p><strong>Username:</strong> {account.username}</p>
        <p><strong>Email:</strong> {account.email}</p>
        <p><strong>Points:</strong> {account.points}</p>
        <p><strong>Ciphers Solved:</strong> {account.solvedCount}</p>
        <p><strong>Roles:</strong> {account.roles && account.roles.length > 0
          ? account.roles.join(", ")
          : "No roles assigned"}</p>
      </div>
    </div>
  );
}
