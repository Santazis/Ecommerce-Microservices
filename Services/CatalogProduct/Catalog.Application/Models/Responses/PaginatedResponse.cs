namespace Catalog.Application.Models.Responses;

public record PaginatedResponse<T>(IEnumerable<T> Items, int TotalCount) where T : class;