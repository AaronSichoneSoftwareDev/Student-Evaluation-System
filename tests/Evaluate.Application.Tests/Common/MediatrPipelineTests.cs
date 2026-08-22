using Evaluate.Application.Common.Behaviours;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Evaluations.Queries.GetStudentReportCard;
using Evaluate.Application.Evaluations.Queries.GetStudentReportCardPdf;
using Evaluate.Application.Students.Commands.CreateStudent;
using Evaluate.Application.Students.Commands.UpdateStudent;
using Evaluate.Application.Students.Commands.DeactivateStudent;
using Evaluate.Application.Evaluations.Commands.CreateEvaluation;
using Evaluate.Domain.Enums;
using Evaluate.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Tests.Common;

/// <summary>
/// Exercises a command through the *real* MediatR pipeline (all registered behaviours,
/// same as production's <c>AddApplication()</c>), instead of calling a handler directly.
/// This is what a browser click actually goes through — a handler-only test would have
/// missed the bug where <see cref="AuthorizationBehaviour{TRequest,TResponse}"/> denied
/// every permission-gated command because no login UI exists to populate a user id.
/// </summary>
public class MediatrPipelineTests
{
    private static IServiceProvider BuildServiceProvider(TestDbContext context)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<IApplicationDbContext>(context);
        services.AddSingleton<ICurrentUserService, FakeCurrentUserService>();
        services.AddSingleton<IIdentityService, FakeIdentityService>();
        services.AddSingleton<IGradingStrategy, AverageGradingStrategy>();
        services.AddSingleton<IReportCardPdfGenerator, FakeReportCardPdfGenerator>();

        services.AddValidatorsFromAssembly(typeof(CreateStudentCommand).Assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(CreateStudentCommand).Assembly);
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_PermissionGatedCommand_SucceedsWithNoLoggedInUser()
    {
        using var context = TestDbContext.Create();
        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        var command = new CreateStudentCommand("STU100", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);

        // Must not throw ForbiddenAccessException — this is exactly what "Add Student" does
        // when clicked in the running app, where no one is authenticated.
        var result = await mediator.Send(command);

        Assert.True(result.Succeeded);
        Assert.True(result.Value > 0);
    }

    [Fact]
    public async Task Send_UpdateStudentCommand_SucceedsWithNoLoggedInUser()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU101", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        var command = new UpdateStudentCommand(student.Id, "Jane", "Doe-Smith", new DateOnly(2013, 1, 1), Gender.Female);

        var result = await mediator.Send(command);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Send_DeactivateStudentCommand_SucceedsWithNoLoggedInUser()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU102", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new DeactivateStudentCommand(student.Id));

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Send_CreateEvaluationCommand_SucceedsWithNoLoggedInUser()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU103", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);
        var course = CourseEntity.Create("MATH01", "Mathematics");
        context.Students.Add(student);
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);
        var algebra = TopicEntity.Create(course.Id, "Algebra", 1);
        context.Topics.Add(algebra);
        await context.SaveChangesAsync(CancellationToken.None);

        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        var command = new CreateEvaluationCommand(
            student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 1),
            [new TopicScoreInput(algebra.Id, 80, "Good work")]);

        var result = await mediator.Send(command);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Send_ReportCardPdfQuery_ThrowsWhenNotAllRegisteredCoursesAreEvaluated()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var term = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        term.MarkAsCurrent();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var math = CourseEntity.Create("MATH01", "Mathematics");
        var english = CourseEntity.Create("ENG01", "English");
        context.Terms.Add(term);
        context.Classes.Add(schoolClass);
        context.Courses.AddRange(math, english);
        await context.SaveChangesAsync(CancellationToken.None);

        var student = StudentEntity.Create("STU104", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        // English is registered to the class but this student was never evaluated in it.
        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, schoolClass.Id),
            TeacherCourseEntity.Assign("teacher-2", english.Id, year.Id, schoolClass.Id));
        await context.SaveChangesAsync(CancellationToken.None);

        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ReportCardNotReadyException>(
            () => mediator.Send(new GetStudentReportCardPdfQuery(student.Id)));
    }

    [Fact]
    public async Task Send_ReportCardPdfQuery_SucceedsWhenAllRegisteredCoursesAreEvaluated()
    {
        using var context = TestDbContext.Create();
        var strategy = new AverageGradingStrategy();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var term = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        term.MarkAsCurrent();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var math = CourseEntity.Create("MATH01", "Mathematics");
        context.Terms.Add(term);
        context.Classes.Add(schoolClass);
        context.Courses.Add(math);
        await context.SaveChangesAsync(CancellationToken.None);

        var algebra = TopicEntity.Create(math.Id, "Algebra", 1);
        context.Topics.Add(algebra);
        var student = StudentEntity.Create("STU105", "Jane", "Doe", new DateOnly(2013, 1, 1), Gender.Female);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        context.TeacherCourses.Add(TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, schoolClass.Id));

        var mathEval = EvaluationEntity.Create(student.Id, "teacher-1", math.Id, year.Id, term.Id, new DateOnly(2026, 3, 1));
        mathEval.RecordTopicResult(algebra.Id, 80, null);
        mathEval.Submit();
        var pct = strategy.ComputeFinalPercentage([80]);
        mathEval.Finalize(pct, strategy.ComputeGrade(pct));
        context.Evaluations.Add(mathEval);
        await context.SaveChangesAsync(CancellationToken.None);

        var provider = BuildServiceProvider(context);
        var mediator = provider.GetRequiredService<IMediator>();

        var pdfBytes = await mediator.Send(new GetStudentReportCardPdfQuery(student.Id));

        Assert.NotEmpty(pdfBytes);
    }

    private class FakeReportCardPdfGenerator : IReportCardPdfGenerator
    {
        public byte[] Generate(ReportCardDto reportCard) => [1, 2, 3];
    }
}
