using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class Column
    {
        public string TableName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        protected dynamic? NewT { get; private set; } = null;

        public void SetDbContext(DbContext? dbContext)
        {
            if (NewT != null)
                TableName = NewT.GetTableName(dbContext);
        }

        public override string ToString() => $"{TableName}.{ColumnName}{(string.IsNullOrEmpty(Alias) ? "" : $" as {Alias}")}";

        public static Column From<T, ID>(Expression<Func<T, object>> propertySelector, string alias = "", DbContext? dbContext = null) where T : IEntity<ID>, new() where ID : notnull
        {
            var obj = new T();
            var res = new Column
            {
                TableName = obj.GetTableName(dbContext),
                Alias = alias,
                ColumnName = obj.SelectProperties(propertySelector).First().Name
            };
            res.NewT = obj;
            return res;
        }

        public static Column From<T>(Expression<Func<T, object>> propertySelector, string alias = "", DbContext? dbContext = null) where T : IEntity<Guid>, new() => From<T, Guid>(propertySelector, alias, dbContext);
    }
}
