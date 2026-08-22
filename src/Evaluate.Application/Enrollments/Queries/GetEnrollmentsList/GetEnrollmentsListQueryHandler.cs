using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;

public class GetEnrollmentsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetEnrollmentsListQuery, List<EnrollmentDto>>
{
    public async Task<List<EnrollmentDto>> Handle(GetEnrollmentsListQuery request, CancellationToken cancellationToken)
    {
        var query = context.StudentEnrollments
            .Include(e => e.Student)
            .Include(e => e.Class)
            .AsQueryable();

        if (request.StudentId.HasValue)
        {
            query = query.Where(e => e.StudentId == request.StudentId);
        }

        if (request.AcademicYearId.HasValue)
        {
            query = query.Where(e => e.AcademicYearId == request.AcademicYearId);
        }

        if (request.ClassId.HasValue)
        {
            query = query.Where(e => e.ClassId == request.ClassId);
        }

        var enrollments = await query.OrderByDescending(e => e.EnrollmentDate).ToListAsync(cancellationToken);

        return enrollments
            .Select(e => new EnrollmentDto(e.Id, e.StudentId, e.Student!.FullName, e.AcademicYearId, e.ClassId, e.Class!.ClassName, e.EnrollmentDate, e.Status))
            .ToList();
    }
}
