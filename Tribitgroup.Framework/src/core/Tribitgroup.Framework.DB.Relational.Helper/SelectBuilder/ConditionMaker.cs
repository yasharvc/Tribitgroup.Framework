using System.Linq.Expressions;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class ConditionMaker
    {
        public string VariableName { get; set; } = string.Empty;
        public ConditionOperatorEnum Operator { get; set; }
        List<object> Values { get; set; } = new List<object>();

        Func<object, string> Converter { get; set; }

        bool IsStringLike { get; set; }

        private ConditionMaker()
        {
            Converter = (value) =>
            {
                return $"{value}";
            };
        }

        public static ConditionMaker Create<ENTITY, IDTYPE, T>(Expression<Func<ENTITY, object>> propSelector, ConditionOperatorEnum operation, params T[] values)
            where ENTITY : IEntity<IDTYPE>, new()
            where T : notnull
            where IDTYPE : notnull
        {
            var v = new ENTITY();
            var name = v.SelectProperties(propSelector).First().Name;
            return Create($"{v.GetTableName()}.{name}", operation, values);
        }

        public static ConditionMaker Create<ENTITY, T>(Expression<Func<ENTITY, object>> propSelector, ConditionOperatorEnum operation, params T[] values)
            where ENTITY : IEntity<Guid>, new()
            where T : notnull => Create<ENTITY, Guid, T>(propSelector, operation, values);

        public static ConditionMaker Create<T>(string variableName, ConditionOperatorEnum operation, params T[] values) where T : notnull
        {
            var t = typeof(T);
            var res = new ConditionMaker
            {
                Operator = operation,
                VariableName = variableName,
            };
            foreach (var item in values)
            {
                res.Values.Add(item);
            }
            res.IsStringLike = t == typeof(string) || t == typeof(DateTime) || t == typeof(DateOnly) || t == typeof(TimeOnly) || t == typeof(Guid);
            return res;
        }

        public static ConditionMaker Create<T>(string variableName, ConditionOperatorEnum operation, Func<T, string> valueConverter, params T[] values) where T : notnull
        {
            var res = Create(variableName, operation, values);
            res.Converter = (o) =>
            {
                return valueConverter((T)o);
            };
            return res;
        }

        public override string ToString()
        {
            if (Operator == ConditionOperatorEnum.Between)
                return $"{VariableName} {Operator.ToSign()} {GetValueString(Values.First())} AND {GetValueString(Values[1])}";
            else if (Operator == ConditionOperatorEnum.In)
                return $"{VariableName} {Operator.ToSign()} ({string.Join(",", Values.Select(m => GetValueString(m)))})";
            else if (Operator == ConditionOperatorEnum.Contains)
                return $"{VariableName} {Operator.ToSign()} {GetValueString(Values.First(), "%", "%")}";
            else if (Operator == ConditionOperatorEnum.EndWith)
                return $"{VariableName} {Operator.ToSign()} {GetValueString(Values.First(), "%")}";
            else if (Operator == ConditionOperatorEnum.StartWith)
                return $"{VariableName} {Operator.ToSign()} {GetValueString(Values.First(), "", "%")}";
            return $"{VariableName} {Operator.ToSign()} {GetValueString(Values.First())}";
        }

        private string GetValueString(object value, string before = "", string after = "") => IsStringLike ? $"'{before}{Converter(value)}{after}'" : $"{before}{Converter(value)}{after}";
    }
}
