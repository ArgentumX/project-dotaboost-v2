using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface IApplicationUser
{
    Guid Id { get; }
    string Email { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<Claim> Claims { get; }
    
}