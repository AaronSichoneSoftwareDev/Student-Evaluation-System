using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Students.Queries.GetStudentsList;

public class GetStudentsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetStudentsListQuery, List<StudentDto>>
{
    public async Task<List<StudentDto>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
    {
        var students = await context.Students
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .ToListAsync(cancellationToken);

        return students
            .Select(s => new StudentDto(s.Id, s.StudentNumber, s.FullName, s.DateOfBirth, s.Gender, s.Email, s.IsActive))
            .ToList();
    }
}
