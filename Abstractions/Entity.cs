namespace MSSqlToPostgreSql.WebAPI.Abstractions;

public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
