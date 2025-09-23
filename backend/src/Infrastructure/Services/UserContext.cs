using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Users;
using Infrastructure.Exceptions;
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
                                    ?? throw new UnauthorizedException();

    public Guid UserId
    {
        get
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new UnauthorizedException();

            return Guid.Parse(idClaim);
        }
    }

    public string? UserEmail => User.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsInRole(string role)
    {
        return User.IsInRole(role);
    }

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
        return new UserDto
        {
            Id = UserId,
            Email = UserEmail,
            Roles = GetRoles(),
            Claims = GetClaims()
        };
    }
}