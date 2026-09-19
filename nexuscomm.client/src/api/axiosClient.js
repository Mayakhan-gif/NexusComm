import axios from 'axios';

// Change this if your backend runs on a different port
const API_BASE_URL = 'https://localhost:7270/api';

const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Attach the JWT token (if we have one) to every outgoing request
axiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('nexuscomm_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// If the backend says "401 Unauthorized" (token expired/invalid), log the user out
axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('nexuscomm_token');
      localStorage.removeItem('nexuscomm_user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axiosClient;