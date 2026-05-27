using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;


namespace SmartSchedule.Application.Services.Users.UpdateUserRole
{
    internal sealed class UpdateUserRoleCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateUserRoleCommand>
    {
        public async Task Handle(
            UpdateUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                throw new ValidationException("Invalid role.");

            user.Role = role;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
