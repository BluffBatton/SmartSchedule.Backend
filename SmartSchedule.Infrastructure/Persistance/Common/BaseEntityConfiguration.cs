using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Common;

namespace SmartSchedule.Infrastructure.Persistance.Common
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAtUtc)
                   .IsRequired();
            builder.Property(e => e.UpdatedAtUtc);
            builder.Property(e => e.DeletedAtUtc);

            builder.HasQueryFilter(e => e.DeletedAtUtc == null);
        }
    }
}
