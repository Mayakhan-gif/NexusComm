import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { MdMarkEmailRead, MdLockOutline, MdEmail, MdPerson, MdPhone } from 'react-icons/md';
import { registerUser } from '../api/authApi';
import { useAuth } from '../context/AuthContext';
import './AuthPages.css';

export default function Register() {
  const [formData, setFormData] = useState({
    name: '', email: '', phoneNumber: '', password: '',
  });
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
      const response = await registerUser(formData);
      login(response.data.data);
      navigate('/dashboard');
    } catch (err) {
      const errors = err.response?.data?.errors;
      setError(errors?.join(' ') || err.response?.data?.message || 'Registration failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-brand-panel">
        <div className="auth-brand-content">
          <div className="auth-brand-icon"><MdMarkEmailRead /></div>
          <h1>Nexuscomm</h1>
          <p>Create your account and start managing all your communications in one place.</p>
          <ul className="auth-feature-list">
            <li>✓ Free to get started</li>
            <li>✓ Setup takes less than a minute</li>
            <li>✓ Secure & reliable delivery</li>
          </ul>
        </div>
      </div>

      <div className="auth-form-panel">
        <div className="auth-form-box">
          <h2>Create an account</h2>
          <p className="auth-subtitle">Get started with Nexuscomm for free</p>

          {error && <div className="auth-error">{error}</div>}

          <form onSubmit={handleSubmit}>
            <div className="auth-input-group">
              <MdPerson className="auth-input-icon" />
              <input
                type="text"
                name="name"
                placeholder="Full name"
                value={formData.name}
                onChange={handleChange}
                required
              />
            </div>

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
              <MdPhone className="auth-input-icon" />
              <input
                type="tel"
                name="phoneNumber"
                placeholder="Phone number (optional)"
                value={formData.phoneNumber}
                onChange={handleChange}
              />
            </div>

            <div className="auth-input-group">
              <MdLockOutline className="auth-input-icon" />
              <input
                type="password"
                name="password"
                placeholder="Password (min. 6 characters)"
                value={formData.password}
                onChange={handleChange}
                required
                minLength={6}
              />
            </div>

            <button type="submit" className="auth-submit-btn" disabled={loading}>
              {loading ? 'Creating account...' : 'Create Account'}
            </button>
          </form>

          <p className="auth-switch">
            Already have an account? <Link to="/login">Log in</Link>
          </p>
        </div>
      </div>
    </div>
  );
}