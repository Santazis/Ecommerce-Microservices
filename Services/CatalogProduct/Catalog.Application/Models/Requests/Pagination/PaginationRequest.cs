namespace Catalog.Application.Models.Requests.Pagination;

public class PaginationRequest
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private int _pageSize { get; set; }
    private int _page { get; set; }
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public PaginationRequest()
    {
        PageSize = DefaultPageSize;
        Page = 1;
    }
}