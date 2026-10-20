namespace Microsoft.OpenApi.Models;

public sealed class OpenApiOAuthFlow
{
    public string AuthorizationUrl { get; set; } = string.Empty;
}

public sealed class OpenApiOAuthFlows
{
    public OpenApiOAuthFlow? AuthorizationCode { get; set; }
}
