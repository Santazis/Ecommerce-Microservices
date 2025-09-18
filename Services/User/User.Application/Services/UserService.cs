using Microsoft.EntityFrameworkCore;
using User.Application.Interfaces;
using User.Application.Models.Requests;
using User.Database;
using User.Domain.Common;
using User.Domain.Common.Errors;
using User.Domain.Models.Entities.Users;

namespace User.Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateUserAsync(CreateUserRequest request,CancellationToken cancellation)
    {
        var email = Email.Create(request.Email);
        var user = Domain.Models.Entities.Users.User.Create(request.UserId,email);
        await _context.Users.AddAsync(user,cancellation);
        await _context.SaveChangesAsync(cancellation);
        //_context.Profiles.Add(new Profile(user.Id,email))
    }

    public async Task<Result> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellation)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellation);
        if (user is null)
        {
            return Result.Failure(UserErrors.UserNotFound);
        }
        var address = Address.Create(request.Street,request.City,request.State,request.Country,request.ZipCode);
        var name = new Name(request.FirstName,request.LastName);
        user.CreateProfile(name,address);
        await _context.SaveChangesAsync(cancellation);
        return Result.Success;
    }
}