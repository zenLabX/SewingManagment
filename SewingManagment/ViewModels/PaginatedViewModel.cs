namespace SewingManagment.ViewModels
{
    public class PaginatedViewModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public string? SearchTerm { get; set; }
        public string? SearchField { get; set; }
        public string? SortField { get; set; } = string.Empty;
        public string? SortDirection { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
