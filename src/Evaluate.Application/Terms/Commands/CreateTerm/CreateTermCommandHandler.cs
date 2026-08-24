using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;
using MediatR;

namespace Evaluate.Application.Terms.Commands.CreateTerm;

public class CreateTermCommandHandler(ITermRepository terms, IAcademicYearRepository academicYears, IUnitOfWork unitOfWork) : IRequestHandler<CreateTermCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
    {
        var academicYearExists = await academicYears.ExistsAsync(request.AcademicYearId, cancellationToken);
        if (!academicYearExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.AcademicYearId);
        }

        var duplicateNumber = await terms.ExistsAsync(request.AcademicYearId, request.TermNumber, cancellationToken: cancellationToken);
        if (duplicateNumber)
        {
            return Result<int>.Failure($"Term {request.TermNumber} already exists for this academic year.");
        }

        var duplicateName = await terms.ExistsByNameAsync(request.AcademicYearId, request.TermName, cancellationToken: cancellationToken);
        if (duplicateName)
        {
            return Result<int>.Failure($"A term named '{request.TermName}' already exists for this academic year.");
        }

        var term = TermEntity.Create(request.AcademicYearId, request.TermName, request.TermNumber, request.StartDate, request.EndDate);

        terms.Add(term);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(term.Id);
    }
}
