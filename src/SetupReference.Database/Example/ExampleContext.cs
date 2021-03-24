using Microsoft.EntityFrameworkCore;
using SetupReference.Database.ExampleContext.Entities;

namespace SetupReference.Database.Example
{
    public class ExampleContext : DbContext
    {
        private readonly int tenantId;

        // Provided for ExampleContextFactory to create instances without the ITenantProvider
        public ExampleContext(DbContextOptions<ExampleContext> dbContextOptions) : base(dbContextOptions) { }

        public ExampleContext(DbContextOptions<ExampleContext> dbContextOptions, ITenantProvider<int> tenantProvider) : base(dbContextOptions)
        {
            tenantId = tenantProvider.TenantId;
        }

        public DbSet<NoteEntity> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteEntity>(entity =>
            {
                // ToTable() require Microsoft.EntityFrameworkCore.Relational
                entity.ToTable("Notes");

                entity.HasKey(e => e.Id);

                // UseIdentityColumn() requires Microsoft.EntityFrameworkCore.SqlServer
                entity.Property(e => e.Id).UseIdentityColumn();

                entity.Property(e => e.TenantId).IsRequired();

                entity.HasIndex(e => new { e.Id, e.DueDate });

                // Simple approach. If this gets out of control a dynamic approach is shown
                // here: https://gunnarpeipman.com/aspnet-core-tenant-providers/
                entity.HasQueryFilter(e => e.TenantId == tenantId);
            });
        }

    }
}
