using System.Linq.Expressions;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class RelationColumn
    {
        public string FromColumn { get; set; } = string.Empty;
        public string ToColumn { get; set; } = string.Empty;
        public bool IsRelationalCondition { get; set; } = true;
        public ConditionOperatorEnum Operator { get; set; } = ConditionOperatorEnum.Equal;

        public static RelationColumn FKRelation<FROM, TO, IDTYPE>(Expression<Func<FROM, object>> FKPropSelector)
            where FROM : IEntity<IDTYPE>, new()
            where TO : IEntity<IDTYPE>, new()
            where IDTYPE : notnull
        {
            var to = new TO();
            var name = new FROM().SelectProperties(FKPropSelector).First().Name;
            var res = new RelationColumn
            {
                FromColumn = name,
                ToColumn = nameof(to.Id),
                IsRelationalCondition = true,
                Operator = ConditionOperatorEnum.Equal
            };
            return res;
        }
        public static RelationColumn FKRelation<FROM>(Expression<Func<FROM, object>> FKPropSelector)
            where FROM : IEntity<Guid>, new()
        {
            var name = new FROM().SelectProperties(FKPropSelector).First().Name;
            var res = new RelationColumn
            {
                FromColumn = name,
                ToColumn = nameof(Entity.Id),
                IsRelationalCondition = true,
                Operator = ConditionOperatorEnum.Equal
            };
            return res;
        }
    }
}
