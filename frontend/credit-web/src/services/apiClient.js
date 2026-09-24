import axios from 'axios';

const createClient = (baseURL) => {
	const client = axios.create({ baseURL, headers: { 'Content-Type': 'application/json' } });
	client.interceptors.request.use((config) => {
		const token = localStorage.getItem('credit-token');
		if (token) config.headers.Authorization = `Bearer ${token}`;
		return config;
	});
	client.interceptors.response.use((response) => response, (error) => {
		if (error.response?.status === 401 && window.location.pathname !== '/login') {
			localStorage.removeItem('credit-token'); localStorage.removeItem('credit-simulator-auth'); window.location.assign('/login');
		}
		return Promise.reject(error);
	});
	return client;
};

export const authClient = createClient(import.meta.env.VITE_AUTH_URL || 'http://localhost:5001');
export const auditClient = createClient(import.meta.env.VITE_AUDIT_URL || 'http://localhost:5004');
export const catalogClient = createClient(import.meta.env.VITE_CATALOG_URL || 'http://localhost:5002');
export const simulationClient = createClient(import.meta.env.VITE_SIMULATION_URL || 'http://localhost:5003');
