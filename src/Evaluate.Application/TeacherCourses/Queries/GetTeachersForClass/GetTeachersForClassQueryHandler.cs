using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Identity;
using MediatR;

namespace Evaluate.Application.TeacherCourses.Queries.GetTeachersForClass;

/// <summary>Scopes the Evaluations page's teacher dropdown to only the teachers actually
/// assigned to the selected class this year — a single indexed lookup plus a batched user
/// resolution by id, rather than loading every user in the system and filtering client-side.</summary>
public class GetTeachersForClassQueryHandler(
    ITeacherCourseRepository teacherCourses,
    IAcademicYearRepository academicYears,
    IIdentityService identityService) : IRequestHandler<GetTeachersForClassQuery, List<UserSummary>>
{
    public async Task<List<UserSummary>> Handle(GetTeachersForClassQuery request, CancellationToken cancellationToken)
    {
        var currentYear = await academicYears.GetCurrentAsync(cancellationToken);
        if (currentYear is null)
        {
            return [];
        }

        var assignments = await teacherCourses.GetActiveForClassAsync(request.ClassId, currentYear.Id, cancellationToken);
        var teacherIds = assignments.Select(a => a.TeacherUserId).Distinct().ToList();
        if (teacherIds.Count == 0)
        {
            return [];
        }

        var teachers = await identityService.GetUsersByIdsAsync(teacherIds, cancellationToken);
        return teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToList();
    }
}
