using MediatR;

namespace Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;

public class GetEnrollmentsListQueryHandler(IEnrollmentRepository enrollments) : IRequestHandler<GetEnrollmentsListQuery, List<EnrollmentDto>>
{
    public Task<List<EnrollmentDto>> Handle(GetEnrollmentsListQuery request, CancellationToken cancellationToken) =>
        enrollments.GetListAsync(request.StudentId, request.AcademicYearId, request.ClassId, cancellationToken);
}
