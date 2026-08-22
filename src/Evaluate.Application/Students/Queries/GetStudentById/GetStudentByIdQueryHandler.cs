using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Students.Queries.GetStudentById;

public class GetStudentByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetStudentByIdQuery, StudentDetailDto?>
{
    public async Task<StudentDetailDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);

        var enrollment = currentYear is null
            ? null
            : await context.StudentEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(
                    e => e.StudentId == student.Id && e.AcademicYearId == currentYear.Id && e.Status == EnrollmentStatus.Active,
                    cancellationToken);

        return new StudentDetailDto(
            student.Id,
            student.StudentNumber,
            student.FirstName,
            student.MiddleName,
            student.LastName,
            student.DateOfBirth,
            student.Gender,
            student.Email,
            student.PhoneNumber,
            student.Address,
            student.IsActive,
            enrollment?.ClassId,
            enrollment?.Class?.ClassName);
    }
}
