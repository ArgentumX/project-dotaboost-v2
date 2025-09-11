using Application.Users;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJwtToken(UserDto user);
}