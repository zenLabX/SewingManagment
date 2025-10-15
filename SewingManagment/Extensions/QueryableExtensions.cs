using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Globalization;

namespace SewingManagment.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySearchAndSort<T>(
            this IQueryable<T> query,
            string? searchTerm,
            string? fieldsCsv,
            string? sortField,
            string? sortDirection)
        {
            // --- 搜尋邏輯 ---
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var allProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

                // 若有 fieldsCsv，僅在其白名單中的欄位搜尋；否則搜尋所有欄位
                if (!string.IsNullOrWhiteSpace(fieldsCsv))
                {
                    var fields = fieldsCsv
                        .Split(',')
                        .Select(f => f.Trim())
                        .Where(f => !string.IsNullOrEmpty(f))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    allProps = allProps.Where(p => fields.Contains(p.Name)).ToList();
                }

                if (allProps.Any())
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    Expression? orBody = null;

                    // 共用：EF.Functions.Like 方法資訊
                    var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                        nameof(DbFunctionsExtensions.Like),
                        new[] { typeof(DbFunctions), typeof(string), typeof(string) }
                    );

                    // 共用：EF.Property<T>(x, propName) 的非閉合泛型方法
                    var efPropertyOpenGeneric = typeof(EF)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .First(m => m.Name == nameof(EF.Property) && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);

                    // 小工具：將某個常數值與指定屬性建立等值條件，處理 Nullable
                    Expression BuildEqualsPredicate(PropertyInfo prop, object value)
                    {
                        var propType = prop.PropertyType;
                        var efPropertyGeneric = efPropertyOpenGeneric.MakeGenericMethod(propType);
                        var propertyAccess = Expression.Call(
                            null,
                            efPropertyGeneric,
                            parameter,
                            Expression.Constant(prop.Name)
                        );

                        var underlying = Nullable.GetUnderlyingType(propType);
                        Expression right;
                        if (underlying != null)
                        {
                            // 將非 Nullable 的常數轉為 Nullable<T>
                            right = Expression.Convert(Expression.Constant(value, underlying), propType);
                        }
                        else
                        {
                            right = Expression.Constant(value, propType);
                        }

                        return Expression.Equal(propertyAccess, right);
                    }

                    var term = searchTerm.Trim();
                    var lower = term.ToLowerInvariant();

                    foreach (var prop in allProps)
                    {
                        var propType = prop.PropertyType;
                        var underlying = Nullable.GetUnderlyingType(propType) ?? propType;

                        Expression? predicateForThisProp = null;

                        // 1) 字串欄位：LIKE 模糊比對
                        if (underlying == typeof(string))
                        {
                            var efPropertyGeneric = efPropertyOpenGeneric.MakeGenericMethod(typeof(string));
                            var propertyAccess = Expression.Call(
                                null,
                                efPropertyGeneric,
                                parameter,
                                Expression.Constant(prop.Name)
                            );

                            var coalesce = Expression.Coalesce(propertyAccess, Expression.Constant(string.Empty, typeof(string)));
                            predicateForThisProp = Expression.Call(
                                likeMethod!,
                                Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                                coalesce,
                                Expression.Constant($"%{term}%")
                            );
                        }
                        else
                        {
                            // 2) 布林欄位
                            if (underlying == typeof(bool))
                            {
                                bool parsedBool;
                                if (lower is "true" or "false" or "1" or "0" or "y" or "n" or "yes" or "no" or "是" or "否")
                                {
                                    parsedBool = lower is "true" or "1" or "y" or "yes" or "是";
                                    predicateForThisProp = BuildEqualsPredicate(prop, parsedBool);
                                }
                            }
                            // 3) Guid
                            else if (underlying == typeof(Guid))
                            {
                                if (Guid.TryParse(term, out var g))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, g);
                                }
                            }
                            // 4) 日期/時間
                            else if (underlying == typeof(DateTime))
                            {
                                if (DateTime.TryParse(term, out var dt))
                                {
                                    // 簡化：精確等值（如需以日期天為單位，可改成區間條件）
                                    predicateForThisProp = BuildEqualsPredicate(prop, dt);
                                }
                            }
                            else if (underlying.FullName == "System.DateOnly")
                            {
                                if (DateOnly.TryParse(term, out var d))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, d);
                                }
                            }
                            // 5) 數值型別（常見）
                            else if (underlying == typeof(int))
                            {
                                if (int.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(long))
                            {
                                if (long.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(short))
                            {
                                if (short.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(byte))
                            {
                                if (byte.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(decimal))
                            {
                                if (decimal.TryParse(term, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(double))
                            {
                                if (double.TryParse(term, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            else if (underlying == typeof(float))
                            {
                                if (float.TryParse(term, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, v);
                                }
                            }
                            // 6) Enum（名稱或數值）
                            else if (underlying.IsEnum)
                            {
                                object? enumValue = null;
                                try
                                {
                                    enumValue = Enum.Parse(underlying, term, true);
                                }
                                catch
                                {
                                    if (long.TryParse(term, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
                                    {
                                        enumValue = Enum.ToObject(underlying, raw);
                                    }
                                }

                                if (enumValue != null)
                                {
                                    predicateForThisProp = BuildEqualsPredicate(prop, enumValue);
                                }
                            }
                        }

                        if (predicateForThisProp != null)
                        {
                            orBody = orBody == null ? predicateForThisProp : Expression.OrElse(orBody, predicateForThisProp);
                        }
                    }

                    if (orBody != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(orBody, parameter);
                        query = query.Where(lambda);
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
