using MediatR;

namespace SmartSchedule.Application.Services.Users.GetUsers
{
    public sealed record GetUsersQuery(
        string? Search,
        string? Role
    ) : IRequest<List<UserResponse>>;
}
