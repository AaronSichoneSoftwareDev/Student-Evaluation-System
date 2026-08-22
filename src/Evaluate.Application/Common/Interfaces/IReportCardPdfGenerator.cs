using Evaluate.Application.Evaluations.Queries.GetStudentReportCard;

namespace Evaluate.Application.Common.Interfaces;

/// <summary>Renders a <see cref="ReportCardDto"/> to a downloadable PDF. Implemented in
/// Infrastructure (QuestPDF is a technical/rendering concern), kept behind this interface
/// so Application stays library-agnostic.</summary>
public interface IReportCardPdfGenerator
{
    byte[] Generate(ReportCardDto reportCard);
}
