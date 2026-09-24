import { useEffect, useState } from 'react';
import { getLoginAttempts } from '../services/auditService';

const getAttemptItems = (response) => Array.isArray(response) ? response : response.value || [];

function getResult(attempt) {
	if (attempt.successful) return { label: 'Acceso exitoso', className: 'success' };
	if (attempt.attemptNumber === 0) return { label: 'Usuario bloqueado', className: 'locked' };
	if (attempt.attemptNumber >= 3) return { label: 'Bloqueado por 3 fallos', className: 'locked' };
	return { label: 'Intento fallido', className: 'failed' };
}

export default function AccessLogPage() {
	const { username } = JSON.parse(localStorage.getItem('credit-simulator-auth') || '{}');
	const [items, setItems] = useState([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState('');

	const loadItems = () => {
		setLoading(true);
		getLoginAttempts().then((response) => setItems(getAttemptItems(response))).catch(() => setError('No se pudo cargar el registro de accesos.')).finally(() => setLoading(false));
	};

	useEffect(() => { loadItems(); }, []);

	return <>
		<div className="page-heading access-heading"><div><p className="eyebrow">Seguridad</p><h1>Actividad de accesos</h1><p>Consulta los intentos de inicio de sesión registrados por el sistema.</p></div><button className="button secondary" onClick={loadItems} disabled={loading}>Actualizar</button></div>
		{loading && <p className="message">Cargando registro...</p>}
		{error && <p className="message error">{error}</p>}
		{!loading && !error && <section className="panel access-panel"><div className="access-summary"><div><strong>{items.length}</strong><span>Registros encontrados</span></div><div><strong>{username || 'Usuario'}</strong><span>Sesión actual</span></div></div>{items.length === 0 ? <div className="empty-state"><h2>Aún no hay accesos registrados</h2><p>Los próximos intentos de inicio de sesión aparecerán aquí.</p></div> : <div className="table-wrap"><table className="access-table"><thead><tr><th>Usuario</th><th>Fecha de registro</th><th>Resultado</th><th>Intento</th></tr></thead><tbody>{items.map((item) => { const result = getResult(item); return <tr key={item.id}><td><strong>{item.username}</strong></td><td>{new Date(item.registeredAt).toLocaleString('es-ES')}</td><td><span className={`access-status ${result.className}`}>{result.label}</span></td><td>{item.attemptNumber === 0 ? 'Bloqueado' : `#${item.attemptNumber}`}</td></tr>; })}</tbody></table></div>}</section>}
	</>;
}