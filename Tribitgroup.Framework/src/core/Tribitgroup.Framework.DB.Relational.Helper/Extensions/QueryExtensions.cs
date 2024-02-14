using System.Linq.Expressions;
using System.Reflection;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.DB.Relational.Helper.Extensions
{
    public static class QueryExtensions
    {
        public static IEnumerable<Column> GetAsColumns<T>(this Entity<T> s) where T : notnull
        {
            var res = new List<Column>();
            var cols = s.GetColumnNames();
            foreach (var col in cols)
            {
                res.Add(new Column
                {
                    TableName = s.GetTableName(),
                    ColumnName = col,
                });
            }
            return res;
        }

        public static IEnumerable<Column> GetAsColumns<T>(this Type type) where T : notnull
        {
            var s = Activator.CreateInstance(type) as Entity<T> ?? throw new InvalidCastException();
            return s.GetAsColumns();
        }

        public static IEnumerable<Column> GetAsColumns(this Type type)
        {
            var s = Activator.CreateInstance(type) as Entity ?? throw new InvalidCastException();
            return s.GetAsColumns();
        }

        public static IEnumerable<Column> GetAsColumns<T>()
        {
            var s = Activator.CreateInstance(typeof(T)) as Entity ?? throw new InvalidCastException();
            return s.GetAsColumns();
        }

        public static string ToSign(this ConditionOperatorEnum oper)
        {
            return oper switch
            {
                ConditionOperatorEnum.Equal => "=",
                ConditionOperatorEnum.NotEqual => "<>",
                ConditionOperatorEnum.SmallerOrEqual => "<=",
                ConditionOperatorEnum.BiggerOrEqual => ">=",
                ConditionOperatorEnum.BiggerThan => ">",
                ConditionOperatorEnum.SmallerThan => "<",
                ConditionOperatorEnum.In => "IN",
                ConditionOperatorEnum.Between => "BETWEEN",
                ConditionOperatorEnum.Contains => "LIKE",
                ConditionOperatorEnum.EndWith => "LIKE",
                ConditionOperatorEnum.StartWith => "LIKE",
                _ => throw new Exception(),
            };
        }

        public static Query Embedd(this Query host, Query query)
        {
            host.SetInnerSelect(query);
            return host;
        }

        public static Query InnerJoin<ENTITY, IDTYPE>(this Query query, Func<List<RelationColumn>> join)
            where IDTYPE : notnull
            where ENTITY : IEntity<IDTYPE>, new()
        {
            query.InnerJoin<ENTITY, IDTYPE>(join);
            return query;
        }

        public static Query InnerJoin<ENTITY>(this Query query, Func<List<RelationColumn>> join)
             where ENTITY : IEntity<Guid>, new() => InnerJoin<ENTITY, Guid>(query, join);

        public static Query InnerJoin<MASTER, DETAIL>(this Query query, Expression<Func<MASTER, object>> propertySelector)
            where MASTER : IEntity<Guid>, new()
            where DETAIL : IEntity<Guid>, new()
        {
            query.InnerJoin<MASTER, DETAIL, Guid>(() =>
            {
                return new List<RelationColumn>
                {
                    RelationColumn.FKRelation(propertySelector)
                };
            });
            return query;
        }

        public static Query LeftJoin<ENTITY, IDTYPE>(this Query query, Func<List<RelationColumn>> join)
        {
            query.LeftJoin<ENTITY, IDTYPE>(join);
            return query;
        }
        public static Query SelectColumns(this Query query, Func<IEnumerable<Column>> cols)
        {
            query.Select(cols);
            return query;
        }

        public static Query AddCondition(this Query query, Func<ConditionMaker> func)
        {
            query.Where(func);
            return query;
        }

        public static Query AddCondition<ENTITY, T>(this Query query, Expression<Func<ENTITY, object>> propertySelector, ConditionOperatorEnum operation, params T[] values)
            where ENTITY : IEntity<Guid>, new()
        {
            query.Where(() =>
            {
                return ConditionMaker.Create(propertySelector, operation, values);
            });
            return query;
        }

        public static IEnumerable<PropertyInfo> SelectProperties<T>(this T obj, params Expression<Func<T, dynamic>>[] propertyLambdas)
        {
            var res = new List<PropertyInfo>();
            foreach (var propertyLambda in propertyLambdas)
            {
                switch (propertyLambda.Body)
                {
                    case MemberExpression m:
                        res.Add(m.Member as PropertyInfo);
                        break;
                    case UnaryExpression u when u.Operand is MemberExpression m:
                        res.Add(m.Member as PropertyInfo);
                        break;
                    default:
                        break;
                }
            }
            return res;
        }
    }
}