import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { MdMarkEmailRead, MdLockOutline, MdEmail } from 'react-icons/md';
import { loginUser } from '../api/authApi';
import { useAuth } from '../context/AuthContext';
import './AuthPages.css';

export default function Login() {
  const [formData, setFormData] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await axiosLogin(formData);
      login(response.data.data);
      navigate('/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Login failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  const axiosLogin = (data) => loginUser(data);

  return (
    <div className="auth-page">
      <div className="auth-brand-panel">
        <div className="auth-brand-content">
          <div className="auth-brand-icon"><MdMarkEmailRead /></div>
          <h1>Nexuscomm</h1>
          <p>One dashboard to schedule, dispatch, and track every Email and WhatsApp message you send.</p>
          <ul className="auth-feature-list">
            <li>✓ Send Email & WhatsApp from one place</li>
            <li>✓ Automatic retry on failure</li>
            <li>✓ Complete delivery tracking</li>
          </ul>
        </div>
      </div>

      <div className="auth-form-panel">
        <div className="auth-form-box">
          <h2>Welcome back</h2>
          <p className="auth-subtitle">Log in to your account to continue</p>

          {error && <div className="auth-error">{error}</div>}

          <form onSubmit={handleSubmit}>
            <div className="auth-input-group">
              <MdEmail className="auth-input-icon" />
              <input
                type="email"
                name="email"
                placeholder="Email address"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>

            <div className="auth-input-group">
              <MdLockOutline className="auth-input-icon" />
              <input
                type="password"
                name="password"
                placeholder="Password"
                value={formData.password}
                onChange={handleChange}
                required
              />
            </div>

            <button type="submit" className="auth-submit-btn" disabled={loading}>
              {loading ? 'Logging in...' : 'Log In'}
            </button>
          </form>

          <p className="auth-switch">
            Don't have an account? <Link to="/register">Create one</Link>
          </p>
        </div>
      </div>
    </div>
  );
}