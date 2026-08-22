namespace Evaluate.Application.Classes.Queries.GetClassesList;

public record ClassDto(int Id, string ClassName, string GradeLevel, string? Description, bool IsActive);
