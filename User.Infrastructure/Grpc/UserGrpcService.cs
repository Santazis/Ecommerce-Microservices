using Grpc.Core;
using ProfileGrpc;
using User.Application.Interfaces;
using User.Application.Models.Requests;
using User.Database;

namespace User.Infrastructure.Grpc;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;
    public UserGrpcService(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    
    public override async Task<CreateUserResponseGrpc> CreateUser(CreateUserRequestGrpc request, ServerCallContext context)
    {
        var userId = Guid.Parse(request.Id);
         await _userService.CreateUserAsync(new CreateUserRequest(userId, request.Email),
            context.CancellationToken);
         return new CreateUserResponseGrpc(){Message = "Profile Created"};
    }
}