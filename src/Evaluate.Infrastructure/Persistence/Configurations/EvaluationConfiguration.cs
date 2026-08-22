using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
{
    public void Configure(EntityTypeBuilder<Evaluation> builder)
    {
        builder.Property(e => e.ReferenceCode).HasMaxLength(20);
        builder.Property(e => e.Subject).HasMaxLength(100);
        builder.HasOne(e => e.Student).WithMany().HasForeignKey(e => e.StudentId);
    }
}
