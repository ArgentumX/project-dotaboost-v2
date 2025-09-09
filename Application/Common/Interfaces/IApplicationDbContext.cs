using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BoostOrder> BoostOrders { get; }
    DbSet<Batch> Batches { get; }
    DbSet<Booster> Boosters { get; }
    DbSet<BoosterApplication> BoosterApplications { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}