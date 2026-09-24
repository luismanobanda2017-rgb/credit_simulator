import { auditClient } from './apiClient';

export const getLoginAttempts = async () => (await auditClient.get('/api/login-attempts')).data;