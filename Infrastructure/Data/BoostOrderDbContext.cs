using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BoostOrderDbContext : DbContext, IBoostOrderDbContext
{
    public BoostOrderDbContext(DbContextOptions<BoostOrderDbContext> options) : base(options) { }
    
    public DbSet<BoostOrder> BoostOrders => Set<BoostOrder>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}