using Evaluate.Application.Common.Interfaces;
using MediatR;

namespace Evaluate.Application.Identity.Queries.GetUsersList;

public class GetUsersListQueryHandler(IIdentityService identityService) : IRequestHandler<GetUsersListQuery, List<UserSummary>>
{
    public Task<List<UserSummary>> Handle(GetUsersListQuery request, CancellationToken cancellationToken) =>
        identityService.GetUsersAsync(cancellationToken);
}
