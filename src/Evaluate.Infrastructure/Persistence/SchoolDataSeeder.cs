using System.Security.Claims;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Security;
using Evaluate.Domain.Enums;
using Evaluate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Infrastructure.Persistence;

/// <summary>
/// Seeds the *real* school-management model (roles/users, academic structure, students,
/// courses, evaluations). Deliberately separate from <see cref="DbSeeder"/>, which keeps
/// seeding the dashboard's own demo data untouched.
/// </summary>
public static class SchoolDataSeeder
{
    public static async Task SeedAsync(
        EvaluateDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IGradingStrategy gradingStrategy,
        CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        var admin = await SeedRolesAndUsersAsync(userManager, roleManager);

        IApplicationDbContext appContext = context;

        if (await appContext.AcademicYears.AnyAsync(cancellationToken))
        {
            return;
        }

        var teacherEmails = new[] { "daniel.osei@evaluate.edu", "priya.anand@evaluate.edu" };
        var teachers = new List<ApplicationUser>();
        foreach (var email in teacherEmails)
        {
            var teacher = await userManager.FindByEmailAsync(email);
            if (teacher is not null)
            {
                teachers.Add(teacher);
            }
        }

        var academicYear = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        academicYear.MarkAsCurrent();
        appContext.AcademicYears.Add(academicYear);
        await appContext.SaveChangesAsync(cancellationToken);

        var term1 = TermEntity.Create(academicYear.Id, "Term 1", 1, new DateOnly(2026, 1, 12), new DateOnly(2026, 4, 17));
        var term2 = TermEntity.Create(academicYear.Id, "Term 2", 2, new DateOnly(2026, 5, 4), new DateOnly(2026, 8, 14));
        var term3 = TermEntity.Create(academicYear.Id, "Term 3", 3, new DateOnly(2026, 9, 7), new DateOnly(2026, 12, 11));
        term2.MarkAsCurrent();
        appContext.Terms.AddRange(term1, term2, term3);

        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var classB = SchoolClassEntity.Create("Grade 8A", "Grade 8");
        appContext.Classes.AddRange(classA, classB);

        var mathematics = CourseEntity.Create("MATH01", "Mathematics");
        var english = CourseEntity.Create("ENG01", "English");
        var science = CourseEntity.Create("SCI01", "Science");
        appContext.Courses.AddRange(mathematics, english, science);

        await appContext.SaveChangesAsync(cancellationToken);

        var algebra = TopicEntity.Create(mathematics.Id, "Algebra", 1);
        var fractions = TopicEntity.Create(mathematics.Id, "Fractions", 2);
        var geometry = TopicEntity.Create(mathematics.Id, "Geometry", 3);
        var grammar = TopicEntity.Create(english.Id, "Grammar", 1);
        var comprehension = TopicEntity.Create(english.Id, "Comprehension", 2);
        var biology = TopicEntity.Create(science.Id, "Biology", 1);
        appContext.Topics.AddRange(algebra, fractions, geometry, grammar, comprehension, biology);

        var students = new List<StudentEntity>
        {
            StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male),
            StudentEntity.Create("STU002", "Mary", "Phiri", new DateOnly(2013, 7, 2), Gender.Female),
            StudentEntity.Create("STU003", "Chanda", "Mwape", new DateOnly(2012, 11, 20), Gender.Female),
            StudentEntity.Create("STU004", "Bwalya", "Zulu", new DateOnly(2013, 1, 9), Gender.Male),
            StudentEntity.Create("STU005", "Natasha", "Tembo", new DateOnly(2012, 9, 30), Gender.Female),
        };
        appContext.Students.AddRange(students);

        await appContext.SaveChangesAsync(cancellationToken);

        foreach (var student in students)
        {
            var enrollment = StudentEnrollmentEntity.Enroll(student.Id, academicYear.Id, classA.Id, new DateOnly(2026, 1, 12));
            appContext.StudentEnrollments.Add(enrollment);
        }

        if (teachers.Count == 2)
        {
            appContext.TeacherCourses.Add(TeacherCourseEntity.Assign(teachers[0].Id, mathematics.Id, academicYear.Id, classA.Id));
            appContext.TeacherCourses.Add(TeacherCourseEntity.Assign(teachers[0].Id, english.Id, academicYear.Id, classA.Id));
            appContext.TeacherCourses.Add(TeacherCourseEntity.Assign(teachers[1].Id, science.Id, academicYear.Id, classA.Id));
        }

        await appContext.SaveChangesAsync(cancellationToken);

        var teacherId = teachers.Count > 0 ? teachers[0].Id : admin.Id;

        // (Algebra, Fractions, Geometry) scores + a short comment per topic, per student.
        var topicScoreSets = new (decimal Score, string Comment)[][]
        {
            [(85, "Solid grasp of linear equations."), (78, "Needs more practice with mixed numbers."), (90, "Excellent spatial reasoning.")],
            [(95, "Outstanding problem-solving skills."), (88, "Very confident with fraction operations."), (91, "Strong understanding of shapes and angles.")],
            [(60, "Struggles with variable manipulation."), (55, "Needs significant support with fractions."), (68, "Basic understanding, needs more practice.")],
            [(78, "Good progress this term."), (80, "Comfortable with most fraction problems."), (74, "Adequate grasp of core concepts.")],
            [(45, "Requires additional support and practice."), (50, "Beginning to understand basic concepts."), (40, "Needs focused intervention.")],
        };
        var mathTopics = new[] { algebra, fractions, geometry };

        for (var i = 0; i < students.Count; i++)
        {
            var evaluation = EvaluationEntity.Create(students[i].Id, teacherId, mathematics.Id, academicYear.Id, term1.Id, new DateOnly(2026, 3, 20));

            for (var t = 0; t < mathTopics.Length; t++)
            {
                var (score, comment) = topicScoreSets[i][t];
                evaluation.RecordTopicResult(mathTopics[t].Id, score, comment);
            }

            evaluation.Submit();

            var finalPercentage = gradingStrategy.ComputeFinalPercentage(topicScoreSets[i].Select(s => s.Score));
            var finalGrade = gradingStrategy.ComputeGrade(finalPercentage);
            evaluation.Finalize(finalPercentage, finalGrade);

            appContext.Evaluations.Add(evaluation);
        }

        await appContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<ApplicationUser> SeedRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await EnsureRoleAsync(roleManager, "Administrator", Permissions.All);
        await EnsureRoleAsync(roleManager, "Teacher", Permissions.TeacherDefaults);

        var admin = await EnsureUserAsync(userManager, "marie.whitfield@evaluate.edu", "Marie", "Whitfield", "Administrator");
        await EnsureUserAsync(userManager, "daniel.osei@evaluate.edu", "Daniel", "Osei", "Teacher");
        await EnsureUserAsync(userManager, "priya.anand@evaluate.edu", "Priya", "Anand", "Teacher");

        return admin;
    }

    private static async Task EnsureRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName, IReadOnlyList<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is not null)
        {
            return;
        }

        role = new ApplicationRole(roleName);
        await roleManager.CreateAsync(role);

        foreach (var permission in permissions)
        {
            await roleManager.AddClaimAsync(role, new Claim(Permissions.ClaimType, permission));
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string role)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
        };

        await userManager.CreateAsync(user, "P@ssw0rd123!");
        await userManager.AddToRoleAsync(user, role);

        return user;
    }
}
