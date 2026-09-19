import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { MdEmail, MdArrowBack } from 'react-icons/md';
import { FaWhatsapp } from 'react-icons/fa';
import { createCommunication } from '../api/communicationsApi';
import './CreateCommunication.css';

export default function CreateCommunication() {
  const navigate = useNavigate();

  const [channel, setChannel] = useState('Email'); // 'Email' | 'WhatsApp'
  const [sendMode, setSendMode] = useState('now');  // 'now' | 'schedule'
  const [formData, setFormData] = useState({
    recipient: '',
    subject: '',
    body: '',
    scheduledDate: '',
    scheduledTime: '',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    // Basic client-side checks (backend re-validates everything anyway)
    if (channel === 'Email' && !formData.subject.trim()) {
      setError('Subject is required for Email messages.');
      return;
    }

    let scheduledAtIso = null;
    if (sendMode === 'schedule') {
      if (!formData.scheduledDate || !formData.scheduledTime) {
        setError('Please choose both a date and a time to schedule this message.');
        return;
      }
      scheduledAtIso = new Date(`${formData.scheduledDate}T${formData.scheduledTime}`).toISOString();
    }

    const payload = {
      channel: channel === 'Email' ? 0 : 1,
      recipient: formData.recipient,
      subject: channel === 'Email' ? formData.subject : null,
      body: formData.body,
      sendNow: sendMode === 'now',
      scheduledAt: scheduledAtIso,
    };

    setLoading(true);
    try {
      const res = await createCommunication(payload);
      setSuccess(
        sendMode === 'now'
          ? 'Message dispatched! Check its status shortly.'
          : 'Message scheduled successfully.'
      );
      setTimeout(() => navigate(sendMode === 'now' ? '/sent' : '/scheduled'), 1200);
    } catch (err) {
      const errors = err.response?.data?.errors;
      setError(errors?.join(' ') || err.response?.data?.message || 'Failed to create communication.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="create-comm-page">
      <div className="page-header">
        <div>
          <button className="back-link" onClick={() => navigate(-1)}>
            <MdArrowBack /> Back
          </button>
          <h1>Create Message</h1>
          <p className="page-subtitle">Send an email or WhatsApp message, now or scheduled</p>
        </div>
      </div>

      <div className="form-card">
        {/* ---- Channel Selector ---- */}
        <div className="channel-selector">
          <button
            type="button"
            className={`channel-option ${channel === 'Email' ? 'active' : ''}`}
            onClick={() => setChannel('Email')}
          >
            <MdEmail size={22} />
            <span>Email</span>
          </button>
          <button
            type="button"
            className={`channel-option whatsapp ${channel === 'WhatsApp' ? 'active' : ''}`}
            onClick={() => setChannel('WhatsApp')}
          >
            <FaWhatsapp size={22} />
            <span>WhatsApp</span>
          </button>
        </div>

        {error && <div className="form-alert error">{error}</div>}
        {success && <div className="form-alert success">{success}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>{channel === 'Email' ? 'Recipient Email' : 'WhatsApp Number'}</label>
            <input
              type={channel === 'Email' ? 'email' : 'tel'}
              name="recipient"
              placeholder={channel === 'Email' ? 'customer@example.com' : '+923001234567'}
              value={formData.recipient}
              onChange={handleChange}
              required
            />
          </div>

          {channel === 'Email' && (
            <div className="form-group">
              <label>Subject</label>
              <input
                type="text"
                name="subject"
                placeholder="Appointment Reminder"
                value={formData.subject}
                onChange={handleChange}
                required
              />
            </div>
          )}

          <div className="form-group">
            <label>Message Body</label>
            <textarea
              name="body"
              rows={6}
              placeholder="Type your message here..."
              value={formData.body}
              onChange={handleChange}
              required
            />
          </div>

          {/* ---- Send Now / Schedule ---- */}
          <div className="form-group">
            <label>When should this be sent?</label>
            <div className="send-mode-selector">
              <button
                type="button"
                className={`mode-option ${sendMode === 'now' ? 'active' : ''}`}
                onClick={() => setSendMode('now')}
              >
                Send Now
              </button>
              <button
                type="button"
                className={`mode-option ${sendMode === 'schedule' ? 'active' : ''}`}
                onClick={() => setSendMode('schedule')}
              >
                Schedule
              </button>
            </div>
          </div>

          {sendMode === 'schedule' && (
            <div className="form-row">
              <div className="form-group">
                <label>Scheduled Date</label>
                <input
                  type="date"
                  name="scheduledDate"
                  value={formData.scheduledDate}
                  onChange={handleChange}
                  min={new Date().toISOString().split('T')[0]}
                  required
                />
              </div>
              <div className="form-group">
                <label>Scheduled Time</label>
                <input
                  type="time"
                  name="scheduledTime"
                  value={formData.scheduledTime}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>
          )}

          <button type="submit" className="btn-primary btn-block" disabled={loading}>
            {loading
              ? 'Processing...'
              : sendMode === 'now'
              ? `Send ${channel === 'Email' ? 'Email' : 'WhatsApp Message'} Now`
              : 'Schedule Message'}
          </button>
        </form>
      </div>
    </div>
  );
}