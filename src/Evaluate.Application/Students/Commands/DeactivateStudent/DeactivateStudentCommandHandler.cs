using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommandHandler(IApplicationDbContext context) : IRequestHandler<DeactivateStudentCommand, Result>
{
    public async Task<Result> Handle(DeactivateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.People.Student), request.Id);

        student.Deactivate();
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
