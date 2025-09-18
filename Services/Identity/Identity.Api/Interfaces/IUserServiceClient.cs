namespace Identity.Api.Interfaces;

public interface IUserServiceClient
{
    Task<string> CreateProfileAsync(Guid userId, string email,CancellationToken cancellation);
}