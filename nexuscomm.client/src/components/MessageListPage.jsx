import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { MdSearch } from 'react-icons/md';
import { getAllCommunications, cancelCommunication } from '../api/communicationsApi';
import StatusBadge from './StatusBadge';
import ChannelIcon from './ChannelIcon';
import './MessageListPage.css';

// statusFilter: null (All) or one of "Scheduled" | "Sent" | "Failed" | "Retrying" | "Cancelled"
export default function MessageListPage({ title, subtitle, statusFilter }) {
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchMessages();
  }, []);

  const fetchMessages = async () => {
    setLoading(true);
    try {
      const res = await getAllCommunications();
      setMessages(res.data.data);
    } catch (err) {
      setError('Failed to load communications.');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (e, id) => {
    e.stopPropagation();
    if (!window.confirm('Cancel this scheduled message?')) return;

    try {
      await cancelCommunication(id);
      fetchMessages();
    } catch (err) {
      alert(err.response?.data?.message || 'Could not cancel this message.');
    }
  };

  const filtered = messages
    .filter((m) => (statusFilter ? m.status === statusFilter : true))
    .filter((m) =>
      search.trim() === ''
        ? true
        : m.recipient.toLowerCase().includes(search.toLowerCase()) ||
          (m.subject || '').toLowerCase().includes(search.toLowerCase())
    );

  const canCancel = (status) => ['Draft', 'Scheduled', 'Retrying'].includes(status);

  if (loading) return <div className="page-loading">Loading messages...</div>;
  if (error) return <div className="page-error">{error}</div>;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>{title}</h1>
          <p className="page-subtitle">{subtitle}</p>
        </div>
      </div>

      <div className="list-toolbar">
        <div className="search-box">
          <MdSearch />
          <input
            type="text"
            placeholder="Search by recipient or subject..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className="empty-state">No messages found here.</div>
      ) : (
        <div className="table-card">
          <table className="data-table">
            <thead>
              <tr>
                <th>Channel</th>
                <th>Recipient</th>
                <th>Subject / Preview</th>
                <th>Status</th>
                <th>Retries</th>
                <th>Created</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((msg) => (
                <tr key={msg.id} onClick={() => navigate(`/messages/${msg.id}`)} className="clickable-row">
                  <td><ChannelIcon channel={msg.channel} /></td>
                  <td>{msg.recipient}</td>
                  <td className="truncate">{msg.subject || msg.failureReason || '—'}</td>
                  <td><StatusBadge status={msg.status} /></td>
                  <td>{msg.retryCount}</td>
                  <td>{new Date(msg.createdAt).toLocaleString()}</td>
                  <td>
                    {canCancel(msg.status) && (
                      <button className="btn-link-danger" onClick={(e) => handleCancel(e, msg.id)}>
                        Cancel
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}