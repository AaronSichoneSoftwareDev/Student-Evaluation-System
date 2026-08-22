using MediatR;

namespace Evaluate.Application.Evaluations.Queries.GetEvaluationsList;

public record GetEvaluationsListQuery(int? StudentId = null, int? CourseId = null, int? TermId = null, int? AcademicYearId = null) : IRequest<List<EvaluationDto>>;
