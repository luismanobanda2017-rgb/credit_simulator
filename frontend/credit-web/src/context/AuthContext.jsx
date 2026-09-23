import { createContext, useContext, useMemo, useState } from 'react';
import { login as loginRequest, register as registerRequest } from '../services/authService';

const AuthContext = createContext(null);
const STORAGE_KEY = 'credit-simulator-auth';

export function AuthProvider({ children }) {
	const [session, setSession] = useState(() => {
		try { return JSON.parse(localStorage.getItem(STORAGE_KEY)) || null; } catch { return null; }
	});
	const login = async (credentials) => { const result = await loginRequest(credentials); localStorage.setItem(STORAGE_KEY, JSON.stringify(result)); localStorage.setItem('credit-token', result.token); setSession(result); return result; };
	const register = async (data) => { const result = await registerRequest(data); localStorage.setItem(STORAGE_KEY, JSON.stringify(result)); localStorage.setItem('credit-token', result.token); setSession(result); return result; };
	const logout = () => { localStorage.removeItem(STORAGE_KEY); localStorage.removeItem('credit-token'); setSession(null); };
	const value = useMemo(() => ({ ...session, isAuthenticated: Boolean(session?.token), login, register, logout }), [session]);
	return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() { return useContext(AuthContext); }
