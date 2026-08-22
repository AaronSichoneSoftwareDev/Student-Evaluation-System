using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateStudentCommand, Result>
{
    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.People.Student), request.Id);

        student.Update(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            request.MiddleName,
            request.Email,
            request.PhoneNumber,
            request.Address);

        if (request.ClassId.HasValue)
        {
            var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);
            if (currentYear is not null)
            {
                var existingEnrollment = await context.StudentEnrollments.FirstOrDefaultAsync(
                    e => e.StudentId == request.Id && e.AcademicYearId == currentYear.Id && e.Status == EnrollmentStatus.Active,
                    cancellationToken);

                if (existingEnrollment is null)
                {
                    var enrollment = StudentEnrollmentEntity.Enroll(request.Id, currentYear.Id, request.ClassId.Value, DateOnly.FromDateTime(DateTime.UtcNow));
                    context.StudentEnrollments.Add(enrollment);
                }
                else if (existingEnrollment.ClassId != request.ClassId.Value)
                {
                    existingEnrollment.ReassignClass(request.ClassId.Value);
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
