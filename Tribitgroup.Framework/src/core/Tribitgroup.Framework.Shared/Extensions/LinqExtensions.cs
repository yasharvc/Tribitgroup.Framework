using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Extensions
{

    public static class LinqExtensions
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> query, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = query.Count();
            return query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderByExpression)
        {
            if (string.IsNullOrEmpty(orderByExpression))
                return query;

            string propertyName, orderByMethod;
            string[] strs = orderByExpression.Split(' ');
            propertyName = strs[0];

            if (strs.Length == 1)
                orderByMethod = "OrderBy";
            else
                orderByMethod = strs[1].Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

            ParameterExpression pe = Expression.Parameter(query.ElementType);
            MemberExpression me = Expression.Property(pe, propertyName);

            MethodCallExpression orderByCall = Expression.Call(typeof(Queryable), orderByMethod, new Type[] { query.ElementType, me.Type }, query.Expression
                , Expression.Quote(Expression.Lambda(me, pe)));

            return query.Provider.CreateQuery(orderByCall) as IQueryable<T> ?? throw new Exception();
        }

        public static IQueryable<T> WhereEqual<T>(this IQueryable<T> query, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            return query.Where($"{propertyName} = {value}");
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, string whereClause)
        {
            if (!condition || string.IsNullOrEmpty(whereClause))
                return query;

            return query.Where(whereClause);
        }

        public static IQueryable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, string whereClause)
        {
            if (!condition || string.IsNullOrEmpty(whereClause))
                return query.AsQueryable();

            return query.AsQueryable().Where(whereClause);
        }


        public static IQueryable<T> Where<T>(this IQueryable<T> query, params Condition[] conditions)
        {
            foreach (var item in conditions)
            {
                query = query.Where(item);
            }
            return query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, IEnumerable<Condition> conditions) => query.Where(conditions.ToArray());

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<Sort>? sorts)
        {
            if (sorts == null)
                return query;

            var sortStr = "";

            foreach (var sort in sorts)
                sortStr += $"{(sortStr.Length > 0 ? "," : "")}{sort}";
            if(sortStr.Length > 0)
                return query.OrderBy(sortStr);
            return query;
        }


        public static void SetMemberValue<T>(this T entity, Expression<Func<T, object>> expression, object? value)
        {
            var propName = entity.GetMemberName(expression);
            entity.SetMemberValue(propName, value);
        }

        public static void SetMemberValue<T>(this T entity, string propName, object? value)
        {
            var prop = entity?.GetType().GetProperty(propName);
            prop?.SetValue(entity, value is null ? null : entity.ChangeTo(prop.PropertyType, value));
        }

        public static void AddValueToListMember<T>(this T entity, Expression<Func<T, object>> expression, params object[] values)
        {
            var propName = entity.GetMemberName(expression);
            AddValueToListMember(entity, propName, values);
        }
        public static void AddValueToListMember<T>(this T entity, string propName, params object[] values)
        {
            var prop = entity?.GetType().GetProperty(propName) ?? throw new Exception("Property is not accessable");
            if(prop.GetValue(entity) == null)
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(prop.PropertyType.GenericTypeArguments[0]) ?? throw new Exception();

                var instance = (IList)(Activator.CreateInstance(constructedListType) ?? throw new InvalidCastException());

                foreach (var item in values)
                    instance.Add(item);

                prop.SetValue(entity, instance);
            }
            else
            {
                var lst = (IList)(prop.GetValue(entity) ?? throw new InvalidCastException());
                foreach (var item in values)
                    lst.Add(item);
            }
        }

        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }
        public static string GetMemberName<T,I>(this T instance, Expression<Func<T, I>> expression)
        {
            return GetMemberName(expression.Body);
        }
        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            var memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }
            return memberNames;
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T>? query, ICollection<Sort>? sorts)
        {
            if (query == null)
            {
                return Enumerable.Empty<T>().AsQueryable();
            }

            if (sorts == null || !sorts.Any())
            {
                return query;
            }
            var res = sorts.First().GetSortText();
            var sortString = sorts.Skip(1).Aggregate(res, (current, sort) => current + " , " + sort.GetSortText());

            return query.OrderBy(sortString).AsQueryable();
        }

        private static string GetSortText(this Sort sort)
        {
            return sort.SortType == SortTypeEnum.ASC ? sort.Column : $"{sort.Column} descending";
        }

        public static async Task<System.Linq.Dynamic.Core.PagedResult<T>> PaginateAsync<T>(this IQueryable<T>? query, Pagination? pagination = null, int defaultMaxCount = 1000)
        {
            if (query is null)
            {
                query = Enumerable.Empty<T>().AsQueryable();
            }
            await Task.CompletedTask;
            return HandlePagination(query, pagination, defaultMaxCount);
        }

        private static System.Linq.Dynamic.Core.PagedResult<T> HandlePagination<T>(IQueryable<T> query, Pagination? pagination, int defaultMaxCount)
        {
            if (pagination is null)
            {
                pagination = GetDefaulPagination(defaultMaxCount);
            }
            else
            {
                if (pagination.Page < 1)
                    pagination.Page = 1;

                if (pagination.Count > defaultMaxCount || pagination.Count < 1)
                    pagination.Count = defaultMaxCount;
            }

            var pagedResult = query.PageResult(pagination.Page, pagination.Count);
            pagedResult.Queryable = pagedResult.Queryable.ToList().AsQueryable();
            return pagedResult;
        }

        private static Pagination GetDefaulPagination(int defaultMaxCount)
        {
            return new Pagination { Page = 1, Count = defaultMaxCount };
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, Condition condition)
        {
            var config = new ParsingConfig
            {
                IsCaseSensitive = false
            };
            if (condition.Operator == ConditionOperatorEnum.Contains)
            {
                return query.Where(config, $"{condition.PropertyName}.Contains(@0)", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.In)
            {
                return ApplyInClause(query, condition);
            }
            else if (condition.Operator == ConditionOperatorEnum.Between)
            {
                return query.Where($"{condition.PropertyName} >= @0 AndAlso {condition.PropertyName} <= @1", condition.Values.First(), condition.Values.Skip(1).Take(1).First());
            }
            else if (condition.Operator == ConditionOperatorEnum.Equal)
            {
                return query.Where($"{condition.PropertyName} == @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.BiggerThan)
            {
                return query.Where($"{condition.PropertyName} > @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.BiggerOrEqual)
            {
                return query.Where($"{condition.PropertyName} >= @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.SmallerOrEqual)
            {
                return query.Where($"{condition.PropertyName} <= @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.SmallerThan)
            {
                return query.Where($"{condition.PropertyName} < @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.NotEqual)
            {
                return query.Where($"{condition.PropertyName} != @0", condition.Values.First());
            }
            return query;
            static IQueryable<T> ApplyInClause(IQueryable<T> query, Condition condition)
            {
                var propType = typeof(T).GetProperty(condition.PropertyName)?.PropertyType;
                if (propType is null)
                    return query;

                if (propType == typeof(string))
                {
                    return query.Where($"{condition.PropertyName} in @0", condition.Values.ToArray());
                }
                if (propType == typeof(Guid))
                {
                    var typedValues = condition.Values.Select(s => new Guid(s)).ToArray();
                    return query.Where($"{condition.PropertyName} in @0", typedValues);
                }
                else
                {
                    Type t = Nullable.GetUnderlyingType(propType) ?? propType;

                    var values = condition.Values.Select(s => Convert.ChangeType(s, t)).ToArray();
                    Type gt = typeof(List<>).MakeGenericType(propType);
                    IList typedValues = (IList)Activator.CreateInstance(gt);

                    foreach (var item in values)
                    {
                        typedValues.Add(item);
                    }
                    return query.Where($"{condition.PropertyName} in @0", typedValues);
                }
            }
        }

        public static string GetMemberName<T>(this Expression<Func<T, object>> expression) => GetMemberName(expression.Body);
        private static string GetMemberName(Expression expression)
        {
            string expressionCannotBeNullMessage = "The expression cannot be null.";
            string invalidExpressionMessage = "Invalid expression.";
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }
            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }
            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }
            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }
            throw new ArgumentException(invalidExpressionMessage);
        }
        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}
