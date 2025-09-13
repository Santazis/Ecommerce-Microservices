using System.Text.RegularExpressions;
using User.Domain.Models.Primitives;

namespace User.Domain.Models.Entities.Users;

public sealed class Email : ValueObject
{
    public string Value { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private Email(string value)
    {
        Value = value;
    }

    private Email()
    {
    }

    public static Email Create(string value)
    {
        if (!IsValidEmail(value))
        {
            throw new ArgumentException("Invalid email");
        }
        return new Email(value);
    }

    private static bool IsValidEmail(string value)
    {
        return Regex.IsMatch(value,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public static implicit operator string(Email email) => email.Value;
}