import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import CreateCommunication from './pages/CreateCommunication';
import ScheduledMessages from './pages/ScheduledMessages';
import SentMessages from './pages/SentMessages';
import FailedMessages from './pages/FailedMessages';
import AllMessages from './pages/AllMessages';
import MessageDetails from './pages/MessageDetails';
import ProtectedRoute from './components/ProtectedRoute';
import MainLayout from './components/Layout/MainLayout';

function withLayout(Component) {
  return (
    <ProtectedRoute>
      <MainLayout>
        <Component />
      </MainLayout>
    </ProtectedRoute>
  );
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />

      <Route path="/dashboard" element={withLayout(Dashboard)} />
      <Route path="/create" element={withLayout(CreateCommunication)} />
      <Route path="/scheduled" element={withLayout(ScheduledMessages)} />
      <Route path="/sent" element={withLayout(SentMessages)} />
      <Route path="/failed" element={withLayout(FailedMessages)} />
      <Route path="/all" element={withLayout(AllMessages)} />
      <Route path="/messages/:id" element={withLayout(MessageDetails)} />

      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

export default App;