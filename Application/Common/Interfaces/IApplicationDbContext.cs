using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BoostOrder> BoostOrders { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}