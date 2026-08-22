using MediatR;

namespace Evaluate.Application.Students.Queries.GetActiveStudentsList;

/// <summary>Students who are actively enrolled during the current academic term — i.e. an
/// active <c>Student</c> with an active <c>StudentEnrollment</c> in the current
/// <c>AcademicYear</c>. Returns an empty list if no academic year is currently marked as
/// current (nothing to be "active in the term" against).</summary>
public record GetActiveStudentsListQuery : IRequest<List<ActiveStudentDto>>;
