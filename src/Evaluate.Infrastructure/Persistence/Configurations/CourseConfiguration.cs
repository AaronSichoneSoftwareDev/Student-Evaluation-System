using Evaluate.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.CourseCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(c => c.CourseCode).IsUnique();
        builder.Property(c => c.CourseName).HasMaxLength(150).IsRequired();
        builder.HasMany(c => c.Topics).WithOne(t => t.Course).HasForeignKey(t => t.CourseId);
    }
}
