using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class EvaluationCriteriaConfiguration : IEntityTypeConfiguration<EvaluationCriteriaEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationCriteriaEntity> builder)
    {
        builder.Property(c => c.CriteriaName).HasMaxLength(150).IsRequired();
        builder.HasOne(c => c.Course).WithMany().HasForeignKey(c => c.CourseId);
    }
}
