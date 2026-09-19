using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexuscomm.API.DTOs;
using Nexuscomm.API.Enums;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;

        public DashboardController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token.");

            var messages = isAdmin
                ? await _communicationService.GetAllAsync()
                : await _communicationService.GetAllForUserAsync(userId);

            var summary = new DashboardSummaryDto
            {
                Total = messages.Count,
                Scheduled = messages.Count(m => m.Status == MessageStatus.Scheduled),
                Processing = messages.Count(m => m.Status == MessageStatus.Processing),
                Sent = messages.Count(m => m.Status == MessageStatus.Sent),
                Retrying = messages.Count(m => m.Status == MessageStatus.Retrying),
                Failed = messages.Count(m => m.Status == MessageStatus.Failed),
                Cancelled = messages.Count(m => m.Status == MessageStatus.Cancelled),
                RecentCommunications = messages.OrderByDescending(m => m.CreatedAt).Take(10).ToList()
            };

            return Ok(ApiResponseDto<DashboardSummaryDto>.SuccessResponse(summary));
        }
    }
}