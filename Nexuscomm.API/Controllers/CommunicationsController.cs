using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexuscomm.API.DTOs;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Controllers
{
    [ApiController]
    [Route("api/communications")]
    [Authorize]
    public class CommunicationsController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;

        public CommunicationsController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommunicationDto dto)
        {
            var result = await _communicationService.CreateAsync(CurrentUserId, dto);
            return Ok(ApiResponseDto<CommunicationResponseDto>.SuccessResponse(
                result, "Communication created successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = IsAdmin
                ? await _communicationService.GetAllAsync()
                : await _communicationService.GetAllForUserAsync(CurrentUserId);

            return Ok(ApiResponseDto<List<CommunicationListDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _communicationService.GetByIdAsync(id, CurrentUserId, IsAdmin);
            return Ok(ApiResponseDto<CommunicationResponseDto>.SuccessResponse(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommunicationDto dto)
        {
            var result = await _communicationService.UpdateAsync(id, CurrentUserId, IsAdmin, dto);
            return Ok(ApiResponseDto<CommunicationResponseDto>.SuccessResponse(
                result, "Communication updated successfully."));
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _communicationService.CancelAsync(id, CurrentUserId, IsAdmin);
            return Ok(ApiResponseDto<object>.SuccessResponse(new { }, "Communication cancelled successfully."));
        }

        [HttpGet("{id}/attempts")]
        public async Task<IActionResult> GetAttempts(int id)
        {
            var result = await _communicationService.GetAttemptsAsync(id, CurrentUserId, IsAdmin);
            return Ok(ApiResponseDto<List<CommunicationAttemptDto>>.SuccessResponse(result));
        }

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token.");

        private bool IsAdmin => User.IsInRole("Admin");
    }
}