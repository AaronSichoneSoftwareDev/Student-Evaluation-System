using Evaluate.Application.Terms.Commands.CreateTerm;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;

namespace Evaluate.Application.Tests.Terms;

public class CreateTermCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithDuplicateTermNameInSameYear_ReturnsFailure()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateTermCommandHandler(RepositoryFactory.Terms(context), RepositoryFactory.AcademicYears(context), context);

        var first = await handler.Handle(
            new CreateTermCommand(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1)),
            CancellationToken.None);
        Assert.True(first.Succeeded);

        var second = await handler.Handle(
            new CreateTermCommand(year.Id, "Term 1", 2, new DateOnly(2026, 5, 1), new DateOnly(2026, 8, 1)),
            CancellationToken.None);

        Assert.False(second.Succeeded);
    }
}
