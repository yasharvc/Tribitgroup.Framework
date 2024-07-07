using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class EntityJoin<T> where T : notnull
    {
        public JoinTypeEnum JoinType { get; set; } = JoinTypeEnum.InnerJoin;
        public IEntity<T> FromEntity { get; set; }
        public IEntity<T> ToEntity { get; set; }
        public bool IsDetailToDetailJoin { get; set; } = false;
        public IEnumerable<RelationColumn> Relation { get; set; } = new List<RelationColumn>();

        public override string ToString()
        {
            return $"{GetJoinAsString()} {(IsDetailToDetailJoin ? FromEntity.GetTableName() : ToEntity.GetTableName())} ON {GetColumnsConditions()}";
        }

        private string GetColumnsConditions()
        {
            return string.Join(" AND ", Relation.Select(m => GetJoinCondition(m)));
        }

        private string GetJoinCondition(RelationColumn m)
        {
            if (m.IsRelationalCondition)
                return $"{FromEntity.GetTableName()}.{m.FromColumn} {m.Operator.ToSign()} {ToEntity.GetTableName()}.{m.ToColumn}";
            return $"{FromEntity.GetTableName()}.{m.FromColumn} {m.Operator.ToSign()} {m.ToColumn}";
        }

        private string GetJoinAsString()
        {
            return JoinType switch
            {
                JoinTypeEnum.InnerJoin => "INNER JOIN",
                JoinTypeEnum.RightJoin => "RIGHT JOIN",
                JoinTypeEnum.LeftJoin => "LEFT JOIN",
                JoinTypeEnum.FullOuterJoin => "FULL OUTER JOIN",
                JoinTypeEnum.CrossJoin => "CROSS JOIN",
                _ => "INNER JOIN",
            };
        }
    }
}
