import { createContext, useContext, useState } from 'react';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem('nexuscomm_user');
    return saved ? JSON.parse(saved) : null;
  });

  const login = (authData) => {
    localStorage.setItem('nexuscomm_token', authData.token);
    localStorage.setItem('nexuscomm_user', JSON.stringify(authData));
    setUser(authData);
  };

  const logout = () => {
    localStorage.removeItem('nexuscomm_token');
    localStorage.removeItem('nexuscomm_user');
    setUser(null);
  };

  const isAdmin = user?.role === 'Admin';

  return (
    <AuthContext.Provider value={{ user, login, logout, isAdmin }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}