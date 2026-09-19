import './StatusBadge.css';

const STATUS_CONFIG = {
  Draft:      { label: 'Draft',     className: 'badge-muted' },
  Scheduled:  { label: 'Scheduled', className: 'badge-info' },
  Processing: { label: 'Processing', className: 'badge-warning' },
  Sent:       { label: 'Sent',      className: 'badge-success' },
  Failed:     { label: 'Failed',    className: 'badge-danger' },
  Retrying:   { label: 'Retrying',  className: 'badge-warning' },
  Cancelled:  { label: 'Cancelled', className: 'badge-muted' },
};

export default function StatusBadge({ status }) {
  const config = STATUS_CONFIG[status] || { label: status, className: 'badge-muted' };
  return <span className={`status-badge ${config.className}`}>{config.label}</span>;
}