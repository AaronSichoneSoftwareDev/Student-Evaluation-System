using MediatR;

namespace Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;

public record GetEnrollmentsListQuery(int? StudentId = null, int? AcademicYearId = null, int? ClassId = null) : IRequest<List<EnrollmentDto>>;
