namespace ClubeBeneficios.Benefits.Domain.Dtos.Requests.Filters;

public class PaginationFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}