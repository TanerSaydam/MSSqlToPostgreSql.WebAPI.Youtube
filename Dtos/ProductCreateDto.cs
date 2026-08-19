namespace MSSqlToPostgreSql.WebAPI.Dtos;

public sealed record ProductCreateDto(
    string Name,
    decimal Price);