import { catalogClient } from './apiClient';
export const getCreditTypes = async () => (await catalogClient.get('/api/credittypes')).data;
