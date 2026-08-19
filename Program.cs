using Microsoft.EntityFrameworkCore;
using MSSqlToPostgreSql.WebAPI.Context;
using MSSqlToPostgreSql.WebAPI.Dtos;
using MSSqlToPostgreSql.WebAPI.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    //opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    opt.UseSnakeCaseNamingConvention();
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapPost("/products", async (
    ProductCreateDto request,
    ApplicationDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    Product product = new()
    {
        Name = request.Name,
        Price = request.Price,
        CreatedAt = DateTime.Now
    };

    dbContext.Add(product);
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(product);
});


app.MapGet("/products", async (
    int pageNumber = 1,
    int pageSize = 10,
    ApplicationDbContext dbContext = default!,
    CancellationToken cancellationToken = default) =>
{
    var query = dbContext
        .Set<Product>()
        .AsNoTracking();

    var totalCount = await query.CountAsync(cancellationToken);

    var products = await query
        .OrderBy(x => x.Name)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new ProductDto(
            x.Id,
            x.Name,
            x.Price,
            x.CreatedAt,
            x.UpdatedAt
            ))
        .ToListAsync(cancellationToken);

    var response = new PaginationResponse<ProductDto>(
        pageNumber,
        pageSize,
        totalCount,
        products);

    return Results.Ok(response);
});

app.MapGet("/orders", async (
    int pageNumber = 1,
    int pageSize = 10,
    ApplicationDbContext dbContext = default!,
    CancellationToken cancellationToken = default) =>
{
    var query = dbContext
        .Set<Order>()
        .AsNoTracking();

    var totalCount = await query.CountAsync(cancellationToken);

    var orders = await query
        .OrderByDescending(x => x.Date)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new OrderDto(
            x.Id,
            x.OrderNumber,
            x.Date,
            x.OrderStatus,
            x.OrderStatus.ToString(),
            x.Items.Select(i => new OrderItemDto(
                i.ProductId,
                dbContext.Set<Product>()
                    .Where(p => p.Id == i.ProductId)
                    .Select(p => p.Name)
                    .FirstOrDefault()!,
                i.Quantity,
                i.Price
            )).ToList()
        ))
        .ToListAsync(cancellationToken);

    var response = new PaginationResponse<OrderDto>(
        pageNumber,
        pageSize,
        totalCount,
        orders);

    return Results.Ok(response);
});

//app.MapGet("/seed-data", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
//{
//    if (dbContext.Set<Product>().Any())
//    {
//        return Results.NoContent();
//    }

//    List<Product> products = new();
//    for (int i = 0; i < 10000; i++)
//    {
//        var faker = new Faker();
//        Product product = new()
//        {
//            Name = faker.Commerce.ProductName() + i,
//            Price = faker.Commerce.Random.Decimal(100, 100000),
//            CreatedAt = DateTime.Now
//        };
//        products.Add(product);
//    }
//    dbContext.AddRange(products);

//    List<Order> orders = new();
//    for (int i = 0; i < 1000; i++)
//    {
//        Random random = new();
//        var statusNum = random.Next(0, 6);

//        Faker faker = new();
//        var startDate = new DateTime(2026, 1, 1);
//        var endDate = new DateTime(2026, 12, 31);
//        var randomDate = faker.Date.Between(startDate, endDate);

//        Order order = new()
//        {
//            OrderNumber = "SP" + DateTime.Now.Year + (i + 1).ToString("0000000000"),
//            Date = DateOnly.FromDateTime(randomDate),
//            CreatedAt = DateTime.Now,
//            OrderStatus = (OrderStatusEnum)statusNum,
//        };

//        var itemCount = random.Next(1, 11);
//        for (int x = 0; x < itemCount; x++)
//        {
//            var nextProductId = random.Next(0, products.Count);
//            var quantity = random.Next(1, 100);
//            var item = new OrderItem(products[nextProductId].Id, quantity, products[nextProductId].Price);
//            order.AddItem(item);
//        }
//        orders.Add(order);
//    }
//    dbContext.AddRange(orders);
//    var result = await dbContext.SaveChangesAsync(cancellationToken);

//    return Results.NoContent();
//});

app.MapPost("/sync", async (
    IConfiguration configuration,
    CancellationToken cancellationToken) =>
{
    var sqlServerOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseSqlServer(configuration.GetConnectionString("SqlServer"))
        .Options;

    var postgresOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseNpgsql(configuration.GetConnectionString("Postgres"))
        .UseSnakeCaseNamingConvention()
        .Options;

    await using var sqlDb = new ApplicationDbContext(sqlServerOptions);
    await using var postgresDb = new ApplicationDbContext(postgresOptions);

    // PostgreSQL doluysa tekrar ekleme
    if (await postgresDb.Set<Product>().AnyAsync(cancellationToken))
    {
        return Results.BadRequest("PostgreSQL veritabanında zaten veri var.");
    }

    var products = await sqlDb
        .Set<Product>()
        .AsNoTracking()
        .ToListAsync(cancellationToken);

    var orders = await sqlDb
        .Set<Order>()
        .AsNoTracking()
        .Include(x => x.Items)
        .ToListAsync(cancellationToken);

    postgresDb.AddRange(products);
    postgresDb.AddRange(orders);

    await postgresDb.SaveChangesAsync(cancellationToken);

    return Results.Ok(new
    {
        ProductCount = products.Count,
        OrderCount = orders.Count
    });
});

app.Run();
