using Evaluate.Application.AcademicYears.Commands.UpdateAcademicYear;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;

namespace Evaluate.Application.Tests.AcademicYears;

public class UpdateAcademicYearCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesNameAndDates()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateAcademicYearCommandHandler(RepositoryFactory.AcademicYears(context), context);
        var result = await handler.Handle(
            new UpdateAcademicYearCommand(year.Id, "2026-2027", new DateOnly(2026, 2, 1), new DateOnly(2027, 1, 31)),
            CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = context.AcademicYears.Single(y => y.Id == year.Id);
        Assert.Equal("2026-2027", updated.YearName);
        Assert.Equal(new DateOnly(2026, 2, 1), updated.StartDate);
    }

    [Fact]
    public async Task Handle_WithNameMatchingAnotherYear_ReturnsFailure()
    {
        using var context = TestDbContext.Create();
        var yearA = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        var yearB = AcademicYearEntity.Create("2027", new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));
        context.AcademicYears.AddRange(yearA, yearB);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateAcademicYearCommandHandler(RepositoryFactory.AcademicYears(context), context);
        var result = await handler.Handle(
            new UpdateAcademicYearCommand(yearB.Id, "2026", new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31)),
            CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Handle_KeepingItsOwnName_Succeeds()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateAcademicYearCommandHandler(RepositoryFactory.AcademicYears(context), context);
        var result = await handler.Handle(
            new UpdateAcademicYearCommand(year.Id, "2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31)),
            CancellationToken.None);

        Assert.True(result.Succeeded);
    }
}
