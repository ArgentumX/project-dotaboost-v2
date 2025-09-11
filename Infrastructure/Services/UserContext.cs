using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Users;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User 
                                    ?? throw new InvalidOperationException("No user context available");

    public Guid UserId
    {
        get
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new InvalidOperationException("User ID claim not found");

            return Guid.Parse(idClaim);
        }
    }

    public string? UserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsInRole(string role) => User.IsInRole(role);
    public IEnumerable<Claim> GetClaims()
    {
        return User.Claims;
    }

    public IEnumerable<string> GetRoles()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
    }

    public UserDto GetUserDto()
    {
        return new UserDto()
        {
            Id = UserId,
            Email = UserEmail,
            Roles = GetRoles(),
            Claims = GetClaims()
        };
    }
}