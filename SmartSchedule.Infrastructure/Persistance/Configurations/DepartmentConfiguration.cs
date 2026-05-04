using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Infrastructure.Persistance.Common;

namespace SmartSchedule.Infrastructure.Persistance.Configurations
{
    public class DepartmentConfiguration : BaseEntityConfiguration<Department>
    {
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            base.Configure(builder);

            builder.ToTable("Departments");

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(d => d.Name)
                .IsUnique()
                .HasFilter("\"DeletedAtUtc\" IS NULL");
        }
    }
}