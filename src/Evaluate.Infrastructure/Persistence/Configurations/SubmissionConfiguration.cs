using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.Property(s => s.ReferenceCode).HasMaxLength(20);
        builder.Property(s => s.Subject).HasMaxLength(100);
        builder.HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId);
    }
}
