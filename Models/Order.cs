using MSSqlToPostgreSql.WebAPI.Abstractions;

namespace MSSqlToPostgreSql.WebAPI.Models;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = new();
    public string OrderNumber { get; set; } = default!;
    public DateOnly Date { get; set; }
    public OrderStatusEnum OrderStatus { get; set; } = OrderStatusEnum.Pending;
    public IReadOnlyCollection<OrderItem> Items => _items.ToArray();

    public void AddItem(OrderItem orderItem)
    {
        _items.Add(orderItem);
    }

    public void RemoveItem(OrderItem orderItem)
    {
        _items.Remove(orderItem);
    }
}

public sealed record OrderItem(Guid ProductId, int Quantity, decimal Price);

public enum OrderStatusEnum
{
    Pending = 0,
    Approve = 1,
    Transfer = 2,
    Completed = 3,
    Rejected = 4,
    Cancelled = 5,
}
