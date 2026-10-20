using System.Collections.Generic;

namespace Dloizides.ArchitectureTests;

/// <summary>QUAL-ADOPT-1 "Fail lint/arch tests when an app re-implements an existing package": the registry.</summary>
public static class ReimplementedPackages
{
    /// <summary>Every known re-implementation with the package that owns it.</summary>
    public static IReadOnlyList<ReimplementedPackage> All { get; } =
    [
        new("realm-authorization", "Identity.Keycloak", "builder.AddRealmAuthorization() from Identity.Keycloak.Authorization", ["RealmAuthorizationHandler", "RealmAuthorizationExtensions"], []),
        new("api-key-auth", "Dloizides.ApiKeys", "AddApiKeyScheme() with an IApiKeyStore from Dloizides.ApiKeys", ["ApiKeyAuthMiddleware", "ApiKeyAuthenticationHandler"], []),
        new("swagger-oauth2-pkce", "ApiDocs.AspNetCore", "the shared OAuth2/PKCE docs setup in ApiDocs.AspNetCore (DUP-SCAN-1 cluster 1 moves it there; exempt with a reason until then)", [], ["NSwag.OpenApiOAuthFlow"]),
        new("log-stress-endpoint", "Logging.Client", "nothing: delete the endpoint, Logging.Client owns log shipping", ["LogStressEndpoint", "LogStressRequest", "LogStressRequestValidator"], []),
    ];
}
