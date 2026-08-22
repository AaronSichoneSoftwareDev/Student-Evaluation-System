namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCard;

public record TopicResultDto(string TopicName, decimal Score, string? Comment);

public record CourseReportDto(string CourseName, decimal? FinalPercentage, string? FinalGrade, string? OverallComments, List<TopicResultDto> Topics);

public record ReportCardDto(
    string StudentName,
    string StudentNumber,
    string ClassName,
    string AcademicYearName,
    string TermName,
    List<CourseReportDto> Courses,
    bool IsComplete);
