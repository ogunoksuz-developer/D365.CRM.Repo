using Microsoft.EntityFrameworkCore;
using Pars.Core.Data.EfCore.ParsCoreContext;
using Pars.CS.DynamicsToKafka.Producer.Data;

namespace Pars.CS.DynamicsToKafka.Producer.API.Data.EF;

public class TemplateContext : ParsCoreDbContext
{
    public TemplateContext(DbContextOptions<TemplateContext> options) : base(options)
    { }



    public DbSet<Basket> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            if (entityType.ClrType.GetCustomAttributesData().Count == 0)
                modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);

        modelBuilder.Entity<BasketProduct>(entity =>
        {
            entity.ToTable("BasketProduct");
            entity.HasOne(d => d.Basket)
             .WithMany(p => p.BasketProducts)
             .HasForeignKey(d => d.BasketId)
             .HasConstraintName("FK_BasketProduct_Basket");

            entity.HasOne(d => d.Product)
              .WithMany(p => p.BasketProducts)
              .HasForeignKey(d => d.ProductId)
              .HasConstraintName("FK_BasketProduct_Product");

        });
    }
}
