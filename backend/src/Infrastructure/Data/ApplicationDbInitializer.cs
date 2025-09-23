using Domain.Constants;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data;


public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
    }
}

public class ApplicationDbInitializer
{
    private ILogger<ApplicationDbInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public ApplicationDbInitializer(ILogger<ApplicationDbInitializer> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task SeedAsync()
    {
        // Default roles
        var adminRole = new IdentityRole(Roles.Administrator);
        if (!await _roleManager.RoleExistsAsync(adminRole.Name!))
        {
            var r = await _roleManager.CreateAsync(adminRole);
            if (!r.Succeeded)
                throw new Exception(r.Errors.First().Description);
        }

        // Default users
        var email =  Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var username = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
        var password = Environment.GetEnvironmentVariable("ADMIN_PASS");
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Admin credentials not provided, skipping seeding.");
            return;
        }
        
        var administrator = await _userManager.FindByEmailAsync(email);
        if (administrator == null)
        {
            administrator = new ApplicationUser { UserName = username, Email = email, EmailConfirmed = true };
            var createRes = await _userManager.CreateAsync(administrator, password);
            if (!createRes.Succeeded)
                throw new Exception($"Failed to seed admin user: {createRes.Errors.First().Description}");
        }
        

        administrator = await _userManager.FindByIdAsync(administrator.Id);
        if (administrator == null)
            throw new Exception($"User with id {administrator?.Id} not found after creation — aborting.");
        
        if (!await _userManager.IsInRoleAsync(administrator, adminRole.Name!))
        {
            var addRoleRes = await _userManager.AddToRoleAsync(administrator, adminRole.Name!);
            if (!addRoleRes.Succeeded)
                throw new Exception($"Failed to add user {email} to role {adminRole.Name}: {addRoleRes.Errors}");
        }

    }
}