using Identity.Api.Interfaces;
using Identity.Api.Services;
using ProfileGrpc;

namespace Identity.Api.Grpc.Clients;

public class GrpcUserServiceClient : IUserServiceClient
{
    private readonly ProfileGrpc.UserService.UserServiceClient _userServiceClient;

    public GrpcUserServiceClient(UserService.UserServiceClient userServiceClient)
    {
        _userServiceClient = userServiceClient;
    }

    public async Task<string> CreateProfileAsync(Guid userId, string email,CancellationToken cancellation)
    {

        var request = new CreateUserRequestGrpc() {Id = userId.ToString(),Email = email};
        var response = await _userServiceClient.CreateUserAsync(request,cancellationToken:cancellation);
        return response.Message;
    }
}