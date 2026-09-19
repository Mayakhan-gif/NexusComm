import { NavLink, useNavigate } from 'react-router-dom';
import {
  MdDashboard, MdAdd, MdSchedule, MdCheckCircle,
  MdErrorOutline, MdList, MdLogout, MdMarkEmailRead
} from 'react-icons/md';
import { useAuth } from '../../context/AuthContext';
import './Sidebar.css';

export default function Sidebar() {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navItems = [
    { to: '/dashboard', icon: <MdDashboard />, label: 'Dashboard' },
    { to: '/create', icon: <MdAdd />, label: 'Create Message' },
    { to: '/scheduled', icon: <MdSchedule />, label: 'Scheduled' },
    { to: '/sent', icon: <MdCheckCircle />, label: 'Sent' },
    { to: '/failed', icon: <MdErrorOutline />, label: 'Failed' },
    { to: '/all', icon: <MdList />, label: 'All Messages' },
  ];

  return (
    <aside className="sidebar">
      <div className="sidebar-brand">
        <div className="brand-icon"><MdMarkEmailRead /></div>
        <span className="brand-name">Nexuscomm</span>
      </div>

      <nav className="sidebar-nav">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
          >
            <span className="sidebar-icon">{item.icon}</span>
            {item.label}
          </NavLink>
        ))}

        {isAdmin && (
          <NavLink
            to="/admin"
            className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
          >
            <span className="sidebar-icon">👑</span>
            Admin Panel
          </NavLink>
        )}
      </nav>

      <div className="sidebar-footer">
        <div className="user-info">
          <div className="user-avatar">{user?.name?.charAt(0)?.toUpperCase() || 'U'}</div>
          <div className="user-meta">
            <div className="user-name">{user?.name}</div>
            <div className="user-role">{user?.role}</div>
          </div>
        </div>
        <button className="logout-btn" onClick={handleLogout} title="Logout">
          <MdLogout />
        </button>
      </div>
    </aside>
  );
}