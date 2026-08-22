using Evaluate.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.Property(c => c.ClassName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.GradeLevel).HasMaxLength(50).IsRequired();
    }
}
