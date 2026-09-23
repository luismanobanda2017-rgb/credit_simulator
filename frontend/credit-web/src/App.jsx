import { Navigate, Route, Routes, Outlet } from 'react-router-dom';
import Navbar from './components/Navbar';
import PrivateRoute from './components/PrivateRoute';
import HistoryPage from './pages/HistoryPage';
import LoginPage from './pages/LoginPage';
import SimulationDetailPage from './pages/SimulationDetailPage';
import SimulatorPage from './pages/SimulatorPage';

function PrivateLayout() { return <><Navbar /><main className="page-shell"><Outlet /></main></>; }

export default function App() {
	return <Routes><Route path="/login" element={<LoginPage />} /><Route element={<PrivateRoute />}><Route element={<PrivateLayout />}><Route path="/simulator" element={<SimulatorPage />} /><Route path="/history" element={<HistoryPage />} /><Route path="/history/:id" element={<SimulationDetailPage />} /></Route></Route><Route path="*" element={<Navigate to="/simulator" replace />} /></Routes>;
}
