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
    }
}
