using Evaluate.Application.Terms.Commands.UpdateTerm;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Tests.Terms;

public class UpdateTermCommandHandlerTests
{
    private static async Task<(TestDbContext Context, AcademicYearEntity Year, TermEntity Term1, TermEntity Term2)> SeedAsync()
    {
        var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var term1 = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        var term2 = TermEntity.Create(year.Id, "Term 2", 2, new DateOnly(2026, 5, 1), new DateOnly(2026, 8, 1));
        context.Terms.AddRange(term1, term2);
        await context.SaveChangesAsync(CancellationToken.None);

        return (context, year, term1, term2);
    }

    [Fact]
    public async Task Handle_UpdatesNameNumberAndDates()
    {
        var (context, _, term1, _) = await SeedAsync();
        var handler = new UpdateTermCommandHandler(RepositoryFactory.Terms(context), context);

        var result = await handler.Handle(
            new UpdateTermCommand(term1.Id, "First Term", 1, new DateOnly(2026, 1, 15), new DateOnly(2026, 4, 15)),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = context.Terms.Single(t => t.Id == term1.Id);
        Assert.Equal("First Term", updated.TermName);
        Assert.Equal(new DateOnly(2026, 1, 15), updated.StartDate);
    }

    [Fact]
    public async Task Handle_WithNumberMatchingAnotherTermInSameYear_ReturnsFailure()
    {
        var (context, _, term1, term2) = await SeedAsync();
        var handler = new UpdateTermCommandHandler(RepositoryFactory.Terms(context), context);

        var result = await handler.Handle(
            new UpdateTermCommand(term2.Id, "Term 2", 1, term2.StartDate, term2.EndDate),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Handle_WithNameMatchingAnotherTermInSameYear_ReturnsFailure()
    {
        var (context, _, term1, term2) = await SeedAsync();
        var handler = new UpdateTermCommandHandler(RepositoryFactory.Terms(context), context);

        var result = await handler.Handle(
            new UpdateTermCommand(term2.Id, "Term 1", term2.TermNumber, term2.StartDate, term2.EndDate),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Handle_KeepingItsOwnNameAndNumber_Succeeds()
    {
        var (context, _, term1, _) = await SeedAsync();
        var handler = new UpdateTermCommandHandler(RepositoryFactory.Terms(context), context);

        var result = await handler.Handle(
            new UpdateTermCommand(term1.Id, "Term 1", 1, term1.StartDate, term1.EndDate),
            CancellationToken.None);

        Assert.True(result.Succeeded);
    }
}
