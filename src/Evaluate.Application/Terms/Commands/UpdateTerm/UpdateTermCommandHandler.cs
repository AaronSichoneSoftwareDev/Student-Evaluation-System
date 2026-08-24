using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Terms.Commands.UpdateTerm;

public class UpdateTermCommandHandler(ITermRepository terms, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTermCommand, Result>
{
    public async Task<Result> Handle(UpdateTermCommand request, CancellationToken cancellationToken)
    {
        var term = await terms.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Academic.Term), request.Id);

        var duplicateNumber = await terms.ExistsAsync(term.AcademicYearId, request.TermNumber, request.Id, cancellationToken);
        if (duplicateNumber)
        {
            return Result.Failure($"Term {request.TermNumber} already exists for this academic year.");
        }

        var duplicateName = await terms.ExistsByNameAsync(term.AcademicYearId, request.TermName, request.Id, cancellationToken);
        if (duplicateName)
        {
            return Result.Failure($"A term named '{request.TermName}' already exists for this academic year.");
        }

        term.Update(request.TermName, request.TermNumber, request.StartDate, request.EndDate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
