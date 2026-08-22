using MediatR;

namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCard;

/// <summary>Aggregates every finalized evaluation a student has for one term into a report
/// card. When <paramref name="TermId"/> is omitted, the current term is resolved the same
/// way <c>GetPendingEvaluationsListQuery</c> does.</summary>
public record GetStudentReportCardQuery(int StudentId, int? TermId = null) : IRequest<ReportCardDto>;
