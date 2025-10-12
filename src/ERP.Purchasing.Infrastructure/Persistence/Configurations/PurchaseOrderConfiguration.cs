using ERP.Purchasing.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Purchasing.Infrastructure.Persistence.Configurations;
public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrderEntity>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderEntity> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(po => po.Id);

        builder.Property(po => po.Number).IsRequired().HasMaxLength(50);
        builder.HasIndex(po => po.Number).IsUnique();

        builder.Property(po => po.IssueDate).IsRequired();
        builder.Property(po => po.TotalPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(po => po.Currency).HasMaxLength(3).IsRequired().HasDefaultValue("USD");
        builder.Property(po => po.State).IsRequired();
        builder.Property(po => po.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(po => po.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(po => po.RowVersion).IsRowVersion();

        builder.HasMany(po => po.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(po => po.State);
        builder.HasIndex(po => po.IsActive);
        builder.HasIndex(po => po.IssueDate);
        builder.HasIndex(po => new { po.State, po.IsActive });
    }
}
