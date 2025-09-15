using User.Application.Models.Requests;
using User.Domain.Common;

namespace User.Application.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(CreateUserRequest request,CancellationToken cancellation);
    Task<Result> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellation);
}