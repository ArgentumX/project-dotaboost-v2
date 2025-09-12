using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BoostOrderConfiguration : IEntityTypeConfiguration<BoostOrder>
{
    public void Configure(EntityTypeBuilder<BoostOrder> builder)
    {
    }
}