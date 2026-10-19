namespace Clean.Web;

public sealed class CreateEventRequest
{
    public string Name { get; init; } = string.Empty;
}

public abstract class AbstractValidator<T>;

public sealed class CreateEventValidator : AbstractValidator<CreateEventRequest>;

public abstract class Endpoint<TRequest>;

public sealed class CreateEventEndpoint : Endpoint<CreateEventRequest>;
