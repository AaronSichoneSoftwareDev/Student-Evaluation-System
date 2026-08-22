using Evaluate.Application.AcademicYears;
using Evaluate.Application.Classes;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using IStudentRepository = Evaluate.Application.Students.IStudentRepository;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Enrollments.Commands.EnrollStudent;

public class EnrollStudentCommandHandler(
    IEnrollmentRepository enrollments,
    IStudentRepository students,
    IAcademicYearRepository academicYears,
    IClassRepository classes,
    IUnitOfWork unitOfWork) : IRequestHandler<EnrollStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var studentExists = await students.ExistsAsync(request.StudentId, cancellationToken);
        if (!studentExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.People.Student), request.StudentId);
        }

        var academicYearExists = await academicYears.ExistsAsync(request.AcademicYearId, cancellationToken);
        if (!academicYearExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.AcademicYearId);
        }

        var classExists = await classes.ExistsAsync(request.ClassId, cancellationToken);
        if (!classExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.SchoolClass), request.ClassId);
        }

        var alreadyEnrolled = await enrollments.HasActiveEnrollmentAsync(request.StudentId, request.AcademicYearId, cancellationToken);
        if (alreadyEnrolled)
        {
            return Result<int>.Failure("This student already has an active enrollment for that academic year.");
        }

        var enrollment = StudentEnrollmentEntity.Enroll(request.StudentId, request.AcademicYearId, request.ClassId, request.EnrollmentDate);

        enrollments.Add(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(enrollment.Id);
    }
}
