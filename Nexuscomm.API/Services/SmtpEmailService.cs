using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Nexuscomm.API.Configurations;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailService(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<EmailSendResult> SendEmailAsync(string toEmail, string subject, string body)
        {
            if (!MailboxAddress.TryParse(toEmail, out var toAddress))
            {
                return EmailSendResult.Failure("Invalid email address");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(toAddress);
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();

            try
            {
                var socketOptions = _settings.EnableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

                await client.ConnectAsync(_settings.Host, _settings.Port, socketOptions);
            }
            catch (SmtpCommandException ex)
            {
                return EmailSendResult.Failure("SMTP connection failed", ex.Message);
            }
            catch (SmtpProtocolException ex)
            {
                return EmailSendResult.Failure("SMTP server unavailable", ex.Message);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                return EmailSendResult.Failure("Network error", ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                return EmailSendResult.Failure("SMTP timeout", ex.Message);
            }

            try
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }
            catch (AuthenticationException ex)
            {
                await SafeDisconnectAsync(client);
                return EmailSendResult.Failure("SMTP authentication failed", ex.Message);
            }

            try
            {
                var response = await client.SendAsync(message);
                await SafeDisconnectAsync(client);
                return EmailSendResult.Success(providerResponse: response);
            }
            catch (SmtpCommandException ex)
            {
                await SafeDisconnectAsync(client);
                return EmailSendResult.Failure("SMTP server rejected the message", ex.Message);
            }
            catch (Exception ex)
            {
                await SafeDisconnectAsync(client);
                return EmailSendResult.Failure("External API error", ex.Message);
            }
        }

        private static async Task SafeDisconnectAsync(SmtpClient client)
        {
            try
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }
            catch
            {
                // Ignore disconnect errors - we already have our result
            }
        }
    }
}