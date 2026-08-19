using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSqlToPostgreSql.WebAPI.Models;

namespace MSSqlToPostgreSql.WebAPI.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.Property(p => p.Name).HasColumnType("varchar(100)");
        builder.Property(p => p.Price).HasColumnType("money");
        builder.HasIndex(x => x.Name).IsUnique(false);
    }
}