import MessageListPage from '../components/MessageListPage';

export default function AllMessages() {
  return (
    <MessageListPage
      title="All Messages"
      subtitle="Complete history of every communication"
      statusFilter={null}
    />
  );
}