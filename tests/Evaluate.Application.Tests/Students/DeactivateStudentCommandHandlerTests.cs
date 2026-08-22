using Evaluate.Application.Students.Commands.DeactivateStudent;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Xunit;
using StudentEntity = Evaluate.Domain.Entities.People.Student;

namespace Evaluate.Application.Tests.Students;

public class DeactivateStudentCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksStudentInactive()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateStudentCommandHandler(RepositoryFactory.Students(context), context);
        var result = await handler.Handle(new DeactivateStudentCommand(student.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(context.Students.Single(s => s.Id == student.Id).IsActive);
    }
}
