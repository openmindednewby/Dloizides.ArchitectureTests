using System;
using NSwag;

namespace Reimplementing.Service;

public static class SwaggerSetup
{
    public static Func<OpenApiOAuthFlow> Flow(string authority) => () => new OpenApiOAuthFlow { AuthorizationUrl = $"{authority}/protocol/openid-connect/auth" };
}
