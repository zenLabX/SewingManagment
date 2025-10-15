using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace SewingManagment.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySearchAndSort<T>(this IQueryable<T> query, string? searchTerm, string? fieldsCsv, string? sortField, string? sortDirection)
        {
            // --- 搜尋邏輯 ---
            if (!string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(fieldsCsv))
            {
                var fields = fieldsCsv.Split(',')
                                      .Select(f => f.Trim())
                                      .Where(f => !string.IsNullOrEmpty(f))
                                      .ToList();

                if (fields.Any())
                {
                    var stringProps = typeof(T).GetProperties()
                                               .Where(p => p.PropertyType == typeof(string))
                                               .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

                    var validFields = fields.Where(f => stringProps.ContainsKey(f)).ToList();

                    if (validFields.Any())
                    {
                        // 多欄位模糊搜尋
                        IQueryable<T> searchQuery = query;

                        foreach (var field in validFields)
                        {
                            var propName = field;
                            searchQuery = searchQuery.Where(x =>
                                EF.Property<string>(x, propName) != null &&
                                EF.Property<string>(x, propName).Contains(searchTerm));
                        }

                        query = searchQuery;
                    }
                }
            }

            // --- 排序邏輯 ---
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                var prop = typeof(T).GetProperty(sortField,
                            System.Reflection.BindingFlags.IgnoreCase |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.Instance);

                if (prop != null)
                {
                    bool descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

                    query = descending
                        ? query.OrderByDescending(x => EF.Property<object>(x, prop.Name))
                        : query.OrderBy(x => EF.Property<object>(x, prop.Name));
                }
            }

            return query;
        }

    }
}



// using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using Microsoft.EntityFrameworkCore;

// namespace SewingManagment.Extensions
// {
//     public static class QueryableExtensions
//     {
//         public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string searchTerm, string searchField)
//         {
//             if (string.IsNullOrWhiteSpace(searchTerm) || string.IsNullOrWhiteSpace(searchField))
//             {
//                 return query;
//             }

//             var parameter = Expression.Parameter(typeof(T), "x");
//             Expression body = null;

//             // 處理多個搜尋欄位或預設行為
//             if (searchField.Contains(",")) // 如果 searchField 是逗號分隔的多個欄位
//             {
//                 var fields = searchField.Split(',').Select(f => f.Trim()).ToList();
//                 foreach (var field in fields)
//                 {
//                     var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
//                     if (property != null && property.PropertyType == typeof(string))
//                     {
//                         var propertyAccess = Expression.Property(parameter, property);
//                         var searchTermExpression = Expression.Constant(searchTerm);
//                         var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
//                         var containsCall = Expression.Call(propertyAccess, containsMethod, searchTermExpression);

//                         if (body == null)
//                         {
//                             body = containsCall;
//                         } else {
//                             body = Expression.OrElse(body, containsCall);
//                         }
//                     }
//                 }
//             }
//             else // 單一搜尋欄位
//             {
//                 var property = typeof(T).GetProperty(searchField, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
//                 if (property != null && property.PropertyType == typeof(string))
//                 {
//                     var propertyAccess = Expression.Property(parameter, property);
//                     var searchTermExpression = Expression.Constant(searchTerm);
//                     var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
//                     body = Expression.Call(propertyAccess, containsMethod, searchTermExpression);
//                 }
//             }

//             // 如果沒有有效的搜尋條件，則返回原始查詢
//             if (body == null) 
//             {
//                 return query;
//             }

//             var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
//             return query.Where(lambda);
//         }

//         // 以欄位選擇器（型別安全）進行多欄位 OR 模糊查詢，供各頁面重用
//         public static IQueryable<T> ApplySearchAny<T>(this IQueryable<T> query, string? searchTerm, params Expression<Func<T, string?>>[] fields)
//         {
//             if (string.IsNullOrWhiteSpace(searchTerm) || fields == null || fields.Length == 0)
//             {
//                 return query;
//             }

//             var term = searchTerm.Trim();
//             Expression? body = null;
//             var parameter = Expression.Parameter(typeof(T), "x");

//             foreach (var field in fields)
//             {
//                 // 將欄位表達式中的參數替換成我們統一的 parameter
//                 var replaced = new ParameterReplacer(parameter).Visit(field.Body);

//                 // EF.Functions.Like(field, "%term%")，需處理可能的 null（COALESCE 成空字串）
//                 var coalesce = Expression.Coalesce(replaced!, Expression.Constant(string.Empty, typeof(string)));
//                 var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
//                     nameof(DbFunctionsExtensions.Like),
//                     new[] { typeof(DbFunctions), typeof(string), typeof(string) }
//                 );

//                 var likeCall = Expression.Call(
//                     likeMethod!,
//                     Expression.Property(null, typeof(EF), nameof(EF.Functions)),
//                     coalesce,
//                     Expression.Constant($"%{term}%")
//                 );

//                 body = body == null ? likeCall : Expression.OrElse(body, likeCall);
//             }

//             if (body == null) return query;
//             var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
//             return query.Where(lambda);
//         }

//         // 以欄位名稱字串（支援逗號）+ 白名單，混合模糊/等值條件，供跨頁面重用
//         public static IQueryable<T> ApplySearchByFields<T>(
//             this IQueryable<T> query,
//             string? searchTerm,
//             string? fieldsCsv,
//             ISet<string> likeFieldsWhitelist,
//             ISet<string>? exactFieldsWhitelist = null)
//             where T : class
//         {
//             if (string.IsNullOrWhiteSpace(searchTerm) || string.IsNullOrWhiteSpace(fieldsCsv))
//             {
//                 return query;
//             }

//             var term = searchTerm.Trim();
//             var fields = fieldsCsv.Split(',').Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToList();
//             if (fields.Count == 0) return query;

//             exactFieldsWhitelist ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

//             var parameter = Expression.Parameter(typeof(T), "x");
//             Expression? body = null;

//             foreach (var field in fields)
//             {
//                 var isExact = exactFieldsWhitelist.Contains(field);
//                 var isLike = likeFieldsWhitelist.Contains(field);
//                 if (!isExact && !isLike) continue; // 不在白名單中不處理

//                 // 取動態屬性值：EF.Property<string?>(x, field)
//                 var efPropertyGeneric = typeof(EF).GetMethods(BindingFlags.Public | BindingFlags.Static)
//                     .First(m => m.Name == nameof(EF.Property) && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
//                     .MakeGenericMethod(typeof(string));
//                 var propertyAccess = Expression.Call(
//                     null,
//                     efPropertyGeneric,
//                     parameter,
//                     Expression.Constant(field)
//                 );

//                 Expression predicate;
//                 if (isExact)
//                 {
//                     // 等值比對（e.g. 性別等代碼欄位）
//                     predicate = Expression.Equal(propertyAccess, Expression.Constant(term, typeof(string)));
//                 }
//                 else
//                 {
//                     // 模糊比對
//                     var coalesce = Expression.Coalesce(propertyAccess, Expression.Constant(string.Empty, typeof(string)));
//                     var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
//                         nameof(DbFunctionsExtensions.Like),
//                         new[] { typeof(DbFunctions), typeof(string), typeof(string) }
//                     );
//                     predicate = Expression.Call(
//                         likeMethod!,
//                         Expression.Property(null, typeof(EF), nameof(EF.Functions)),
//                         coalesce,
//                         Expression.Constant($"%{term}%")
//                     );
//                 }

//                 body = body == null ? predicate : Expression.OrElse(body, predicate);
//             }

//             if (body == null) return query;
//             var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
//             return query.Where(lambda);
//         }

//         private sealed class ParameterReplacer : ExpressionVisitor
//         {
//             private readonly ParameterExpression _parameter;
//             public ParameterReplacer(ParameterExpression parameter)
//             {
//                 _parameter = parameter;
//             }
//             protected override Expression VisitParameter(ParameterExpression node)
//             {
//                 return _parameter;
//             }
//         }
//     }
// }
