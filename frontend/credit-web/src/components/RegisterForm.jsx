import { useState } from 'react';
import { useAuth } from '../context/AuthContext';

export default function RegisterForm({ onLogin }) {
	const { register } = useAuth(); const [form, setForm] = useState({ username: '', email: '', password: '' }); const [error, setError] = useState(''); const [loading, setLoading] = useState(false);
	const submit = async (event) => { event.preventDefault(); setError(''); setLoading(true); try { await register(form); } catch (e) { setError(e.response?.data?.message || 'No se pudo crear la cuenta.'); } finally { setLoading(false); } };
	return <form className="auth-form" onSubmit={submit}><label>Usuario<input required minLength="3" value={form.username} onChange={(e) => setForm({ ...form, username: e.target.value })} /></label><label>Correo electrónico<input required type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} /></label><label>Contraseña<input required minLength="6" type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} /></label>{error && <p className="message error">{error}</p>}<button className="button primary" disabled={loading}>{loading ? 'Creando...' : 'Registrarme'}</button><button type="button" className="link-button" onClick={onLogin}>Ya tengo una cuenta</button></form>;
}
