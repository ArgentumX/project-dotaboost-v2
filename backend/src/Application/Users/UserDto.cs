using System.Security.Claims;

namespace Application.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
    public IEnumerable<string> Roles { get; set; }
}