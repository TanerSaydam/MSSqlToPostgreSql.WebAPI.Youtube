namespace MSSqlToPostgreSql.WebAPI.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    DateTime CreatedAt,
    DateTime? UpdatedAt);