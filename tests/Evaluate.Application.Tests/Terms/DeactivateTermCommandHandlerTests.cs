using Evaluate.Application.Terms.Commands.DeactivateTerm;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Tests.Terms;

public class DeactivateTermCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksTermInactive()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);
        var term = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        context.Terms.Add(term);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateTermCommandHandler(RepositoryFactory.Terms(context), context);
        var result = await handler.Handle(new DeactivateTermCommand(term.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(context.Terms.Single(t => t.Id == term.Id).IsActive);
    }

    [Fact]
    public async Task Handle_WhenTermIsCurrent_ReturnsFailure()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);
        var term = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        term.MarkAsCurrent();
        context.Terms.Add(term);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateTermCommandHandler(RepositoryFactory.Terms(context), context);
        var result = await handler.Handle(new DeactivateTermCommand(term.Id), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.True(context.Terms.Single(t => t.Id == term.Id).IsActive);
    }
}
