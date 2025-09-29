using Identity.Api.Interfaces;
using ProfileGrpc;


namespace Identity.Api.Grpc.Clients;

public class GrpcUserServiceClient : IUserServiceClient
{
    private readonly ProfileGrpc.UserService.UserServiceClient _userServiceClient;
    private readonly ILogger<GrpcUserServiceClient> _logger;
    public GrpcUserServiceClient(UserService.UserServiceClient userServiceClient, ILogger<GrpcUserServiceClient> logger)
    {
        _userServiceClient = userServiceClient;
        _logger = logger;
    }

    public async Task<string> CreateProfileAsync(Guid userId, string email,CancellationToken cancellation)
    {
        _logger.LogInformation("Calling gRPC CreateProfile for UserId={UserId}, Email={Email}", userId, email);
        var request = new CreateUserRequestGrpc() {Id = userId.ToString(),Email = email};
        var response = await _userServiceClient.CreateUserAsync(request,cancellationToken:cancellation);
        _logger.LogInformation("gRPC CreateUser succeeded for UserId={UserId}, Email={Email}. Response={Message}",
            userId, email, response.Message);
        return response.Message;
    }
}