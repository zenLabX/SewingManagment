using Microsoft.EntityFrameworkCore;
using SewingManagment.Models;
using SewingManagment.ViewModels;

namespace SewingManagment.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PaginatedViewModel<T>> ToPaginatedViewModel<T>(
            this IQueryable<T> source, QueryViewModel queryViewModel)
        {
            var totalCount = await source.CountAsync(); // 非同步計算總筆數
            var totalPages = (int)Math.Ceiling(totalCount / (double)queryViewModel.PageSize);

            var items = await source
            .Skip((queryViewModel.PageNumber - 1) * queryViewModel.PageSize)
            .Take(queryViewModel.PageSize)
            .ToListAsync(); // 非同步取得分頁資料

            return new PaginatedViewModel<T>
            {
                Items = items,
                SearchTerm = queryViewModel.SearchTerm,
                SearchField = queryViewModel.SearchField,
                PageNumber = queryViewModel.PageNumber,
                PageSize = queryViewModel.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }

}
