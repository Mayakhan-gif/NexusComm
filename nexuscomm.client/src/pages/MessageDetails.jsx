import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { MdArrowBack, MdCheckCircle, MdCancel } from 'react-icons/md';
import { getCommunicationById } from '../api/communicationsApi';
import StatusBadge from '../components/StatusBadge';
import ChannelIcon from '../components/ChannelIcon';
import './MessageDetails.css';

export default function MessageDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [message, setMessage] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchDetails();
  }, [id]);

  const fetchDetails = async () => {
    try {
      const res = await getCommunicationById(id);
      setMessage(res.data.data);
    } catch (err) {
      setError(err.response?.data?.message || 'Could not load message details.');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div className="page-loading">Loading details...</div>;
  if (error) return <div className="page-error">{error}</div>;
  if (!message) return null;

  const fields = [
    { label: 'Message ID', value: `#${message.id}` },
    { label: 'Recipient', value: message.recipient },
    { label: 'Subject', value: message.subject || '—' },
    { label: 'Created At', value: new Date(message.createdAt).toLocaleString() },
    { label: 'Scheduled At', value: message.scheduledAt ? new Date(message.scheduledAt).toLocaleString() : '—' },
    { label: 'Processing Started', value: message.processingStartedAt ? new Date(message.processingStartedAt).toLocaleString() : '—' },
    { label: 'Sent At', value: message.sentAt ? new Date(message.sentAt).toLocaleString() : '—' },
    { label: 'Retry Count', value: `${message.retryCount} / ${message.maxRetryAttempts}` },
    { label: 'Provider Message ID', value: message.providerMessageId || '—' },
  ];

  return (
    <div>
      <button className="back-link" onClick={() => navigate(-1)}>
        <MdArrowBack /> Back
      </button>

      <div className="details-header">
        <div className="details-title">
          <ChannelIcon channel={message.channel} size={24} />
          <h1>Message #{message.id}</h1>
          <StatusBadge status={message.status} />
        </div>
      </div>

      {message.failureReason && (
        <div className="failure-banner">
          <strong>Failure Reason:</strong> {message.failureReason}
        </div>
      )}

      <div className="details-grid">
        <div className="details-card">
          <h3>Message Info</h3>
          <div className="field-list">
            {fields.map((f) => (
              <div key={f.label} className="field-row">
                <span className="field-label">{f.label}</span>
                <span className="field-value">{f.value}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="details-card">
          <h3>Message Body</h3>
          <p className="message-body-preview">{message.body}</p>
        </div>
      </div>

      <div className="details-card">
        <h3>Attempt History</h3>
        {message.attempts.length === 0 ? (
          <div className="empty-state">No dispatch attempts yet.</div>
        ) : (
          <div className="attempt-timeline">
            {message.attempts.map((attempt) => (
              <div key={attempt.id} className={`attempt-item ${attempt.status === 'Success' ? 'success' : 'failed'}`}>
                <div className="attempt-icon">
                  {attempt.status === 'Success' ? <MdCheckCircle /> : <MdCancel />}
                </div>
                <div className="attempt-content">
                  <div className="attempt-header">
                    <strong>Attempt {attempt.attemptNumber}</strong>
                    <span className="attempt-time">{new Date(attempt.attemptedAt).toLocaleString()}</span>
                  </div>
                  <div className="attempt-status">
                    {attempt.status === 'Success'
                      ? 'Delivered successfully'
                      : attempt.failureReason || 'Failed'}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}