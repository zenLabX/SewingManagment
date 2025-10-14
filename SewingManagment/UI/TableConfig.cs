using Microsoft.AspNetCore.Html;

namespace SewingManagment.UI
{
    public class TableConfig
    {
        public List<string> Columns { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public Func<object, string, string>? FormatFunc { get; set; }
        public Func<dynamic, string>? RowClassFunc { get; set; }
        public Func<dynamic, IHtmlContent>? ActionsFunc { get; set; }
    }
}
