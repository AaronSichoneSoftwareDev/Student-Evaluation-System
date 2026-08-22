using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Enrollments.Commands.EnrollStudent;

public class EnrollStudentCommandHandler(IApplicationDbContext context) : IRequestHandler<EnrollStudentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var studentExists = await context.Students.AnyAsync(s => s.Id == request.StudentId, cancellationToken);
        if (!studentExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.People.Student), request.StudentId);
        }

        var academicYearExists = await context.AcademicYears.AnyAsync(y => y.Id == request.AcademicYearId, cancellationToken);
        if (!academicYearExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.AcademicYearId);
        }

        var classExists = await context.Classes.AnyAsync(c => c.Id == request.ClassId, cancellationToken);
        if (!classExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.SchoolClass), request.ClassId);
        }

        var alreadyEnrolled = await context.StudentEnrollments.AnyAsync(
            e => e.StudentId == request.StudentId && e.AcademicYearId == request.AcademicYearId && e.Status == EnrollmentStatus.Active,
            cancellationToken);
        if (alreadyEnrolled)
        {
            return Result<int>.Failure("This student already has an active enrollment for that academic year.");
        }

        var enrollment = StudentEnrollmentEntity.Enroll(request.StudentId, request.AcademicYearId, request.ClassId, request.EnrollmentDate);

        context.StudentEnrollments.Add(enrollment);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(enrollment.Id);
    }
}
