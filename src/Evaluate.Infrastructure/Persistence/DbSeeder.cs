using Evaluate.Domain.Entities.Dashboard;
using Evaluate.Domain.Enums.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence;

public static class DbSeeder
{
    private static readonly string[] Subjects = ["Mathematics", "Science", "English", "History", "Computer Science"];

    public static async Task SeedAsync(EvaluateDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (await context.Students.AnyAsync(cancellationToken))
        {
            return;
        }

        var students = new List<Student>
        {
            new() { Name = "Ava Thompson", Initials = "AT", Email = "ava.thompson@evaluate.edu" },
            new() { Name = "Liam Carter", Initials = "LC", Email = "liam.carter@evaluate.edu" },
            new() { Name = "Noah Bennett", Initials = "NB", Email = "noah.bennett@evaluate.edu" },
            new() { Name = "Emma Ruiz", Initials = "ER", Email = "emma.ruiz@evaluate.edu" },
            new() { Name = "Olivia Chen", Initials = "OC", Email = "olivia.chen@evaluate.edu" },
            new() { Name = "Mason Patel", Initials = "MP", Email = "mason.patel@evaluate.edu" },
            new() { Name = "Sophia Nguyen", Initials = "SN", Email = "sophia.nguyen@evaluate.edu" },
            new() { Name = "Ethan Brooks", Initials = "EB", Email = "ethan.brooks@evaluate.edu" },
            new() { Name = "Isabella Kim", Initials = "IK", Email = "isabella.kim@evaluate.edu" },
            new() { Name = "Lucas Romero", Initials = "LR", Email = "lucas.romero@evaluate.edu" },
        };
        context.Students.AddRange(students);

        var instructors = new List<Instructor>
        {
            new()
            {
                Name = "Dr. Marie Whitfield",
                Title = "Head of Assessment",
                Initials = "MW",
                Quote = "The purpose of evaluation is not to rank students but to reveal how they think, so we can teach them better."
            },
            new() { Name = "Prof. Daniel Osei", Title = "Mathematics Faculty", Initials = "DO", Quote = string.Empty },
            new() { Name = "Ms. Priya Anand", Title = "Science Faculty", Initials = "PA", Quote = string.Empty },
        };
        context.Instructors.AddRange(instructors);

        await context.SaveChangesAsync(cancellationToken);

        var rng = new Random(42);
        var evaluations = new List<Evaluation>();
        var evalStatuses = new[] { EvaluationStatus.Passed, EvaluationStatus.Passed, EvaluationStatus.Passed, EvaluationStatus.Pending, EvaluationStatus.Failed };
        var years = new[] { 2021, 2022, 2023, 2024, 2025 };
        var evalId = 100000;
        foreach (var student in students)
        {
            foreach (var year in years)
            {
                var subject = Subjects[rng.Next(Subjects.Length)];
                var status = evalStatuses[rng.Next(evalStatuses.Length)];
                var score = status switch
                {
                    EvaluationStatus.Passed => 70 + rng.Next(0, 29),
                    EvaluationStatus.Pending => 50 + rng.Next(0, 20),
                    _ => 30 + rng.Next(0, 25),
                };
                evaluations.Add(new Evaluation
                {
                    ReferenceCode = $"EV-{evalId++}",
                    StudentId = student.Id,
                    Subject = subject,
                    Score = score,
                    Status = status,
                    Date = new DateOnly(year, rng.Next(1, 13), rng.Next(1, 28)),
                });
            }
        }
        context.Evaluations.AddRange(evaluations);

        var submissions = new List<Submission>();
        var subStatuses = new[] { SubmissionStatus.Graded, SubmissionStatus.Graded, SubmissionStatus.PendingReview, SubmissionStatus.Rejected };
        var subId = 200000;
        foreach (var student in students)
        {
            foreach (var year in years)
            {
                var subject = Subjects[rng.Next(Subjects.Length)];
                var status = subStatuses[rng.Next(subStatuses.Length)];
                var score = status == SubmissionStatus.Graded ? 65 + rng.Next(0, 34) : 40 + rng.Next(0, 30);
                submissions.Add(new Submission
                {
                    ReferenceCode = $"SB-{subId++}",
                    StudentId = student.Id,
                    Subject = subject,
                    Score = score,
                    Status = status,
                    Date = new DateOnly(year, rng.Next(1, 13), rng.Next(1, 28)),
                });
            }
        }
        context.Submissions.AddRange(submissions);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        context.ActivityFeedItems.AddRange(
            new ActivityFeedItem { Date = today.AddDays(-1), Title = "Graded midterm exam", Description = "Mathematics midterm exams graded for Grade 10 cohort." },
            new ActivityFeedItem { Date = today.AddDays(-2), Title = "Added feedback", Description = "Left detailed feedback on Science lab report submissions." },
            new ActivityFeedItem { Date = today.AddDays(-3), Title = "Joined Peer Review Committee", Description = "Onboarded as a reviewer for the end-of-term peer assessments." },
            new ActivityFeedItem { Date = today.AddDays(-5), Title = "Completed rubric", Description = "Finalized grading rubric for the Group Project evaluation." },
            new ActivityFeedItem { Date = today.AddDays(-7), Title = "Published progress report", Description = "Quarterly progress report shared with parents and guardians." }
        );

        context.InboxMessages.AddRange(
            new InboxMessage { SenderName = "Priya Anand", Initials = "PA", Preview = "Hey! The lab scores are ready for review.", Time = new TimeOnly(13, 40) },
            new InboxMessage { SenderName = "Daniel Osei", Initials = "DO", Preview = "I've finished grading! See you soon.", Time = new TimeOnly(12, 34) },
            new InboxMessage { SenderName = "Marie Whitfield", Initials = "MW", Preview = "This rubric update looks great.", Time = new TimeOnly(11, 17) },
            new InboxMessage { SenderName = "Grace Fuller", Initials = "GF", Preview = "Nice to meet you at the review meeting.", Time = new TimeOnly(10, 20) },
            new InboxMessage { SenderName = "Victor Adams", Initials = "VA", Preview = "Hey! There's an available slot tomorrow.", Time = new TimeOnly(9, 47) },
            new InboxMessage { SenderName = "Rachel Cho", Initials = "RC", Preview = "Hey! There's feedback pending on your desk.", Time = new TimeOnly(9, 2) }
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
