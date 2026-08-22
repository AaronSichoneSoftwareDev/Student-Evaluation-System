using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Enrollments;
using MediatR;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler(
    IStudentRepository students,
    IAcademicYearRepository academicYears,
    IEnrollmentRepository enrollments,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await students.StudentNumberExistsAsync(request.StudentNumber, cancellationToken);
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

        students.Add(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.ClassId.HasValue)
        {
            var currentYear = await academicYears.GetCurrentAsync(cancellationToken);
            if (currentYear is not null)
            {
                var enrollment = StudentEnrollmentEntity.Enroll(student.Id, currentYear.Id, request.ClassId.Value, DateOnly.FromDateTime(DateTime.UtcNow));
                enrollments.Add(enrollment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return Result<int>.Success(student.Id);
    }
}
