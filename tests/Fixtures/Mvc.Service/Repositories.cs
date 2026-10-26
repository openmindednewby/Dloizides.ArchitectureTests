namespace Mvc.Service;

public interface IRepository<T>;

public interface ICustomerRepository;

public sealed class CustomerRepository : ICustomerRepository;

public sealed class OrderRepository(ICustomerRepository customers)
{
    public ICustomerRepository Customers { get; } = customers;
}

public sealed class LedgerStore(IRepository<Account> accounts) : IRepository<LedgerStore>
{
    public IRepository<Account> Accounts { get; } = accounts;
}

public interface IProductRepository;

public sealed class ProductRepository : IProductRepository;

public sealed class CachedProductRepository(IProductRepository inner) : IProductRepository
{
    public IProductRepository Inner { get; } = inner;
}

public sealed class CachedPriceRepository(IProductRepository inner, ICustomerRepository customers) : IProductRepository
{
    public IProductRepository Inner { get; } = inner;

    public ICustomerRepository Customers { get; } = customers;
}

public sealed class OrderService(ICustomerRepository customers, OrderRepository orders)
{
    public ICustomerRepository Customers { get; } = customers;

    public OrderRepository Orders { get; } = orders;
}
