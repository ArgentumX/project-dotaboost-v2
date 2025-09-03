using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BoostOrderDbInitializer
{
    public static void Initialize(BoostOrderDbContext context)
    {
        context.Database.EnsureCreated();
    }
}