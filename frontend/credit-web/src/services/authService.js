import { authClient } from './apiClient';
export const login = async (data) => (await authClient.post('/api/auth/login', data)).data;
export const register = async (data) => (await authClient.post('/api/auth/register', data)).data;
