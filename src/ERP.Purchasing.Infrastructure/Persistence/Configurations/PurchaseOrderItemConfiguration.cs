using ERP.Purchasing.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Purchasing.Infrastructure.Persistence.Configurations;
public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItemEntity>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItemEntity> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.SerialNumber).IsRequired();
        builder.Property(i => i.GoodCode).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.Currency).HasMaxLength(3).IsRequired().HasDefaultValue("USD");

        builder.HasIndex(i => i.PurchaseOrderId);
        builder.HasIndex(i => new { i.PurchaseOrderId, i.GoodCode }).IsUnique();
    }
}
