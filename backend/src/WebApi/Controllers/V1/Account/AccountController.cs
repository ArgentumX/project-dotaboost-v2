using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Users;
using Application.Users.GenerateJwtToken;
using Asp.Versioning;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
// где определён ApplicationUser
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApi.Controllers.V1.Account;

[ApiVersion(1.0)]
public class AccountController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var userResult = await _userManager.CreateAsync(user, model.Password);
        if (!userResult.Succeeded) return BadRequest(userResult.Errors);

        var userDto = new UserDto
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user),
            Claims = await _userManager.GetClaimsAsync(user)
        };
        var command = new GenerateJwtTokenCommand(userDto);
        var result = await Mediator.Send(command);
        return Ok(new { token=result, user=userDto });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var userResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!userResult.Succeeded) return Unauthorized("Invalid credentials");

        var userDto = new UserDto
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user),
            Claims = await _userManager.GetClaimsAsync(user)
        };
        var command = new GenerateJwtTokenCommand(userDto);
        var result = await Mediator.Send(command);
        return Ok(new { token=result, user=userDto });
    }
}

public record RegisterDto(string Email, string Password);

public record LoginDto(string Email, string Password);