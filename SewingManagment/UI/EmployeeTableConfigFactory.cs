using Microsoft.AspNetCore.Html;
using SewingManagment.Models;

namespace SewingManagment.UI
{
    public static class EmployeeTableConfigFactory
    {
        public static TableConfig Create()
        {
            return new TableConfig
            {
                Columns = new List<string> { "Id", "Name", "Gender", "Position" },
                Headers = new Dictionary<string, string>
                {
                    { "Id", "工號" },
                    { "Name", "姓名" },
                    { "Gender", "性別" },
                    { "Position", "職務" }
                },
                // 可根據資料客製化 FormatFunc
                FormatFunc = (item, col) =>
                {
                    var e = (Employee)item;
                    return col switch
                    {
                        "Gender" => e.Gender == "F" ? "女" : "男",
                        "Position" => e.Position switch
                        {
                            "03" => "經理",
                            "02" => "組長",
                            _ => "一般人員"
                        },
                        _ => e.GetType().GetProperty(col)?.GetValue(e)?.ToString() ?? ""
                    };
                },
                RowClassFunc = e => e.Position == "03" ? "table-warning" : "",
                // Note: Tag helpers (asp-*) are not processed when emitting HTML from a string in code.
                // Generate normal href links so the Edit button actually navigates.
                // 需要手動改跳轉到指定頁
                ActionsFunc = e => new HtmlString($@"
                <a class='btn btn-warning px-4' href='/Employee/Edit/{e.Id}'>編輯</a>
                <a class='btn btn-danger px-4 btn-delete' data-id='{e.Id}'>刪除</a>")
            };
        }
    }
}
