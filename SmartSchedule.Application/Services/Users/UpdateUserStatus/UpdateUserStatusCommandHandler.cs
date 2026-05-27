using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Users.UpdateUserStatus
{
    internal sealed class UpdateUserStatusCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateUserStatusCommand>
    {
        public async Task Handle(
            UpdateUserStatusCommand request,
            CancellationToken cancellationToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user is null)
                throw new NotFoundException("User not found.");

            if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
                throw new ValidationException("Invalid status.");

            user.Status = status;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
