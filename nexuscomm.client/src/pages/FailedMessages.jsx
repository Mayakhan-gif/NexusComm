import MessageListPage from '../components/MessageListPage';

export default function FailedMessages() {
  return (
    <MessageListPage
      title="Failed Messages"
      subtitle="Messages that could not be delivered after all retry attempts"
      statusFilter="Failed"
    />
  );
}