import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
	const { username, logout } = useAuth(); const navigate = useNavigate();
	const exit = () => { logout(); navigate('/login'); };
	return <header className="navbar"><div className="brand">Credit<span>Sim</span></div><nav><NavLink to="/simulator">Simulador</NavLink><NavLink to="/history">Historial</NavLink></nav><div className="user-menu"><span>{username}</span><button className="button ghost" onClick={exit}>Cerrar sesión</button></div></header>;
}
