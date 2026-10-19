namespace Clean.Web;

public sealed class CreateEventRequest
{
    public string Name { get; init; } = string.Empty;
}

public abstract class AbstractValidator<T>;

public sealed class CreateEventValidator : AbstractValidator<CreateEventRequest>;
