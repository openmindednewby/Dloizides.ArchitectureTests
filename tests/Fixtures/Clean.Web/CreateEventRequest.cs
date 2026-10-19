namespace Clean.Web;

public sealed class CreateEventRequest
{
    public string Name { get; init; } = string.Empty;
}

public sealed class CreateEventValidator : FastEndpoints.Validator<CreateEventRequest>;

public abstract class Endpoint<TRequest>;

public sealed class CreateEventEndpoint : Endpoint<CreateEventRequest>;
