using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;

namespace Evaluate.Application.Classes.Commands.CreateClass;

public class CreateClassCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateClassCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await context.Classes.AnyAsync(c => c.ClassName == request.ClassName, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"A class named '{request.ClassName}' already exists.");
        }

        var schoolClass = SchoolClassEntity.Create(request.ClassName, request.GradeLevel, request.Description);

        context.Classes.Add(schoolClass);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(schoolClass.Id);
    }
}
