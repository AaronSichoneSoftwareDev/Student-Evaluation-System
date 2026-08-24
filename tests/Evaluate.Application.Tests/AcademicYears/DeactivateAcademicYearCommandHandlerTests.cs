using Evaluate.Application.AcademicYears.Commands.DeactivateAcademicYear;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;

namespace Evaluate.Application.Tests.AcademicYears;

public class DeactivateAcademicYearCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksYearInactive()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateAcademicYearCommandHandler(RepositoryFactory.AcademicYears(context), context);
        var result = await handler.Handle(new DeactivateAcademicYearCommand(year.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(context.AcademicYears.Single(y => y.Id == year.Id).IsActive);
    }

    [Fact]
    public async Task Handle_WhenYearIsCurrent_ReturnsFailure()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateAcademicYearCommandHandler(RepositoryFactory.AcademicYears(context), context);
        var result = await handler.Handle(new DeactivateAcademicYearCommand(year.Id), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.True(context.AcademicYears.Single(y => y.Id == year.Id).IsActive);
    }
}
