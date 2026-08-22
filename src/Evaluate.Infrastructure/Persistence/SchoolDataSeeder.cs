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
/// seeding the dashboard's own demo data untouched. Sized so every data table in the app has
/// roughly 20 rows — two full pages at the 10-per-page pagination the tables use.
/// </summary>
public static class SchoolDataSeeder
{
    private static readonly (string First, string Last)[] TeacherNames =
    [
        ("Daniel", "Osei"), ("Priya", "Anand"), ("Grace", "Mulenga"), ("Kennedy", "Banda"),
        ("Fatima", "Suleiman"), ("Joseph", "Mwansa"), ("Linda", "Chileshe"), ("Robert", "Sinkala"),
        ("Agnes", "Kabwe"), ("Michael", "Tembo"), ("Ruth", "Nyirenda"), ("Patrick", "Zulu"),
        ("Esther", "Chanda"), ("Felix", "Mumba"), ("Beatrice", "Lungu"), ("Simon", "Musonda"),
        ("Christine", "Phiri"), ("Emmanuel", "Kaunda"), ("Joyce", "Mwewa"),
    ];

    private static readonly (string First, string Last, DateOnly Dob, Gender Gender)[] StudentNames =
    [
        ("Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male),
        ("Mary", "Phiri", new DateOnly(2013, 7, 2), Gender.Female),
        ("Chanda", "Mwape", new DateOnly(2012, 11, 20), Gender.Female),
        ("Bwalya", "Zulu", new DateOnly(2013, 1, 9), Gender.Male),
        ("Natasha", "Tembo", new DateOnly(2012, 9, 30), Gender.Female),
        ("Emmanuel", "Kabwe", new DateOnly(2013, 5, 18), Gender.Male),
        ("Ruth", "Chileshe", new DateOnly(2012, 12, 3), Gender.Female),
        ("Joseph", "Sinkala", new DateOnly(2013, 2, 27), Gender.Male),
        ("Agnes", "Mulenga", new DateOnly(2013, 8, 11), Gender.Female),
        ("Kennedy", "Nyirenda", new DateOnly(2012, 10, 6), Gender.Male),
        ("Beatrice", "Musonda", new DateOnly(2013, 4, 23), Gender.Female),
        ("Simon", "Lungu", new DateOnly(2012, 6, 15), Gender.Male),
        ("Christine", "Mwewa", new DateOnly(2013, 9, 8), Gender.Female),
        ("Felix", "Kaunda", new DateOnly(2013, 1, 30), Gender.Male),
        ("Joyce", "Chanda", new DateOnly(2012, 11, 12), Gender.Female),
        ("Patrick", "Mumba", new DateOnly(2013, 3, 5), Gender.Male),
        ("Esther", "Zimba", new DateOnly(2012, 7, 21), Gender.Female),
        ("Michael", "Ngoma", new DateOnly(2013, 6, 9), Gender.Male),
        ("Linda", "Kalenga", new DateOnly(2012, 8, 26), Gender.Female),
        ("Robert", "Chisala", new DateOnly(2013, 5, 2), Gender.Male),
    ];

    private static readonly string[] StrongComments =
        ["Excellent grasp of the material.", "Consistently strong performance.", "Shows real mastery of this topic."];

    private static readonly string[] GoodComments =
        ["Solid understanding overall.", "Good progress this term.", "Comfortable with most concepts here."];

    private static readonly string[] AverageComments =
        ["Adequate grasp of core concepts.", "Making steady progress.", "Basic understanding, needs more practice."];

    private static readonly string[] WeakComments =
        ["Needs additional support here.", "Struggling with some core concepts.", "Requires focused intervention."];

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

        var teachers = new List<ApplicationUser>();
        foreach (var (first, last) in TeacherNames)
        {
            var email = $"{first.ToLowerInvariant()}.{last.ToLowerInvariant()}@evaluate.edu";
            var teacher = await userManager.FindByEmailAsync(email);
            if (teacher is not null)
            {
                teachers.Add(teacher);
            }
        }

        // Academic Years: 20 consecutive years, the most recent marked current.
        const int currentCalendarYear = 2026;
        var years = new List<AcademicYearEntity>();
        for (var offset = 19; offset >= 0; offset--)
        {
            var y = currentCalendarYear - offset;
            years.Add(AcademicYearEntity.Create(y.ToString(), new DateOnly(y, 1, 1), new DateOnly(y, 12, 31)));
        }
        var academicYear = years[^1];
        academicYear.MarkAsCurrent();
        appContext.AcademicYears.AddRange(years);
        await appContext.SaveChangesAsync(cancellationToken);

        // Terms: 3 per year for the 7 most recent years (~20 rows) — older years predate digitized term records.
        var termedYears = years.TakeLast(7).ToList();
        var allTerms = new List<TermEntity>();
        foreach (var year in termedYears)
        {
            var y = year.StartDate.Year;
            allTerms.Add(TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(y, 1, 12), new DateOnly(y, 4, 17)));
            allTerms.Add(TermEntity.Create(year.Id, "Term 2", 2, new DateOnly(y, 5, 4), new DateOnly(y, 8, 14)));
            allTerms.Add(TermEntity.Create(year.Id, "Term 3", 3, new DateOnly(y, 9, 7), new DateOnly(y, 12, 11)));
        }
        var term1 = allTerms.Single(t => t.AcademicYearId == academicYear.Id && t.TermNumber == 1);
        var term2 = allTerms.Single(t => t.AcademicYearId == academicYear.Id && t.TermNumber == 2);
        term2.MarkAsCurrent();
        appContext.Terms.AddRange(allTerms);

        // Classes: 10 grade levels x 2 sections = 20.
        var classes = new List<SchoolClassEntity>();
        for (var grade = 1; grade <= 10; grade++)
        {
            classes.Add(SchoolClassEntity.Create($"Grade {grade}A", $"Grade {grade}"));
            classes.Add(SchoolClassEntity.Create($"Grade {grade}B", $"Grade {grade}"));
        }
        appContext.Classes.AddRange(classes);
        var classA = classes.Single(c => c.ClassName == "Grade 7A");
        var classB = classes.Single(c => c.ClassName == "Grade 8A");

        // Courses: 20 subjects.
        var courseDefs = new (string Code, string Name)[]
        {
            ("MATH01", "Mathematics"), ("ENG01", "English"), ("SCI01", "Science"), ("HIST01", "History"),
            ("GEO01", "Geography"), ("ICT01", "Information & Communication Technology"), ("FRE01", "French"),
            ("BIO01", "Biology"), ("CHEM01", "Chemistry"), ("PHY01", "Physics"), ("LIT01", "Literature"),
            ("CIV01", "Civic Education"), ("RE01", "Religious Education"), ("AGR01", "Agricultural Science"),
            ("HEC01", "Home Economics"), ("DAT01", "Design & Technology"), ("ECO01", "Economics"),
            ("ART01", "Art"), ("MUS01", "Music"), ("PE01", "Physical Education"),
        };
        var courses = courseDefs.Select(c => CourseEntity.Create(c.Code, c.Name)).ToList();
        appContext.Courses.AddRange(courses);
        await appContext.SaveChangesAsync(cancellationToken);

        var mathematics = courses[0];
        var english = courses[1];

        // Topics: ~20, spread across the first 10 (content-bearing) courses.
        var topicDefs = new (int CourseIndex, string Name)[]
        {
            (0, "Algebra"), (0, "Fractions"), (0, "Geometry"),
            (1, "Grammar"), (1, "Comprehension"), (1, "Creative Writing"),
            (2, "Biology Basics"), (2, "Chemistry Basics"), (2, "Physics Basics"),
            (3, "Ancient Civilizations"), (3, "World Wars"),
            (4, "Map Reading"), (4, "Climate Zones"),
            (5, "Spreadsheets"), (5, "Programming Basics"),
            (6, "Basic Vocabulary"), (6, "Verb Conjugation"),
            (7, "Cell Biology"),
            (8, "Periodic Table"),
            (9, "Forces and Motion"),
        };
        var topicOrderByCourse = new Dictionary<int, int>();
        var topics = new List<TopicEntity>();
        foreach (var (courseIndex, name) in topicDefs)
        {
            topicOrderByCourse.TryGetValue(courseIndex, out var order);
            order++;
            topicOrderByCourse[courseIndex] = order;
            topics.Add(TopicEntity.Create(courses[courseIndex].Id, name, order));
        }
        appContext.Topics.AddRange(topics);

        var algebra = topics.Single(t => t.TopicName == "Algebra");
        var fractions = topics.Single(t => t.TopicName == "Fractions");
        var geometry = topics.Single(t => t.TopicName == "Geometry");
        var grammar = topics.Single(t => t.TopicName == "Grammar");
        var comprehension = topics.Single(t => t.TopicName == "Comprehension");
        var creativeWriting = topics.Single(t => t.TopicName == "Creative Writing");

        // Students: 20, all enrolled in Grade 7A for the current academic year.
        var students = StudentNames
            .Select((s, i) => StudentEntity.Create($"STU{i + 1:000}", s.First, s.Last, s.Dob, s.Gender))
            .ToList();
        appContext.Students.AddRange(students);
        await appContext.SaveChangesAsync(cancellationToken);

        foreach (var student in students)
        {
            appContext.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, academicYear.Id, classA.Id, new DateOnly(academicYear.StartDate.Year, 1, 12)));
        }

        // Teacher-course assignments: Grade 7A gets exactly Mathematics + English (so the
        // report-card-readiness demo has a real, reachable "fully evaluated" state); the
        // remaining ~18 rows spread other teachers across other classes/courses to reach ~20 total.
        var teacherCourseAssignments = new List<TeacherCourseEntity>();
        if (teachers.Count > 0)
        {
            teacherCourseAssignments.Add(TeacherCourseEntity.Assign(teachers[0].Id, mathematics.Id, academicYear.Id, classA.Id));
            teacherCourseAssignments.Add(TeacherCourseEntity.Assign(teachers[0].Id, english.Id, academicYear.Id, classA.Id));

            var otherClasses = classes.Where(c => c.Id != classA.Id).ToList();
            for (var i = 0; i < 18 && otherClasses.Count > 0; i++)
            {
                var teacher = teachers[i % teachers.Count];
                var course = courses[i % courses.Count];
                var schoolClass = otherClasses[i % otherClasses.Count];
                teacherCourseAssignments.Add(TeacherCourseEntity.Assign(teacher.Id, course.Id, academicYear.Id, schoolClass.Id));
            }
        }
        appContext.TeacherCourses.AddRange(teacherCourseAssignments);

        await appContext.SaveChangesAsync(cancellationToken);

        var mathTeacherId = teachers.Count > 0 ? teachers[0].Id : admin.Id;

        // Evaluations for the current term (Term 2): everyone gets Mathematics finalized except
        // the last 4 students (left pending), and the first 10 also get English finalized — giving
        // a realistic mix of "fully pending", "partially pending", and "report card ready" students.
        var rng = new Random(20260501);
        var mathTopics = new[] { algebra, fractions, geometry };
        var englishTopics = new[] { grammar, comprehension, creativeWriting };

        for (var i = 0; i < students.Count - 4; i++)
        {
            FinalizeEvaluation(appContext, gradingStrategy, students[i].Id, mathTeacherId, mathematics.Id, academicYear.Id, term2.Id, mathTopics, rng);
        }

        for (var i = 0; i < 10; i++)
        {
            FinalizeEvaluation(appContext, gradingStrategy, students[i].Id, mathTeacherId, english.Id, academicYear.Id, term2.Id, englishTopics, rng);
        }

        await appContext.SaveChangesAsync(cancellationToken);
    }

    private static void FinalizeEvaluation(
        IApplicationDbContext context,
        IGradingStrategy gradingStrategy,
        int studentId,
        string teacherUserId,
        int courseId,
        int academicYearId,
        int termId,
        TopicEntity[] courseTopics,
        Random rng)
    {
        var evaluation = EvaluationEntity.Create(studentId, teacherUserId, courseId, academicYearId, termId, new DateOnly(2026, 6, 1));

        var scores = new List<decimal>();
        foreach (var topic in courseTopics)
        {
            var (score, comment) = GenerateResult(rng);
            evaluation.RecordTopicResult(topic.Id, score, comment);
            scores.Add(score);
        }

        evaluation.Submit();
        var finalPercentage = gradingStrategy.ComputeFinalPercentage(scores);
        evaluation.Finalize(finalPercentage, gradingStrategy.ComputeGrade(finalPercentage));

        context.Evaluations.Add(evaluation);
    }

    private static (decimal Score, string Comment) GenerateResult(Random rng)
    {
        var score = rng.Next(40, 99);
        var comment = score switch
        {
            >= 85 => StrongComments[rng.Next(StrongComments.Length)],
            >= 70 => GoodComments[rng.Next(GoodComments.Length)],
            >= 55 => AverageComments[rng.Next(AverageComments.Length)],
            _ => WeakComments[rng.Next(WeakComments.Length)],
        };
        return (score, comment);
    }

    private static async Task<ApplicationUser> SeedRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await EnsureRoleAsync(roleManager, "Administrator", Permissions.All);
        await EnsureRoleAsync(roleManager, "Teacher", Permissions.TeacherDefaults);

        var admin = await EnsureUserAsync(userManager, "marie.whitfield@evaluate.edu", "Marie", "Whitfield", "Administrator");

        foreach (var (first, last) in TeacherNames)
        {
            var email = $"{first.ToLowerInvariant()}.{last.ToLowerInvariant()}@evaluate.edu";
            await EnsureUserAsync(userManager, email, first, last, "Teacher");
        }

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
