namespace Dirty.Domain;

public sealed class RefundRequest
{
    public decimal Amount { get; init; }

    public int TenantId { get; init; }
}
