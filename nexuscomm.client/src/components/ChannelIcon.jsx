import { MdEmail } from 'react-icons/md';
import { FaWhatsapp } from 'react-icons/fa';

export default function ChannelIcon({ channel, size = 18 }) {
  if (channel === 'WhatsApp' || channel === 1) {
    return <FaWhatsapp size={size} color="var(--color-whatsapp)" />;
  }
  return <MdEmail size={size} color="var(--color-email)" />;
}