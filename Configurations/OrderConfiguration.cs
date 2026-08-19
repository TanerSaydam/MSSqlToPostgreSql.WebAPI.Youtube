using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSqlToPostgreSql.WebAPI.Models;

namespace MSSqlToPostgreSql.WebAPI.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.Property(p => p.OrderNumber).HasColumnType("varchar(16)");
        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.OwnsMany(i => i.Items);
    }
}
