using Evaluate.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class TeacherCourseConfiguration : IEntityTypeConfiguration<TeacherCourse>
{
    public void Configure(EntityTypeBuilder<TeacherCourse> builder)
    {
        builder.Property(tc => tc.TeacherUserId).IsRequired();
        builder.HasOne(tc => tc.Course).WithMany().HasForeignKey(tc => tc.CourseId);

        // Powers "which teachers/subjects are assigned to this class" lookups (the Evaluations
        // page's class -> teacher cascade, and the report-card readiness check) without a table scan.
        builder.HasIndex(tc => new { tc.ClassId, tc.AcademicYearId, tc.IsActive });
    }
}
