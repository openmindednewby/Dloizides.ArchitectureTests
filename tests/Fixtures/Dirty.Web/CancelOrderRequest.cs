namespace Dirty.Web;

public sealed class CancelOrderRequest
{
    public string Reason { get; init; } = string.Empty;
}

public abstract class AbstractValidator<T>;

public sealed class CancelOrderValidator : AbstractValidator<CancelOrderRequest>;

public sealed class CancelOrderEndpoint : Endpoint<CancelOrderRequest, string>;
