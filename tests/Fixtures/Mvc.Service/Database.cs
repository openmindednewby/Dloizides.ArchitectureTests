using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mvc.Service;

public class ShopDbContext : DbContext;

public sealed class ArchiveDbContext(ShopDbContext source) : DbContext
{
    public bool Ready => source is not null;
}

public sealed class Invoice
{
    public int Id { get; set; }
}

public sealed class InvoiceRepository(ShopDbContext db)
{
    public async Task<bool> AnyAsync() => await db.Database.CanConnectAsync().ConfigureAwait(false);
}

public sealed class InvoiceStore(IDbContextFactory<ShopDbContext> factory)
{
    public bool Ready => factory is not null;
}

public sealed class InvoiceService(ShopDbContext db)
{
    public bool Ready => db is not null;
}

public sealed class InvoicesController
{
    private readonly IDbContextFactory<ShopDbContext> factory;

    public InvoicesController(IDbContextFactory<ShopDbContext> factory) => this.factory = factory;

    public bool Ready => factory is not null;
}

public sealed class InvoiceCleanupJob
{
    private ShopDbContext? db;

    public void Attach(ShopDbContext context) => db = context;

    public bool Ready => db is not null;
}

public sealed class InvoiceConfiguration(ShopDbContext db) : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder) => builder.HasKey(invoice => invoice.Id);

    public bool Ready => db is not null;
}

public sealed class ShopDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args) => new();
}

public sealed class ShopModelSnapshot(ShopDbContext db) : ModelSnapshot
{
    public bool Ready => db is not null;

    protected override void BuildModel(ModelBuilder modelBuilder) => modelBuilder.Entity<Invoice>();
}

public sealed class AddInvoices(ShopDbContext db) : Migration
{
    public bool Ready => db is not null;

    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("SELECT 1");
}
