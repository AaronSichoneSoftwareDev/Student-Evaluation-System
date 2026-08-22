using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;

namespace Evaluate.Application.EvaluationCriteria.Commands.CreateEvaluationCriteria;

public class CreateEvaluationCriteriaCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateEvaluationCriteriaCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateEvaluationCriteriaCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.CourseId);
        }

        var existingWeight = await context.EvaluationCriteria
            .Where(c => c.CourseId == request.CourseId && c.IsActive)
            .SumAsync(c => c.Weight, cancellationToken);

        if (existingWeight + request.Weight > 100)
        {
            return Result<int>.Failure($"Total criteria weight for this course would exceed 100% ({existingWeight}% already assigned).");
        }

        var criteria = EvaluationCriteriaEntity.Create(request.CourseId, request.CriteriaName, request.MaxScore, request.Weight, request.Description);

        context.EvaluationCriteria.Add(criteria);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(criteria.Id);
    }
}
