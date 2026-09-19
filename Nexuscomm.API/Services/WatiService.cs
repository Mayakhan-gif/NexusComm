using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Nexuscomm.API.Configurations;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Services
{
    public class WatiService : IWatiService
    {
        private readonly HttpClient _httpClient;
        private readonly WatiSettings _settings;

        private static readonly Regex ValidNumberPattern = new(@"^\d{8,15}$", RegexOptions.Compiled);

        public WatiService(HttpClient httpClient, IOptions<WatiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl.TrimEnd('/') + "/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiToken);
        }

        public async Task<WhatsAppSendResult> SendMessageAsync(string whatsAppNumber, string messageText)
        {
            var cleanedNumber = whatsAppNumber.Replace("+", "").Replace(" ", "").Replace("-", "");

            if (!ValidNumberPattern.IsMatch(cleanedNumber))
            {
                return WhatsAppSendResult.Failure("Invalid WhatsApp number");
            }

            var encodedMessage = Uri.EscapeDataString(messageText);
            var requestUrl = $"api/v1/sendSessionMessage/{cleanedNumber}?messageText={encodedMessage}";

            HttpResponseMessage response;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                response = await _httpClient.PostAsync(requestUrl, content: null, cts.Token);
            }
            catch (TaskCanceledException)
            {
                return WhatsAppSendResult.Failure("WATI API timeout");
            }
            catch (HttpRequestException ex)
            {
                return WhatsAppSendResult.Failure("WATI API unavailable", ex.Message);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return WhatsAppSendResult.Success(providerResponse: responseBody);
            }

            var failureReason = response.StatusCode switch
            {
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "WATI authentication failed",
                HttpStatusCode.TooManyRequests => "WATI API rate limit exceeded",
                HttpStatusCode.BadRequest => "WATI rejected the message",
                HttpStatusCode.NotFound => "WATI endpoint not found - check configuration",
                >= HttpStatusCode.InternalServerError => "WATI API unavailable",
                _ => "WATI rejected the message"
            };

            return WhatsAppSendResult.Failure(failureReason, responseBody);
        }
    }
}