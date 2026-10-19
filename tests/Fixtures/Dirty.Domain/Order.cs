namespace Dirty.Domain;

public sealed class Order
{
    public Dirty.Application.OrderHandler? Uses { get; init; }
}
