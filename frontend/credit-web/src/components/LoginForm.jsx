import { useState } from 'react';
import { useAuth } from '../context/AuthContext';

export default function LoginForm({ onRegister }) {
	const { login } = useAuth(); const [form, setForm] = useState({ username: '', password: '' }); const [error, setError] = useState(''); const [loading, setLoading] = useState(false);
	const submit = async (event) => { event.preventDefault(); setError(''); setLoading(true); try { await login(form); } catch (e) { setError(e.response?.data?.message || 'No se pudo iniciar sesión.'); } finally { setLoading(false); } };
	return <form className="auth-form" onSubmit={submit}><label>Usuario<input required value={form.username} onChange={(e) => setForm({ ...form, username: e.target.value })} /></label><label>Contraseña<input required type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} /></label>{error && <p className="message error">{error}</p>}<button className="button primary" disabled={loading}>{loading ? 'Entrando...' : 'Iniciar sesión'}</button><button type="button" className="link-button" onClick={onRegister}>Crear una cuenta</button></form>;
}
