using Nexuscomm.API.Models;

namespace Nexuscomm.API.Services.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
    }
}