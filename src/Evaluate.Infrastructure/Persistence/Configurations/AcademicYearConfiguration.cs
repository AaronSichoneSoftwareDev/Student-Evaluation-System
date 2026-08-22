using Evaluate.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.Property(y => y.YearName).HasMaxLength(50).IsRequired();
        builder.HasIndex(y => y.YearName).IsUnique();
        builder.HasMany(y => y.Terms).WithOne(t => t.AcademicYear).HasForeignKey(t => t.AcademicYearId);
    }
}
