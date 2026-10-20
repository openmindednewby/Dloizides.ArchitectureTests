namespace Mvc.Service;

public sealed class OpenCaseRequest
{
    public string Subject { get; init; } = string.Empty;
}

public sealed class CloseCaseRequest
{
    public string Reason { get; init; } = string.Empty;
}

public sealed class CaseSearchQuery
{
    public string Term { get; init; } = string.Empty;
}

public sealed class CaseLookup
{
    public int CaseId { get; init; }
}

public sealed class HelperInput
{
    public string Note { get; init; } = string.Empty;
}

public sealed class InjectedClock
{
    public string Zone { get; init; } = string.Empty;
}

public sealed class OpenCaseValidator : FluentValidation.AbstractValidator<OpenCaseRequest>;
