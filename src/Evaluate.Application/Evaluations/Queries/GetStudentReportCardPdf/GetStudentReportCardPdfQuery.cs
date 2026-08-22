using MediatR;

namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCardPdf;

public record GetStudentReportCardPdfQuery(int StudentId, int? TermId = null) : IRequest<byte[]>;
