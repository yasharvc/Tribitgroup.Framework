using Microsoft.EntityFrameworkCore;
using System.Data;
using Tribitgroup.Framework.Shared.Types;
using Dapper;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class Query
    {
        dynamic FromTable { get; set; }
        List<dynamic> Joins { get; set; } = new List<dynamic>();
        List<Column> SelectedColumns { get; set; } = new List<Column>();
        ICollection<Func<ConditionMaker>> Conditions { get; set; } = new List<Func<ConditionMaker>>();
        Query FromSelect { get; set; }
        string Alias { get; set; }
        DbContext? DbContext { get; set; } = null;

        private Query()
        {

        }

        public static Query From<ENTITY, IDTYPE>(DbContext? dbContext = null) where IDTYPE : notnull where ENTITY : IEntity<IDTYPE>, new()
        {
            var e = new ENTITY();
            var res = new Query
            {
                FromTable = e,
                Alias = e.GetTableName(dbContext)
            };
            res.DbContext = dbContext;
            return res;
        }

        public static Query From<T>(DbContext? dbContext = null) where T : IEntity<Guid>, new() => From<T, Guid>(dbContext);

        public static Query From(Query from)
        {
            var res = new Query
            {
                FromSelect = from,
            };
            return res;
        }

        public void SetInnerSelect(Query innerSelect) => FromSelect = innerSelect;

        public void InnerJoin<ENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where ENTITY : IEntity<IDTYPE>, new()
        {
            var toEntity = new ENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = GetFromTable<IDTYPE>(),
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.InnerJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }
        public void InnerJoin<FROMENTITY, TOENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where FROMENTITY : IEntity<IDTYPE>, new()
            where TOENTITY : IEntity<IDTYPE>, new()
        {
            var fromEntity = new FROMENTITY();
            var toEntity = new TOENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = fromEntity,
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.InnerJoin,
                Relation = join(),
                IsDetailToDetailJoin = typeof(FROMENTITY) != FromTable.GetType(),
            };
            Joins.Add(entityJoin);
        }

        public void LeftJoin<ENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where ENTITY : IEntity<IDTYPE>, new()
        {
            var toEntity = new ENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = GetFromTable<IDTYPE>(),
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.LeftJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }

        public void LeftJoin<FROMENTITY, TOENTITY, IDTYPE>(Func<List<RelationColumn>> join) where IDTYPE : notnull
            where FROMENTITY : Entity<IDTYPE>, new()
            where TOENTITY : Entity<IDTYPE>, new()
        {
            var fromEntity = new FROMENTITY();
            var toEntity = new TOENTITY();

            var entityJoin = new EntityJoin<IDTYPE>
            {
                FromEntity = fromEntity,
                ToEntity = toEntity,
                JoinType = JoinTypeEnum.LeftJoin,
                Relation = join()
            };
            Joins.Add(entityJoin);
        }

        public void Select(Func<IEnumerable<Column>> cols)
        { 
            var res = cols();
            foreach (var col in res)
            {
                col.SetDbContext(DbContext);
            }
            SelectedColumns.AddRange(res);
        }
        public void Where(Func<ConditionMaker> func) => Conditions.Add(func);

        public override string ToString()
        {
            var selectCols = string.Join(", ", SelectedColumns.Select(m => m.ToString()));
            var joins = "";
            if (Joins.Any())
            {
                joins = string.Join(" ", Joins.Select(m => m.ToString()));
            }
            else
            {
                joins = string.Join(", ", SelectedColumns.Select(m => m.TableName).Distinct());
            }
            var where = GetWhereClause();
            where = where.Any() ? $" WHERE {where}" : "";
            return $"SELECT {(string.IsNullOrEmpty(selectCols) ? "*" : selectCols)} FROM {GetFromTablePart()} {joins} {where}";
        }

        public async Task<IEnumerable<TDTO>> RunAsync<TDTO>(
            IDbConnection connection,
            Column idColumn,
            QueryMapper<TDTO> mapper)
            where TDTO : Entity, new()
        {
            var rawData = await connection.QueryAsync<object>(ToString());
            var res = new Dictionary<Guid,TDTO>();

            foreach (var item in rawData.ToList())
            {
                var valuesDict = (IDictionary<string, object>)item;
                var id = valuesDict[string.IsNullOrEmpty(idColumn.Alias) ? idColumn.ColumnName : idColumn.Alias].To<Guid>();
                var tempDTO = res.ContainsKey(id) ? res[id] : new TDTO { Id = id };
                var listDict = new Dictionary<string, object>();

                foreach (var key in valuesDict.Keys)
                {
                    var selectedMapper = mapper.GetMapperFor(key);
                    if (selectedMapper == null)
                        continue;

                    var value = valuesDict[key];

                    if (selectedMapper.IsListType)
                    {
                        try
                        {
                            var propNames = selectedMapper.PropertyName.Split('.');
                            object obj = tempDTO;

                            //Create List if there is no array
                            var tempProp = obj.GetType().GetProperty(propNames.First()) ?? throw new Exception();
                            var temp = tempProp?.GetValue(obj) ?? Activator.CreateInstance(tempProp.PropertyType);
                            var genericType = obj.GetType().GetGenericTypeFromList(propNames.First());
                            var genericItem = listDict.ContainsKey(propNames.First())
                                ? listDict[propNames.First()]
                                : Activator.CreateInstance(genericType);


                            tempProp.SetValue(obj, temp);

                            genericItem.SetMemberValue(propNames.Last(), value);

                            //foreach (var prop in propNames.Skip(1).Take(propNames.Length - 1))
                            //{
                            //    var tempProp = obj.GetType().GetProperty(prop) ?? throw new Exception();
                            //    var temp = tempProp?.GetValue(obj) ?? Activator.CreateInstance(tempProp.PropertyType);

                            //    tempProp.SetValue(obj, temp);

                            //    obj = obj.GetType().GetProperty(prop).GetValue(obj);
                            //}
                            //obj.SetMemberValue(propNames.Last(), value);

                            //obj.AddValueToListMember(propNames.First(), genericItem);
                            //How to know the current 
                            if(!listDict.ContainsKey(propNames.First()))
                                listDict[propNames.First()] = genericItem;
                        }
                        catch { }
                    }
                    else if(selectedMapper.IsBasicType)
                    {
                        try
                        {
                            tempDTO.SetMemberValue(selectedMapper.PropertyName, value);
                        }
                        catch { }
                    }
                    else if(selectedMapper.IsObjectType)
                    {
                        try
                        {
                            var propNames = selectedMapper.PropertyName.Split('.');
                            object obj = tempDTO;

                            foreach (var prop in propNames.Take(propNames.Length-1))
                            {
                                var tempProp = obj.GetType().GetProperty(prop) ?? throw new Exception();
                                var temp = tempProp?.GetValue(obj) ?? Activator.CreateInstance(tempProp.PropertyType);

                                tempProp.SetValue(obj, temp);

                                obj = obj.GetType().GetProperty(prop).GetValue(obj);
                            }
                            obj.SetMemberValue(propNames.Last(), value);
                        }
                        catch { }
                    }
                }

                foreach (var listData in listDict)
                {
                    tempDTO.AddValueToListMember(listData.Key, listData.Value);
                }

                res[id] = tempDTO;
            }

            return res.Values;
        }

        /*
         var myDict = (IDictionary<string,object>)item;
                var tempDTO = new TDTO();

                foreach (var key in myDict.Keys)
                {
                    var selectedMapper = mapper.GetMapperFor(key);
                    if (selectedMapper == null)
                        continue;

                    if (!selectedMapper.IsList)
                    {
                        try
                        {
                            tempDTO.SetMemberValue(selectedMapper.PropertyName, myDict[key]);
                        }
                        catch { }
                    }
                    else
                    {
                        var propNames = selectedMapper.PropertyName.Split('.');
                        //Get other props and then save
                        var newChild = Activator.CreateInstance(typeof(TDTO).GetGenericTypeFromList(propNames.First()) ?? throw new Exception());
                        newChild.SetMemberValue(propNames.Last(), myDict[key]);
                        tempDTO.AddValueToListMember(propNames.First(), newChild ?? throw new NullReferenceException());
                    }
                }
                //Get first ID column
                //sort items based on propnames and dots
                //Dont add same tempDTO
                res.Add(tempDTO);
         */

        public enum ColumnType
        {
            Simple,
            List
        }

        public class ColumnToClassMapping<TDTO> where TDTO : class, new()
        {
            public Column Column { get; set; }
            public ColumnType ColumnType { get; set; } = ColumnType.Simple;
        }

        private string GetWhereClause()
        {
            var where = "";

            foreach (var item in Conditions)
            {
                where += $"{(where.Any() ? " AND " : "")} ({item()})";
            }

            return where;
        }

        private string GetFromTablePart()
        {
            if (FromSelect == null)
                return FromTable.GetTableName();
            return $"({FromSelect}) as {FromSelect.Alias}";
        }
        private IEntity<T> GetFromTable<T>() where T : notnull
        {
            if (FromSelect == null)
                return FromTable;
            return FromSelect.FromTable;
        }

        IEnumerable<string> ReservedWords = new List<string>
        {
            "A",
"ABORT",
"ABS",
"ABSOLUTE",
"ACCESS",
"ACTION",
"ADA",
"ADD",
"ADMIN",
"AFTER",
"AGGREGATE",
"ALIAS",
"ALL",
"ALLOCATE",
"ALSO",
"ALTER",
"ALWAYS",
"ANALYSE",
"ANALYZE",
"AND",
"ANY",
"ARE",
"ARRAY",
"AS",
"ASC",
"ASENSITIVE",
"ASSERTION",
"ASSIGNMENT",
"ASYMMETRIC",
"AT",
"ATOMIC",
"ATTRIBUTE",
"ATTRIBUTES",
"AUDIT",
"AUTHORIZATION",
"AUTO_INCREMENT",
"AVG",
"AVG_ROW_LENGTH",
"BACKUP",
"BACKWARD",
"BEFORE",
"BEGIN",
"BERNOULLI",
"BETWEEN",
"BIGINT",
"BINARY",
"BIT",
"BIT_LENGTH",
"BITVAR",
"BLOB",
"BOOL",
"BOOLEAN",
"BOTH",
"BREADTH",
"BREAK",
"BROWSE",
"BULK",
"BY",
"C",
"CACHE",
"CALL",
"CALLED",
"CARDINALITY",
"CASCADE",
"CASCADED",
"CASE",
"CAST",
"CATALOG",
"CATALOG_NAME",
"CEIL",
"CEILING",
"CHAIN",
"CHANGE",
"CHAR",
"CHAR_LENGTH",
"CHARACTER",
"CHARACTER_LENGTH",
"CHARACTER_SET_CATALOG",
"CHARACTER_SET_NAME",
"CHARACTER_SET_SCHEMA",
"CHARACTERISTICS",
"CHARACTERS",
"CHECK",
"CHECKED",
"CHECKPOINT",
"CHECKSUM",
"CLASS",
"CLASS_ORIGIN",
"CLOB",
"CLOSE",
"CLUSTER",
"CLUSTERED",
"COALESCE",
"COBOL",
"COLLATE",
"COLLATION",
"COLLATION_CATALOG",
"COLLATION_NAME",
"COLLATION_SCHEMA",
"COLLECT",
"COLUMN",
"COLUMN_NAME",
"COLUMNS",
"COMMAND_FUNCTION",
"COMMAND_FUNCTION_CODE",
"COMMENT",
"COMMIT",
"COMMITTED",
"COMPLETION",
"COMPRESS",
"COMPUTE",
"CONDITION",
"CONDITION_NUMBER",
"CONNECT",
"CONNECTION",
"CONNECTION_NAME",
"CONSTRAINT",
"CONSTRAINT_CATALOG",
"CONSTRAINT_NAME",
"CONSTRAINT_SCHEMA",
"CONSTRAINTS",
"CONSTRUCTOR",
"CONTAINS",
"CONTAINSTABLE",
"CONTINUE",
"CONVERSION",
"CONVERT",
"COPY",
"CORR",
"CORRESPONDING",
"COUNT",
"COVAR_POP",
"COVAR_SAMP",
"CREATE",
"CREATEDB",
"CREATEROLE",
"CREATEUSER",
"CROSS",
"CSV",
"CUBE",
"CUME_DIST",
"CURRENT",
"CURRENT_DATE",
"CURRENT_DEFAULT_TRANSFORM_GROUP",
"CURRENT_PATH",
"CURRENT_ROLE",
"CURRENT_TIME",
"CURRENT_TIMESTAMP",
"CURRENT_TRANSFORM_GROUP_FOR_TYPE",
"CURRENT_USER",
"CURSOR",
"CURSOR_NAME",
"CYCLE",
"DATA",
"DATABASE",
"DATABASES",
"DATE",
"DATETIME",
"DATETIME_INTERVAL_CODE",
"DATETIME_INTERVAL_PRECISION",
"DAY",
"DAY_HOUR",
"DAY_MICROSECOND",
"DAY_MINUTE",
"DAY_SECOND",
"DAYOFMONTH",
"DAYOFWEEK",
"DAYOFYEAR",
"DBCC",
"DEALLOCATE",
"DEC",
"DECIMAL",
"DECLARE",
"DEFAULT",
"DEFAULTS",
"DEFERRABLE",
"DEFERRED",
"DEFINED",
"DEFINER",
"DEGREE",
"DELAY_KEY_WRITE",
"DELAYED",
"DELETE",
"DELIMITER",
"DELIMITERS",
"DENSE_RANK",
"DENY",
"DEPTH",
"DEREF",
"DERIVED",
"DESC",
"DESCRIBE",
"DESCRIPTOR",
"DESTROY",
"DESTRUCTOR",
"DETERMINISTIC",
"DIAGNOSTICS",
"DICTIONARY",
"DISABLE",
"DISCONNECT",
"DISK",
"DISPATCH",
"DISTINCT",
"DISTINCTROW",
"DISTRIBUTED",
"DIV",
"DO",
"DOMAIN",
"DOUBLE",
"DROP",
"DUAL",
"DUMMY",
"DUMP",
"DYNAMIC",
"DYNAMIC_FUNCTION",
"DYNAMIC_FUNCTION_CODE",
"EACH",
"ELEMENT",
"ELSE",
"ELSEIF",
"ENABLE",
"ENCLOSED",
"ENCODING",
"ENCRYPTED",
"END",
"END-EXEC",
"ENUM",
"EQUALS",
"ERRLVL",
"ESCAPE",
"ESCAPED",
"EVERY",
"EXCEPT",
"EXCEPTION",
"EXCLUDE",
"EXCLUDING",
"EXCLUSIVE",
"EXEC",
"EXECUTE",
"EXISTING",
"EXISTS",
"EXIT",
"EXP",
"EXPLAIN",
"EXTERNAL",
"EXTRACT",
"FALSE",
"FETCH",
"FIELDS",
"FILE",
"FILLFACTOR",
"FILTER",
"FINAL",
"FIRST",
"FLOAT",
"FLOAT4",
"FLOAT8",
"FLOOR",
"FLUSH",
"FOLLOWING",
"FOR",
"FORCE",
"FOREIGN",
"FORTRAN",
"FORWARD",
"FOUND",
"FREE",
"FREETEXT",
"FREETEXTTABLE",
"FREEZE",
"FROM",
"FULL",
"FULLTEXT",
"FUNCTION",
"FUSION",
"G",
"GENERAL",
"GENERATED",
"GET",
"GLOBAL",
"GO",
"GOTO",
"GRANT",
"GRANTED",
"GRANTS",
"GREATEST",
"GROUP",
"GROUPING",
"HANDLER",
"HAVING",
"HEADER",
"HEAP",
"HIERARCHY",
"HIGH_PRIORITY",
"HOLD",
"HOLDLOCK",
"HOST",
"HOSTS",
"HOUR",
"HOUR_MICROSECOND",
"HOUR_MINUTE",
"HOUR_SECOND",
"IDENTIFIED",
"IDENTITY",
"IDENTITY_INSERT",
"IDENTITYCOL",
"IF",
"IGNORE",
"ILIKE",
"IMMEDIATE",
"IMMUTABLE",
"IMPLEMENTATION",
"IMPLICIT",
"IN",
"INCLUDE",
"INCLUDING",
"INCREMENT",
"INDEX",
"INDICATOR",
"INFILE",
"INFIX",
"INHERIT",
"INHERITS",
"INITIAL",
"INITIALIZE",
"INITIALLY",
"INNER",
"INOUT",
"INPUT",
"INSENSITIVE",
"INSERT",
"INSERT_ID",
"INSTANCE",
"INSTANTIABLE",
"INSTEAD",
"INT",
"INT1",
"INT2",
"INT3",
"INT4",
"INT8",
"INTEGER",
"INTERSECT",
"INTERSECTION",
"INTERVAL",
"INTO",
"INVOKER",
"IS",
"ISAM",
"ISNULL",
"ISOLATION",
"ITERATE",
"JOIN",
"K",
"KEY",
"KEY_MEMBER",
"KEY_TYPE",
"KEYS",
"KILL",
"LANCOMPILER",
"LANGUAGE",
"LARGE",
"LAST",
"LAST_INSERT_ID",
"LATERAL",
"LEAD",
"LEADING",
"LEAST",
"LEAVE",
"LEFT",
"LENGTH",
"LESS",
"LEVEL",
"LIKE",
"LIMIT",
"LINENO",
"LINES",
"LISTEN",
"LN",
"LOAD",
"LOCAL",
"LOCALTIME",
"LOCALTIMESTAMP",
"LOCATION",
"LOCATOR",
"LOCK",
"LOGIN",
"LOGS",
"LONG",
"LONGBLOB",
"LONGTEXT",
"LOOP",
"LOW_PRIORITY",
"LOWER",
"M",
"MAP",
"MATCH",
"MATCHED",
"MAX",
"MAX_ROWS",
"MAXEXTENTS",
"MAXVALUE",
"MEDIUMBLOB",
"MEDIUMINT",
"MEDIUMTEXT",
"MEMBER",
"MERGE",
"MESSAGE_LENGTH",
"MESSAGE_OCTET_LENGTH",
"MESSAGE_TEXT",
"METHOD",
"MIDDLEINT",
"MIN",
"MIN_ROWS",
"MINUS",
"MINUTE",
"MINUTE_MICROSECOND",
"MINUTE_SECOND",
"MINVALUE",
"MLSLABEL",
"MOD",
"MODE",
"MODIFIES",
"MODIFY",
"MODULE",
"MONTH",
"MONTHNAME",
"MORE",
"MOVE",
"MULTISET",
"MUMPS",
"MYISAM",
"NAME",
"NAMES",
"NATIONAL",
"NATURAL",
"NCHAR",
"NCLOB",
"NESTING",
"NEW",
"NEXT",
"NO",
"NO_WRITE_TO_BINLOG",
"NOAUDIT",
"NOCHECK",
"NOCOMPRESS",
"NOCREATEDB",
"NOCREATEROLE",
"NOCREATEUSER",
"NOINHERIT",
"NOLOGIN",
"NONCLUSTERED",
"NONE",
"NORMALIZE",
"NORMALIZED",
"NOSUPERUSER",
"NOT",
"NOTHING",
"NOTIFY",
"NOTNULL",
"NOWAIT",
"NULL",
"NULLABLE",
"NULLIF",
"NULLS",
"NUMBER",
"NUMERIC",
"OBJECT",
"OCTET_LENGTH",
"OCTETS",
"OF",
"OFF",
"OFFLINE",
"OFFSET",
"OFFSETS",
"OIDS",
"OLD",
"ON",
"ONLINE",
"ONLY",
"OPEN",
"OPENDATASOURCE",
"OPENQUERY",
"OPENROWSET",
"OPENXML",
"OPERATION",
"OPERATOR",
"OPTIMIZE",
"OPTION",
"OPTIONALLY",
"OPTIONS",
"OR",
"ORDER",
"ORDERING",
"ORDINALITY",
"OTHERS",
"OUT",
"OUTER",
"OUTFILE",
"OUTPUT",
"OVER",
"OVERLAPS",
"OVERLAY",
"OVERRIDING",
"OWNER",
"PACK_KEYS",
"PAD",
"PARAMETER",
"PARAMETER_MODE",
"PARAMETER_NAME",
"PARAMETER_ORDINAL_POSITION",
"PARAMETER_SPECIFIC_CATALOG",
"PARAMETER_SPECIFIC_NAME",
"PARAMETER_SPECIFIC_SCHEMA",
"PARAMETERS",
"PARTIAL",
"PARTITION",
"PASCAL",
"PASSWORD",
"PATH",
"PCTFREE",
"PERCENT",
"PERCENT_RANK",
"PERCENTILE_CONT",
"PERCENTILE_DISC",
"PLACING",
"PLAN",
"PLI",
"POSITION",
"POSTFIX",
"POWER",
"PRECEDING",
"PRECISION",
"PREFIX",
"PREORDER",
"PREPARE",
"PREPARED",
"PRESERVE",
"PRIMARY",
"PRINT",
"PRIOR",
"PRIVILEGES",
"PROC",
"PROCEDURAL",
"PROCEDURE",
"PROCESS",
"PROCESSLIST",
"PUBLIC",
"PURGE",
"QUOTE",
"RAID0",
"RAISERROR",
"RANGE",
"RANK",
"RAW",
"READ",
"READS",
"READTEXT",
"REAL",
"RECHECK",
"RECONFIGURE",
"RECURSIVE",
"REF",
"REFERENCES",
"REFERENCING",
"REGEXP",
"REGR_AVGX",
"REGR_AVGY",
"REGR_COUNT",
"REGR_INTERCEPT",
"REGR_R2",
"REGR_SLOPE",
"REGR_SXX",
"REGR_SXY",
"REGR_SYY",
"REINDEX",
"RELATIVE",
"RELEASE",
"RELOAD",
"RENAME",
"REPEAT",
"REPEATABLE",
"REPLACE",
"REPLICATION",
"REQUIRE",
"RESET",
"RESIGNAL",
"RESOURCE",
"RESTART",
"RESTORE",
"RESTRICT",
"RESULT",
"RETURN",
"RETURNED_CARDINALITY",
"RETURNED_LENGTH",
"RETURNED_OCTET_LENGTH",
"RETURNED_SQLSTATE",
"RETURNS",
"REVOKE",
"RIGHT",
"RLIKE",
"ROLE",
"ROLLBACK",
"ROLLUP",
"ROUTINE",
"ROUTINE_CATALOG",
"ROUTINE_NAME",
"ROUTINE_SCHEMA",
"ROW",
"ROW_COUNT",
"ROW_NUMBER",
"ROWCOUNT",
"ROWGUIDCOL",
"ROWID",
"ROWNUM",
"ROWS",
"RULE",
"SAVE",
"SAVEPOINT",
"SCALE",
"SCHEMA",
"SCHEMA_NAME",
"SCHEMAS",
"SCOPE",
"SCOPE_CATALOG",
"SCOPE_NAME",
"SCOPE_SCHEMA",
"SCROLL",
"SEARCH",
"SECOND",
"SECOND_MICROSECOND",
"SECTION",
"SECURITY",
"SELECT",
"SELF",
"SENSITIVE",
"SEPARATOR",
"SEQUENCE",
"SERIALIZABLE",
"SERVER_NAME",
"SESSION",
"SESSION_USER",
"SET",
"SETOF",
"SETS",
"SETUSER",
"SHARE",
"SHOW",
"SHUTDOWN",
"SIGNAL",
"SIMILAR",
"SIMPLE",
"SIZE",
"SMALLINT",
"SOME",
"SONAME",
"SOURCE",
"SPACE",
"SPATIAL",
"SPECIFIC",
"SPECIFIC_NAME",
"SPECIFICTYPE",
"SQL",
"SQL_BIG_RESULT",
"SQL_BIG_SELECTS",
"SQL_BIG_TABLES",
"SQL_CALC_FOUND_ROWS",
"SQL_LOG_OFF",
"SQL_LOG_UPDATE",
"SQL_LOW_PRIORITY_UPDATES",
"SQL_SELECT_LIMIT",
"SQL_SMALL_RESULT",
"SQL_WARNINGS",
"SQLCA",
"SQLCODE",
"SQLERROR",
"SQLEXCEPTION",
"SQLSTATE",
"SQLWARNING",
"SQRT",
"SSL",
"STABLE",
"START",
"STARTING",
"STATE",
"STATEMENT",
"STATIC",
"STATISTICS",
"STATUS",
"STDDEV_POP",
"STDDEV_SAMP",
"STDIN",
"STDOUT",
"STORAGE",
"STRAIGHT_JOIN",
"STRICT",
"STRING",
"STRUCTURE",
"STYLE",
"SUBCLASS_ORIGIN",
"SUBLIST",
"SUBMULTISET",
"SUBSTRING",
"SUCCESSFUL",
"SUM",
"SUPERUSER",
"SYMMETRIC",
"SYNONYM",
"SYSDATE",
"SYSID",
"SYSTEM",
"SYSTEM_USER",
"TABLE",
"TABLE_NAME",
"TABLES",
"TABLESAMPLE",
"TABLESPACE",
"TEMP",
"TEMPLATE",
"TEMPORARY",
"TERMINATE",
"TERMINATED",
"TEXT",
"TEXTSIZE",
"THAN",
"THEN",
"TIES",
"TIME",
"TIMESTAMP",
"TIMEZONE_HOUR",
"TIMEZONE_MINUTE",
"TINYBLOB",
"TINYINT",
"TINYTEXT",
"TO",
"TOAST",
"TOP",
"TOP_LEVEL_COUNT",
"TRAILING",
"TRAN",
"TRANSACTION",
"TRANSACTION_ACTIVE",
"TRANSACTIONS_COMMITTED",
"TRANSACTIONS_ROLLED_BACK",
"TRANSFORM",
"TRANSFORMS",
"TRANSLATE",
"TRANSLATION",
"TREAT",
"TRIGGER",
"TRIGGER_CATALOG",
"TRIGGER_NAME",
"TRIGGER_SCHEMA",
"TRIM",
"TRUE",
"TRUNCATE",
"TRUSTED",
"TSEQUAL",
"TYPE",
"UESCAPE",
"UID",
"UNBOUNDED",
"UNCOMMITTED",
"UNDER",
"UNDO",
"UNENCRYPTED",
"UNION",
"UNIQUE",
"UNKNOWN",
"UNLISTEN",
"UNLOCK",
"UNNAMED",
"UNNEST",
"UNSIGNED",
"UNTIL",
"UPDATE",
"UPDATETEXT",
"UPPER",
"USAGE",
"USE",
"USER",
"USER_DEFINED_TYPE_CATALOG",
"USER_DEFINED_TYPE_CODE",
"USER_DEFINED_TYPE_NAME",
"USER_DEFINED_TYPE_SCHEMA",
"USING",
"UTC_DATE",
"UTC_TIME",
"UTC_TIMESTAMP",
"VACUUM",
"VALID",
"VALIDATE",
"VALIDATOR",
"VALUE",
"VALUES",
"VAR_POP",
"VAR_SAMP",
"VARBINARY",
"VARCHAR",
"VARCHAR2",
"VARCHARACTER",
"VARIABLE",
"VARIABLES",
"VARYING",
"VERBOSE",
"VIEW",
"VOLATILE",
"WAITFOR",
"WHEN",
"WHENEVER",
"WHERE",
"WHILE",
"WIDTH_BUCKET",
"WINDOW",
"WITH",
"WITHIN",
"WITHOUT",
"WORK",
"WRITE",
"WRITETEXT",
"X509",
"XOR",
"YEAR",
"YEAR_MONTH",
"ZEROFILL",
"ZONE"
        };
    }
}
