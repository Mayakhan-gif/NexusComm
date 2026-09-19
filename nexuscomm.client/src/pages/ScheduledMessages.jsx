import MessageListPage from '../components/MessageListPage';

export default function ScheduledMessages() {
  return (
    <MessageListPage
      title="Scheduled Messages"
      subtitle="Messages waiting to be dispatched at their scheduled time"
      statusFilter="Scheduled"
    />
  );
}