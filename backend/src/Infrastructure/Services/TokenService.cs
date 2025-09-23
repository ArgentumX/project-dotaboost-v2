using System.Text;
using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Users;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

public async Task<string> GenerateJwtToken(UserDto user)
{
    var jwtSection = _configuration.GetSection("Jwt");
    var keyString = jwtSection.GetValue<string>("Key");
    var issuer = jwtSection.GetValue<string>("Issuer");
    var audience = jwtSection.GetValue<string>("Audience");
    var expireMinutes = jwtSection.GetValue<int>("ExpireMinutes", 60);

    if (string.IsNullOrWhiteSpace(keyString))
        throw new InvalidOperationException("JWT key is not configured.");
    var key = Encoding.UTF8.GetBytes(keyString);
    
    if (key.Length < 32)
        throw new InvalidOperationException("JWT Key is too short. Use at least 256-bit key for HmacSha256.");
    
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),            
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };
    
    var roles = user.Roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
    
    if (roles.Any())
    {
        var rolesJson = System.Text.Json.JsonSerializer.Serialize(roles);
        claims.Add(new Claim("roles", rolesJson, JsonClaimValueTypes.Json));
    }
    
    var existingTypes = new HashSet<string>(claims.Select(c => c.Type));
    foreach (var c in user.Claims)
    {
        if (existingTypes.Contains(c.Type) && c.Value == claims.First(cl => cl.Type == c.Type).Value) continue;
        claims.Add(new Claim(c.Type, c.Value));
        existingTypes.Add(c.Type);
    }

    var signingKey = new SymmetricSecurityKey(key);
    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var now = DateTime.UtcNow;
    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        notBefore: now,
        expires: now.AddMinutes(expireMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}