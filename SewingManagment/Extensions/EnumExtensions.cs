using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SewingManagment.Extensions
{
    public static class EnumExtensions
    {
        /** 下拉選單專用 */
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Text = e.GetType()
                                   .GetMember(e.ToString())
                                   .First()
                                   .GetCustomAttribute<DisplayAttribute>()?.Name
                                   ?? e.ToString(),
                           Value = e.ToString()
                       });
        }
    }
}
