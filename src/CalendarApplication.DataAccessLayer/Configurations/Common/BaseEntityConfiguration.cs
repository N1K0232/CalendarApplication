using CalendarApplication.DataAccessLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarApplication.DataAccessLayer.Configurations.Common;

internal abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("newid()");

        builder.Property(x => x.CreationDate).ValueGeneratedOnAdd().HasDefaultValueSql("getutcdate()");
        builder.Property(x => x.LastModificationDate).ValueGeneratedOnUpdate();
    }
}