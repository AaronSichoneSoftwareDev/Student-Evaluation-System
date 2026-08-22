using Evaluate.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Named "Student" (singular) — the dashboard's own demo entity already owns the
        // "Students" table name, and EF has no other way to tell the two apart by default.
        builder.ToTable("Student");
        builder.Property(s => s.StudentNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.StudentNumber).IsUnique();
        builder.Property(s => s.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.LastName).HasMaxLength(100).IsRequired();
        builder.Ignore(s => s.FullName);
        builder.HasMany(s => s.Enrollments).WithOne(e => e.Student).HasForeignKey(e => e.StudentId);
    }
}
