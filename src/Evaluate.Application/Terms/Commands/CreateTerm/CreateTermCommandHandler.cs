using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Terms.Commands.CreateTerm;

public class CreateTermCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTermCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTermCommand request, CancellationToken cancellationToken)
    {
        var academicYearExists = await context.AcademicYears.AnyAsync(y => y.Id == request.AcademicYearId, cancellationToken);
        if (!academicYearExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Academic.AcademicYear), request.AcademicYearId);
        }

        var duplicateNumber = await context.Terms.AnyAsync(
            t => t.AcademicYearId == request.AcademicYearId && t.TermNumber == request.TermNumber,
            cancellationToken);
        if (duplicateNumber)
        {
            return Result<int>.Failure($"Term {request.TermNumber} already exists for this academic year.");
        }

        var term = TermEntity.Create(request.AcademicYearId, request.TermName, request.TermNumber, request.StartDate, request.EndDate);

        context.Terms.Add(term);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(term.Id);
    }
}
