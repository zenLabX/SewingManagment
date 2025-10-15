using System.ComponentModel.DataAnnotations;

namespace SewingManagment.ViewModels
{
    public class QueryViewModel
    {
        public string? SearchTerm { get; set; } = string.Empty;
        public string? SearchField { get; set; } = string.Empty;
        public string? SortField { get; set; } = "Id";
        public string? SortDirection { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
