import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  MdSchedule, MdCheckCircle, MdErrorOutline, MdSync,
  MdMailOutline, MdCancel, MdArrowForward
} from 'react-icons/md';
import { getDashboardSummary } from '../api/communicationsApi';
import StatusBadge from '../components/StatusBadge';
import ChannelIcon from '../components/ChannelIcon';
import './Dashboard.css';

export default function Dashboard() {
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchSummary();
  }, []);

  const fetchSummary = async () => {
    try {
      const res = await getDashboardSummary();
      setSummary(res.data.data);
    } catch (err) {
      setError('Failed to load dashboard data.');
    } finally {
      setLoading(false);
    }
  };

  const statCards = summary ? [
    { label: 'Total Messages', value: summary.total, icon: <MdMailOutline />, color: 'primary' },
    { label: 'Scheduled', value: summary.scheduled, icon: <MdSchedule />, color: 'info' },
    { label: 'Sent', value: summary.sent, icon: <MdCheckCircle />, color: 'success' },
    { label: 'Retrying', value: summary.retrying, icon: <MdSync />, color: 'warning' },
    { label: 'Failed', value: summary.failed, icon: <MdErrorOutline />, color: 'danger' },
    { label: 'Cancelled', value: summary.cancelled, icon: <MdCancel />, color: 'muted' },
  ] : [];

  if (loading) return <div className="page-loading">Loading dashboard...</div>;
  if (error) return <div className="page-error">{error}</div>;

  return (
    <div className="dashboard-page">
      <div className="page-header">
        <div>
          <h1>Dashboard</h1>
          <p className="page-subtitle">Overview of all your communications</p>
        </div>
        <Link to="/create" className="btn-primary">+ New Message</Link>
      </div>

      <div className="stats-grid">
        {statCards.map((stat) => (
          <div key={stat.label} className={`stat-card stat-${stat.color}`}>
            <div className="stat-icon">{stat.icon}</div>
            <div>
              <div className="stat-value">{stat.value}</div>
              <div className="stat-label">{stat.label}</div>
            </div>
          </div>
        ))}
      </div>

      <div className="recent-section">
        <div className="section-header">
          <h2>Recent Communications</h2>
          <Link to="/all" className="section-link">View all <MdArrowForward /></Link>
        </div>

        {summary.recentCommunications.length === 0 ? (
          <div className="empty-state">No communications yet. Create your first message!</div>
        ) : (
          <div className="table-card">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Channel</th>
                  <th>Recipient</th>
                  <th>Subject / Preview</th>
                  <th>Status</th>
                  <th>Created</th>
                </tr>
              </thead>
              <tbody>
                {summary.recentCommunications.map((msg) => (
                  <tr key={msg.id}>
                    <td><ChannelIcon channel={msg.channel} /></td>
                    <td>{msg.recipient}</td>
                    <td className="truncate">{msg.subject || '—'}</td>
                    <td><StatusBadge status={msg.status} /></td>
                    <td>{new Date(msg.createdAt).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}