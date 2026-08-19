using MSSqlToPostgreSql.WebAPI.Models;

namespace MSSqlToPostgreSql.WebAPI.Dtos;

public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    DateOnly Date,
    OrderStatusEnum OrderStatus,
    string OrderStatusName,
    List<OrderItemDto> Items);

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price);