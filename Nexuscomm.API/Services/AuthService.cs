using Microsoft.AspNetCore.Identity;
using Nexuscomm.API.DTOs;
using Nexuscomm.API.Exceptions;
using Nexuscomm.API.Models;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        private const string DefaultRole = "User";

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser is not null)
            {
                throw new ValidationException("An account with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            var (token, expiresAt) = _tokenService.GenerateToken(user, new List<string> { DefaultRole });

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email!,
                Role = DefaultRole,
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null || !user.IsActive)
            {
                throw new ValidationException("Invalid email or password.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                throw new ValidationException("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? DefaultRole,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}