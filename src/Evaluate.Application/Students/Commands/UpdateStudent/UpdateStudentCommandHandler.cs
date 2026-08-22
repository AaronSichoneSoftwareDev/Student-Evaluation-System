using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Enrollments;
using MediatR;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler(
    IStudentRepository students,
    IAcademicYearRepository academicYears,
    IEnrollmentRepository enrollments,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateStudentCommand, Result>
{
    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(request.Id, cancellationToken)
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
            var currentYear = await academicYears.GetCurrentAsync(cancellationToken);
            if (currentYear is not null)
            {
                var existingEnrollment = await enrollments.GetActiveByStudentAndYearAsync(request.Id, currentYear.Id, cancellationToken);

                if (existingEnrollment is null)
                {
                    var enrollment = StudentEnrollmentEntity.Enroll(request.Id, currentYear.Id, request.ClassId.Value, DateOnly.FromDateTime(DateTime.UtcNow));
                    enrollments.Add(enrollment);
                }
                else if (existingEnrollment.ClassId != request.ClassId.Value)
                {
                    existingEnrollment.ReassignClass(request.ClassId.Value);
                }
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
