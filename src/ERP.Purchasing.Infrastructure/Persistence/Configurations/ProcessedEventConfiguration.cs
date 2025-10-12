﻿using ERP.Purchasing.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Purchasing.Infrastructure.Persistence.Configurations;
public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.ToTable("ProcessedEvents");
        builder.HasKey(e => e.EventId);
        builder.Property(e => e.ProcessedAt).IsRequired();
        builder.Property(e => e.EventType).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.ProcessedAt);
    }
}
