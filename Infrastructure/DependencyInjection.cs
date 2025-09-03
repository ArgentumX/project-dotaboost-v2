using Application.Common.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<BoostOrderDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
        services.AddScoped<IBoostOrderDbContext>(provider => provider.GetService<BoostOrderDbContext>());
    }
}