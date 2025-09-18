namespace User.Domain.Common.Errors;

public static class UserErrors
{
    public static Error UserNotFound = new("User.NotFound","User not found");
}