import axiosClient from './axiosClient';

export const createCommunication = (data) => axiosClient.post('/communications', data);
export const getAllCommunications = () => axiosClient.get('/communications');
export const getCommunicationById = (id) => axiosClient.get(`/communications/${id}`);
export const updateCommunication = (id, data) => axiosClient.put(`/communications/${id}`, data);
export const cancelCommunication = (id) => axiosClient.post(`/communications/${id}/cancel`);
export const getAttempts = (id) => axiosClient.get(`/communications/${id}/attempts`);
export const getDashboardSummary = () => axiosClient.get('/dashboard/summary');