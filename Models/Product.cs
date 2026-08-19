using MSSqlToPostgreSql.WebAPI.Abstractions;

namespace MSSqlToPostgreSql.WebAPI.Models;

public sealed class Product : Entity
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
