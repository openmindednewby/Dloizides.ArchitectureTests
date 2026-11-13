using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace Mvc.Service;

public interface IBillingService;

public sealed class BillingController(IBillingService invoices) : ControllerBase
{
    public override string ToString() => invoices.ToString() ?? string.Empty;
}

public sealed class CustomersController(ICustomerRepository customers) : ControllerBase
{
    public override string ToString() => customers.ToString() ?? string.Empty;
}

public sealed class ProductListEndpoint(IProductRepository products) : Endpoint<object, string>
{
    public override string ToString() => products.ToString() ?? string.Empty;
}

public sealed class OrderExportEndpoint : EndpointWithoutRequest
{
    public OrderExportEndpoint(IBillingService invoices, OrderRepository orders) => Label = $"{invoices}{orders}";

    public string Label { get; }
}

public sealed class BillListEndpoint(IBillingService invoices) : Endpoint<object, string>
{
    public override string ToString() => invoices.ToString() ?? string.Empty;
}

public sealed class LedgerReportService(ICustomerRepository customers)
{
    public override string ToString() => customers.ToString() ?? string.Empty;
}
