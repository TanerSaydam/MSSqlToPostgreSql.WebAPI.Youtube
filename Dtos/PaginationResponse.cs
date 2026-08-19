namespace MSSqlToPostgreSql.WebAPI.Dtos;

public sealed record PaginationResponse<T>(
    int PageNumber,
    int PageSize,
    int TotalCount,
    List<T> Data);