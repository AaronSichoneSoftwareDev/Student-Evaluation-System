using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommandHandler(IStudentRepository students, IUnitOfWork unitOfWork) : IRequestHandler<DeactivateStudentCommand, Result>
{
    public async Task<Result> Handle(DeactivateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.People.Student), request.Id);

        student.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
