using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProfileGrpc;
using User.Application.Interfaces;
using User.Application.Models.Requests;
using User.Database;

namespace User.Infrastructure.Grpc;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;
    public UserGrpcService(ApplicationDbContext context, IUserService userService, ILogger<UserGrpcService> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }

    public override async Task<CreateUserResponseGrpc> CreateUser(CreateUserRequestGrpc request,
        ServerCallContext context)
    {
        _logger.LogInformation("Processing profile create for user {userId},peer {peer},method {method}",request.Id,context.Peer,context.Method);
        var userId = Guid.Parse(request.Id);
        await _userService.CreateUserAsync(new CreateUserRequest(userId, request.Email),
            context.CancellationToken);
        _logger.LogInformation("Profile created successfully. UserId={UserId}, Email={Email}", userId, request.Email);

        return new CreateUserResponseGrpc() { Message = $"User {userId} created successfully" };
    }
}