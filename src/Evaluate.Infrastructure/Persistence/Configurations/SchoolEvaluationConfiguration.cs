using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Infrastructure.Persistence.Configurations;

public class SchoolEvaluationConfiguration : IEntityTypeConfiguration<EvaluationEntity>
{
    public void Configure(EntityTypeBuilder<EvaluationEntity> builder)
    {
        // Named "Evaluation" (singular) — the dashboard's own demo entity already owns the
        // "Evaluations" table name, and EF has no other way to tell the two apart by default.
        builder.ToTable("Evaluation");
        builder.Property(e => e.TeacherUserId).IsRequired();
        builder.HasMany(e => e.Results).WithOne(r => r.Evaluation).HasForeignKey(r => r.EvaluationId);

        // The duplicate-evaluation guard and report-card queries both filter on this triple;
        // the pending-evaluations list filters by course/term/status instead.
        builder.HasIndex(e => new { e.StudentId, e.CourseId, e.TermId });
        builder.HasIndex(e => new { e.CourseId, e.TermId, e.Status });
    }
}
