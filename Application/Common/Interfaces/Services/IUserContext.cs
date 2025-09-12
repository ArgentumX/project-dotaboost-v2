using System.Security.Claims;
using Application.Users;

namespace Application.Common.Interfaces.Services;

public interface IUserContext
{
    Guid UserId { get; }
    string? UserEmail { get; }
    bool IsInRole(string role);
    IEnumerable<Claim> GetClaims();
    IEnumerable<string> GetRoles();
    UserDto GetUserDto();
}