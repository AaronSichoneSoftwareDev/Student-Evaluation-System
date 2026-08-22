using Evaluate.Application.Identity;
using MediatR;

namespace Evaluate.Application.TeacherCourses.Queries.GetTeachersForClass;

public record GetTeachersForClassQuery(int ClassId) : IRequest<List<UserSummary>>;
