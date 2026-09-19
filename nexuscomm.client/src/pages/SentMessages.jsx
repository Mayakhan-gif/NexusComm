import MessageListPage from '../components/MessageListPage';

export default function SentMessages() {
  return (
    <MessageListPage
      title="Sent Messages"
      subtitle="Successfully delivered communications"
      statusFilter="Sent"
    />
  );
}