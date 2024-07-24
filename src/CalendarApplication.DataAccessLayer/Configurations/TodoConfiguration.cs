using CalendarApplication.DataAccessLayer.Configurations.Common;
using CalendarApplication.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarApplication.DataAccessLayer.Configurations;

internal class TodoConfiguration : BaseEntityConfiguration<Todo>
{
    public override void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2048).IsRequired(false);
        builder.Property(t => t.UserId).IsRequired();

        builder.ToTable("Todos");
        base.Configure(builder);
    }
}