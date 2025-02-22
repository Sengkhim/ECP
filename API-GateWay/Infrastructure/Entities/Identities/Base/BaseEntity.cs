namespace API_GateWay.Infrastructure.Entities.Identities.Base;

public abstract class BaseEntity<T>
{
    public required T Id { get; init; }
}