using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Terms.Commands.DeactivateTerm;

public class DeactivateTermCommandHandler(ITermRepository terms, IUnitOfWork unitOfWork) : IRequestHandler<DeactivateTermCommand, Result>
{
    public async Task<Result> Handle(DeactivateTermCommand request, CancellationToken cancellationToken)
    {
        var term = await terms.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Academic.Term), request.Id);

        if (term.IsCurrent)
        {
            return Result.Failure("This is the current term — mark a different term as current before deactivating it.");
        }

        term.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
