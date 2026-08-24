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

        // Class-scoped lookups (who's enrolled in this class this year) and student-scoped
        // lookups (this student's active enrollment) are both hot paths — index both directions.
        builder.HasIndex(e => new { e.ClassId, e.AcademicYearId, e.Status });
        builder.HasIndex(e => new { e.StudentId, e.AcademicYearId, e.Status });

        // Year-wide "how many pupils are eligible for evaluation" counts (dashboard stat) scan
        // by year + status without a class/student filter — index that shape too.
        builder.HasIndex(e => new { e.AcademicYearId, e.Status });
    }
}
