using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using MediatR;

namespace Evaluate.Application.Classes.Commands.CreateClass;

public class CreateClassCommandHandler(IClassRepository classes, IUnitOfWork unitOfWork) : IRequestHandler<CreateClassCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await classes.ExistsByNameAsync(request.ClassName, cancellationToken);
        if (alreadyExists)
        {
            return Result<int>.Failure($"A class named '{request.ClassName}' already exists.");
        }

        var schoolClass = SchoolClassEntity.Create(request.ClassName, request.GradeLevel, request.Description);

        classes.Add(schoolClass);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(schoolClass.Id);
    }
}
