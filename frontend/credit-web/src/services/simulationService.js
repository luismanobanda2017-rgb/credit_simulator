import { simulationClient } from './apiClient';
export const createSimulation = async (data) => (await simulationClient.post('/api/simulations', data)).data;
export const getSimulations = async () => (await simulationClient.get('/api/simulations')).data;
export const getSimulation = async (id) => (await simulationClient.get(`/api/simulations/${id}`)).data;
export const downloadSimulation = async (id, format) => {
	const response = await simulationClient.get(`/api/simulations/${id}/export`, { params: { format }, responseType: 'blob' });
	const disposition = response.headers['content-disposition'] || '';
	const filename = disposition.match(/filename="?([^";]+)"?/)?.[1] || `simulacion-${id}.${format}`;
	const url = URL.createObjectURL(response.data); const anchor = document.createElement('a'); anchor.href = url; anchor.download = filename; anchor.click(); URL.revokeObjectURL(url);
};
