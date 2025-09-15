namespace User.Application.Models.Requests;

public record UpdateProfileRequest(string FirstName,string LastName,string Country,string City,string Street,string ZipCode,string? State);