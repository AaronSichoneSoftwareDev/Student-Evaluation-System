using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Courses;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;
using MediatR;

namespace Evaluate.Application.EvaluationCriteria.Commands.CreateEvaluationCriteria;

public class CreateEvaluationCriteriaCommandHandler(IEvaluationCriteriaRepository criteria, ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<CreateEvaluationCriteriaCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateEvaluationCriteriaCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await courses.ExistsAsync(request.CourseId, cancellationToken);
        if (!courseExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.CourseId);
        }

        var existingWeight = await criteria.GetActiveWeightSumAsync(request.CourseId, cancellationToken);

        if (existingWeight + request.Weight > 100)
        {
            return Result<int>.Failure($"Total criteria weight for this course would exceed 100% ({existingWeight}% already assigned).");
        }

        var newCriteria = EvaluationCriteriaEntity.Create(request.CourseId, request.CriteriaName, request.MaxScore, request.Weight, request.Description);

        criteria.Add(newCriteria);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(newCriteria.Id);
    }
}
