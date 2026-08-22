using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await context.Students.AnyAsync(s => s.StudentNumber == request.StudentNumber, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"A student with number '{request.StudentNumber}' already exists.");
        }

        var student = StudentEntity.Create(
            request.StudentNumber,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.MiddleName,
            request.Email,
            request.PhoneNumber,
            request.Address);

        context.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        if (request.ClassId.HasValue)
        {
            var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);
            if (currentYear is not null)
            {
                var enrollment = StudentEnrollmentEntity.Enroll(student.Id, currentYear.Id, request.ClassId.Value, DateOnly.FromDateTime(DateTime.UtcNow));
                context.StudentEnrollments.Add(enrollment);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        return Result<int>.Success(student.Id);
    }
}
