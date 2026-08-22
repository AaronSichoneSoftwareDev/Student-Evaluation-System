using Evaluate.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
    {
        builder.HasOne(e => e.AcademicYear).WithMany().HasForeignKey(e => e.AcademicYearId);
        builder.HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId);
    }
}
