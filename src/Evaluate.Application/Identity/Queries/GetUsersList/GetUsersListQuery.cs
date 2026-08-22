using MediatR;

namespace Evaluate.Application.Identity.Queries.GetUsersList;

public record GetUsersListQuery : IRequest<List<UserSummary>>;
