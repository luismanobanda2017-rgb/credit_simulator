import { Navigate } from 'react-router-dom';
import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import LoginForm from '../components/LoginForm';
import RegisterForm from '../components/RegisterForm';

export default function LoginPage() {
	const { isAuthenticated } = useAuth(); const [registering, setRegistering] = useState(false);
	if (isAuthenticated) return <Navigate to="/simulator" replace />;
	return <main className="auth-page"><section className="auth-panel"><div className="brand large">Credit<span>Sim</span></div><p className="eyebrow">Finanzas claras, decisiones seguras</p><h1>{registering ? 'Crea tu cuenta' : 'Bienvenido de nuevo'}</h1>{registering ? <RegisterForm onLogin={() => setRegistering(false)} /> : <LoginForm onRegister={() => setRegistering(true)} />}</section><aside className="auth-aside"><p className="eyebrow">Tu próximo paso</p><h2>Entiende cada cuota antes de firmar.</h2><p>Compara métodos de amortización y conserva tus simulaciones en un solo lugar.</p></aside></main>;
}
