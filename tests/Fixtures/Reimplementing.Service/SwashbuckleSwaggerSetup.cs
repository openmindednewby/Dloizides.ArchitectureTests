using System;
using Microsoft.OpenApi.Models;

namespace Reimplementing.Service;

public static class SwashbuckleSwaggerSetup
{
    public static Func<OpenApiOAuthFlows> Flows(string authority) => () => new OpenApiOAuthFlows { AuthorizationCode = new OpenApiOAuthFlow { AuthorizationUrl = $"{authority}/connect/authorize" } };
}
