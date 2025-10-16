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
        
        // Advanced search bindings
        public string? GlobalConnector { get; set; } = "AND"; // AND / OR
        public List<ConditionDto> Conditions { get; set; } = new();
        public List<SortDto> Sorts { get; set; } = new();
    }

    public class ConditionDto
    {
        public string? Field { get; set; }
        public string? Operator { get; set; }
        public string? Value { get; set; }
    }

    public class SortDto
    {
        public string? Field { get; set; }
        public string? Direction { get; set; }
    }
}
