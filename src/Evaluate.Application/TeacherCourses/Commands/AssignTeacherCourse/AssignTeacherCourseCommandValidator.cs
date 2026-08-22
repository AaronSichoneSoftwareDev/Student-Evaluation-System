using FluentValidation;

namespace Evaluate.Application.TeacherCourses.Commands.AssignTeacherCourse;

public class AssignTeacherCourseCommandValidator : AbstractValidator<AssignTeacherCourseCommand>
{
    public AssignTeacherCourseCommandValidator()
    {
        RuleFor(x => x.TeacherUserId).NotEmpty();
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
        RuleFor(x => x.ClassId).GreaterThan(0);
    }
}
