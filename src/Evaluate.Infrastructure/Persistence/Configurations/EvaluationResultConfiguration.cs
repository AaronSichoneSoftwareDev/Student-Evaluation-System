using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaluationResultEntity = Evaluate.Domain.Entities.Evaluations.EvaluationResult;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class EvaluationResultConfiguration : IEntityTypeConfiguration<EvaluationResultEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationResultEntity> builder)
    {
        builder.HasOne(r => r.Topic).WithMany().HasForeignKey(r => r.TopicId);
    }
}
