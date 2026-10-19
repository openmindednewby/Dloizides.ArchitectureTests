namespace Dirty.Web;

public sealed class GetOrderRequest
{
    public int OrderId { get; init; }
}

public abstract class Endpoint<TRequest, TResponse>;

public sealed class PlaceOrderEndpoint : Endpoint<PlaceOrderRequest, string>;

public sealed class GetOrderEndpoint : Endpoint<GetOrderRequest, string>;
