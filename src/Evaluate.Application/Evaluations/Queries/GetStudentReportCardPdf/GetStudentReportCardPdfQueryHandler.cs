using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Evaluations.Queries.GetStudentReportCard;
using MediatR;

namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCardPdf;

/// <summary>Composes the existing <see cref="GetStudentReportCardQuery"/> (rather than
/// duplicating the aggregation logic) and hands the result to the PDF generator. Refuses to
/// generate a PDF until the student has a finalized evaluation for every subject registered
/// to their class this term — a report card built from partial results would misrepresent
/// the student's progress to the parent reading it.</summary>
public class GetStudentReportCardPdfQueryHandler(IMediator mediator, IReportCardPdfGenerator pdfGenerator) : IRequestHandler<GetStudentReportCardPdfQuery, byte[]>
{
    public async Task<byte[]> Handle(GetStudentReportCardPdfQuery request, CancellationToken cancellationToken)
    {
        var reportCard = await mediator.Send(new GetStudentReportCardQuery(request.StudentId, request.TermId), cancellationToken);

        if (!reportCard.IsComplete)
        {
            throw new ReportCardNotReadyException(
                "This student hasn't been evaluated in every subject registered to their class yet — the report card isn't ready to download.");
        }

        return pdfGenerator.Generate(reportCard);
    }
}
