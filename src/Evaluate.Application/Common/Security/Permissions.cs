namespace Evaluate.Application.Common.Security;

/// <summary>
/// Permission strings stored as ASP.NET Core Identity role claims (Type = "permission"),
/// rather than a hand-rolled Permissions table — Identity already owns Users/Roles, this
/// just extends it with fine-grained claims. Seeded per role in <c>SchoolDataSeeder</c>.
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";

    public static class AcademicYears { public const string Create = "AcademicYears.Create"; public const string Edit = "AcademicYears.Edit"; public const string View = "AcademicYears.View"; }
    public static class Terms { public const string Create = "Terms.Create"; public const string Edit = "Terms.Edit"; public const string View = "Terms.View"; }
    public static class Classes { public const string Create = "Classes.Create"; public const string View = "Classes.View"; }
    public static class Students { public const string Create = "Students.Create"; public const string Edit = "Students.Edit"; public const string View = "Students.View"; }
    public static class Enrollments { public const string Create = "Enrollments.Create"; public const string View = "Enrollments.View"; }
    public static class Courses { public const string Create = "Courses.Create"; public const string Edit = "Courses.Edit"; public const string View = "Courses.View"; }
    public static class Topics { public const string Create = "Topics.Create"; public const string Edit = "Topics.Edit"; public const string View = "Topics.View"; }
    public static class TeacherCourses { public const string Create = "TeacherCourses.Create"; public const string View = "TeacherCourses.View"; }
    public static class EvaluationCriteria { public const string Create = "EvaluationCriteria.Create"; public const string View = "EvaluationCriteria.View"; }
    public static class Evaluations { public const string Create = "Evaluations.Create"; public const string View = "Evaluations.View"; }
    public static class EvaluationResults { public const string Create = "EvaluationResults.Create"; public const string View = "EvaluationResults.View"; }
    public static class Users { public const string Create = "Users.Create"; public const string View = "Users.View"; }
    public static class AuditLogs { public const string View = "AuditLogs.View"; }

    /// <summary>All permission strings across every module — used to grant Administrator
    /// everything without hand-maintaining a duplicate list.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        AcademicYears.Create, AcademicYears.Edit, AcademicYears.View,
        Terms.Create, Terms.Edit, Terms.View,
        Classes.Create, Classes.View,
        Students.Create, Students.Edit, Students.View,
        Enrollments.Create, Enrollments.View,
        Courses.Create, Courses.Edit, Courses.View,
        Topics.Create, Topics.Edit, Topics.View,
        TeacherCourses.Create, TeacherCourses.View,
        EvaluationCriteria.Create, EvaluationCriteria.View,
        Evaluations.Create, Evaluations.View,
        EvaluationResults.Create, EvaluationResults.View,
        Users.Create, Users.View,
        AuditLogs.View,
    ];

    /// <summary>What a Teacher can do day-to-day: run evaluations, view the academic
    /// structure, but not manage users or the academic calendar itself.</summary>
    public static IReadOnlyList<string> TeacherDefaults { get; } =
    [
        Classes.View,
        Students.Create, Students.Edit, Students.View,
        Enrollments.View,
        Courses.View,
        Topics.View,
        EvaluationCriteria.View,
        Evaluations.Create, Evaluations.View,
        EvaluationResults.Create, EvaluationResults.View,
    ];
}
