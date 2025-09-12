using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<BoostOrder> BoostOrders => Set<BoostOrder>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Booster> Boosters => Set<Booster>();
    public DbSet<BoosterApplication> BoosterApplications => Set<BoosterApplication>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}