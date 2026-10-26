using System;

namespace Microsoft.AspNetCore.Mvc
{
    public abstract class ControllerBase;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromBodyAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromQueryAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromRouteAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromServicesAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RouteAttribute(string template) : Attribute
    {
        public string Template { get; } = template;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NonActionAttribute : Attribute;
}

namespace FluentValidation
{
    public interface IValidator<T>;

    public abstract class AbstractValidator<T> : IValidator<T>;
}
