using FluentValidation;

namespace Evaluate.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.CourseCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CourseName).NotEmpty().MaximumLength(150);
    }
}
