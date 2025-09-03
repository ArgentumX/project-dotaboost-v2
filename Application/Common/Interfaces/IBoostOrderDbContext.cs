using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IBoostOrderDbContext
{
    DbSet<BoostOrder> BoostOrders { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}