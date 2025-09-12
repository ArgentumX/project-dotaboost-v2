using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();
    }
}