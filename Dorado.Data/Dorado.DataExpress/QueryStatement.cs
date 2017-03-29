using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.PagerProvider;
using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Dorado.DataExpress
{
    public class QueryStatement : BaseQuery
    {
        protected internal List<BaseExpression> Columns = new List<BaseExpression>();
        protected readonly LinkedList<BaseExpression> Conditions = new LinkedList<BaseExpression>();
        protected readonly List<BaseExpression> Groups = new List<BaseExpression>();
        protected readonly List<BaseExpression> Havings = new List<BaseExpression>();
        protected readonly List<JoinExpression> Joins = new List<JoinExpression>();
        protected internal List<OrderExpression> Orders = new List<OrderExpression>();
        protected QueryStatement NextCriterion;
        private bool UnionAllRows;
        private bool _all;
        private int _currentPage = 1;
        private bool _distinct;
        private int _pageSize = 20;

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;
            }
        }

        public bool Distinct
        {
            get
            {
                return this._distinct;
            }
            set
            {
                this._distinct = value;
            }
        }

        public bool All
        {
            get
            {
                return this._all;
            }
            set
            {
                this._all = value;
            }
        }

        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
            set
            {
                this._currentPage = value;
            }
        }

        public QueryStatement()
        {
        }

        public QueryStatement(TableSchema table, SqlStatement st)
        {
            base.Table = table;
            base.Statement = st;
        }

        public static implicit operator DataTable(QueryStatement st)
        {
            return st.Execute();
        }

        public static implicit operator PagedDataTable(QueryStatement st)
        {
            return st.ExecutePage();
        }

        public static implicit operator DbDataReader(QueryStatement st)
        {
            return st.ExecuteReader();
        }

        public static implicit operator Array(QueryStatement st)
        {
            return st.ExecuteUnique();
        }

        public static implicit operator object[](QueryStatement st)
        {
            return st.ExecuteUnique();
        }

        public static implicit operator int(QueryStatement st)
        {
            object result = st.ExecuteScalar();
            if (result is int)
            {
                return (int)result;
            }
            if (result is decimal)
            {
                return Convert.ToInt32((decimal)result);
            }
            if (result is double)
            {
                return Convert.ToInt32((double)result);
            }
            if (result is float)
            {
                return Convert.ToInt32((float)result);
            }
            return (int)result;
        }

        public static implicit operator long(QueryStatement st)
        {
            return (long)st.ExecuteScalar();
        }

        public static implicit operator float(QueryStatement st)
        {
            return (float)st.ExecuteScalar();
        }

        public static implicit operator double(QueryStatement st)
        {
            return (double)st.ExecuteScalar();
        }

        public static implicit operator string(QueryStatement st)
        {
            return (string)st.ExecuteScalar();
        }

        public static implicit operator decimal(QueryStatement st)
        {
            return (decimal)st.ExecuteScalar();
        }

        public static implicit operator DateTime(QueryStatement st)
        {
            object result = st.ExecuteScalar();
            if (Convert.IsDBNull(result))
            {
                return DateTime.MinValue;
            }
            if (result is DateTime)
            {
                return (DateTime)result;
            }
            return DateTime.Parse(result.ToString());
        }

        public static QueryStatement operator &(QueryStatement st, BaseCompareExpression exp)
        {
            return st.Where(PreviousOperator.And, exp);
        }

        public static QueryStatement operator |(QueryStatement st, BaseCompareExpression exp)
        {
            return st.Where(PreviousOperator.Or, exp);
        }

        public static QueryStatement operator +(QueryStatement st, string column)
        {
            return st.AddColumn(column);
        }

        public static QueryStatement operator +(QueryStatement st, ColumnSchema column)
        {
            return st.AddColumn(column);
        }

        public static QueryStatement operator +(QueryStatement st, BaseExpression exp)
        {
            return st.AddColumn(exp);
        }

        public static QueryStatement operator +(QueryStatement st, object[] columns)
        {
            return st.AddColumns(columns);
        }

        internal QueryStatement AddGroup(ColumnSchema col)
        {
            SimpleColumnExpression exp = new SimpleColumnExpression(col);
            this.Groups.Add(exp);
            return this;
        }

        internal QueryStatement AddGroup(string colName)
        {
            if (!base.Statement.Database.Dialect.ValidateSystemName(colName))
            {
                throw new Exception(string.Format("列名称[{0}]错误，列名中不允许包含特殊字符!", colName));
            }
            return this.AddGroup(new ColumnSchema(colName));
        }

        public QueryStatement GroupBy(string colName)
        {
            return this.AddGroup(colName);
        }

        public QueryStatement GroupBy(ColumnSchema col)
        {
            return this.AddGroup(col);
        }

        internal QueryStatement AddSimpleGroup(string name)
        {
            SimpleNameExpression exp = new SimpleNameExpression(name);
            this.Groups.Add(exp);
            return this;
        }

        private void ProcessExpression(BaseExpression exp)
        {
            exp.Dialect = base.Statement.Database.Dialect;
            exp.Driver = base.Statement.Database.Driver;
            exp.Parent = this;
        }

        internal QueryStatement AddHaving(ColumnSchema col)
        {
            SimpleColumnExpression exp = new SimpleColumnExpression(col);
            this.Havings.Add(exp);
            return this;
        }

        internal QueryStatement AddHaving(string name)
        {
            SimpleNameExpression exp = new SimpleNameExpression(name);
            this.Havings.Add(exp);
            return this;
        }

        public QueryStatement Union(QueryStatement cri, bool unionAll)
        {
            this.UnionAllRows = unionAll;
            this.NextCriterion = cri;
            cri.Parent = this;
            return this;
        }

        public QueryStatement UnionAll(QueryStatement cri)
        {
            return this.Union(cri, true);
        }

        public QueryStatement Union(QueryStatement cri)
        {
            return this.Union(cri, false);
        }

        internal QueryStatement AddColumn(ColumnSchema col)
        {
            return this.AddColumn(col, string.Empty);
        }

        internal QueryStatement AddColumn(ColumnSchema col, string alias)
        {
            SimpleColumnExpression exp = new SimpleColumnExpression(col, alias);
            this.Columns.Add(exp);
            return this;
        }

        internal QueryStatement AddColumn(BaseExpression exp)
        {
            return this.AddColumn(exp, string.Empty);
        }

        internal QueryStatement AddColumn(BaseExpression exp, string alias)
        {
            exp.Alias = alias;
            this.Columns.Add(exp);
            return this;
        }

        internal QueryStatement AddColumn(string colName)
        {
            if (!base.Statement.Database.Dialect.ValidateSystemName(colName))
            {
                throw new Exception(string.Format("列名称[{0}]错误，列名中不允许包含特殊字符!", colName));
            }
            return this.AddColumn(new ColumnSchema(colName));
        }

        internal QueryStatement AddColumns(params object[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                object col = columns[i];
                if (col is ColumnSchema)
                {
                    this.AddColumn((ColumnSchema)col);
                }
                else
                {
                    if (col is string)
                    {
                        this.AddColumn((string)col);
                    }
                    else
                    {
                        if (col is BaseExpression)
                        {
                            this.AddColumn((BaseExpression)col);
                        }
                        else
                        {
                            this.AddColumn(SqlExpression.Val(col));
                        }
                    }
                }
            }
            return this;
        }

        internal QueryStatement AddJoin(JoinExpression join)
        {
            this.Joins.Add(join);
            return this;
        }

        public QueryStatement LeftJoin(TableSchema table, ColumnSchema colLeft, ColumnSchema colRight)
        {
            return this.AddJoin(SqlExpression.LeftJoin(table, colLeft, colRight));
        }

        public QueryStatement LeftJoin(string tableName, string colLeftName, string colRightName)
        {
            TableSchema table = new TableSchema(tableName);
            return this.LeftJoin(table, new ColumnSchema(tableName, colLeftName)
            {
                Table = base.Table
            }, new ColumnSchema(base.Table.Name, colRightName)
            {
                Table = table
            });
        }

        public QueryStatement InnerJoin(TableSchema table, ColumnSchema colLeft, ColumnSchema colRight)
        {
            return this.AddJoin(SqlExpression.InnerJoin(table, colLeft, colRight));
        }

        public QueryStatement InnerJoin(string tableName, string colLeftName, string colRightName)
        {
            TableSchema table = new TableSchema(tableName);
            ColumnSchema colLeft = new ColumnSchema(base.Table, colLeftName);
            ColumnSchema colRight = new ColumnSchema(table, colRightName);
            return this.InnerJoin(table, colLeft, colRight);
        }

        internal QueryStatement AddCondition(BaseCompareExpression exp)
        {
            this.Conditions.AddLast(exp);
            return this;
        }

        internal QueryStatement AddCondition(PreviousOperator pre, BaseCompareExpression exp)
        {
            exp.PreRelation = pre;
            this.Conditions.AddLast(exp);
            return this;
        }

        public QueryStatement Where(BaseCompareExpression exp)
        {
            return this.AddCondition(exp);
        }

        public QueryStatement Where(BaseExpression exp)
        {
            this.Conditions.AddLast(exp);
            return this;
        }

        public QueryStatement Where(PreviousOperator pre, BaseCompareExpression exp)
        {
            return this.AddCondition(pre, exp);
        }

        internal QueryStatement AddOrder(OrderExpression order)
        {
            this.Orders.Add(order);
            return this;
        }

        public QueryStatement OrderBy(ColumnSchema col, OrderMethod method)
        {
            if (method != OrderMethod.ASC)
            {
                return this.AddOrder(SqlExpression.DescOrder(col));
            }
            return this.AddOrder(SqlExpression.Order(col));
        }

        public QueryStatement OrderBy(string colName, OrderMethod method)
        {
            if (method != OrderMethod.ASC)
            {
                return this.AddOrder(SqlExpression.DescOrder(colName));
            }
            return this.AddOrder(SqlExpression.Order(colName));
        }

        public QueryStatement OrderBy(string colName)
        {
            return this.OrderBy(colName, OrderMethod.ASC);
        }

        public QueryStatement OrderBy(params string[] columns)
        {
            return this.OrderBy(OrderMethod.ASC, columns);
        }

        public QueryStatement OrderBy(ColumnSchema col)
        {
            return this.OrderBy(col, OrderMethod.ASC);
        }

        public QueryStatement OrderBy(params ColumnSchema[] columns)
        {
            return this.OrderBy(OrderMethod.ASC, columns);
        }

        public QueryStatement OrderBy(OrderExpression orderExpression)
        {
            return this.AddOrder(orderExpression);
        }

        public QueryStatement OrderBy(OrderMethod method, params string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; i++)
            {
                string colName = columnNames[i];
                this.OrderBy(colName, method);
            }
            return this;
        }

        public QueryStatement OrderBy(OrderMethod method, params ColumnSchema[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnSchema column = columns[i];
                this.OrderBy(column, method);
            }
            return this;
        }

        public QueryStatement SetDistinct()
        {
            this._distinct = true;
            return this;
        }

        public QueryStatement SetAll()
        {
            this._all = true;
            return this;
        }

        public QueryStatement SetCurrentPage(int page)
        {
            this.CurrentPage = page;
            return this;
        }

        public DataTable Execute()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            DataTable table = base.Statement.ExecuteDataTable();
            table.TableName = (string.IsNullOrEmpty(base.Table.Alias) ? base.Table.Name : base.Table.Alias);
            return table;
        }

        public long Count()
        {
            base.Statement.Command.CommandText = this.GenerateCountSql();
            this.ProcessParameters();
            object result = base.Statement.ExecuteScalar();
            return Convert.ToInt64(result);
        }

        public QueryStatement From(string table)
        {
            return this.From(new TableSchema(table));
        }

        public QueryStatement From(TableSchema table)
        {
            base.Table = table;
            return this;
        }

        public QueryStatement From<TEntity>() where TEntity : new()
        {
            return this.From(BinderManager<TEntity>.Binder.EntifyInfo.Schema);
        }

        public PagedDataTable Execute(int page)
        {
            this.CurrentPage = page;
            BasePager provider = PagerFactory.GetProvider(base.Statement.Database.Driver);
            provider.Query = this;
            return provider.ExecuteByPageSize(this.PageSize, this.CurrentPage);
        }

        internal PagedEntity<T> Execute<T>(int page) where T : new()
        {
            this.CurrentPage = page;
            BasePager provider = PagerFactory.GetProvider(base.Statement.Database.Driver);
            provider.Query = this;
            return provider.ExecuteByPageSize<T>(this.PageSize, this.CurrentPage);
        }

        public PagedDataTable Execute(int start, int limit)
        {
            BasePager provider = PagerFactory.GetProvider(base.Statement.Database.Driver);
            provider.Query = this;
            provider.PageSize = this.PageSize;
            return provider.ExecuteByLimit(start, limit);
        }

        internal PagedEntity<T> Execute<T>(int start, int limit) where T : new()
        {
            BasePager provider = PagerFactory.GetProvider(base.Statement.Database.Driver);
            provider.Query = this;
            provider.PageSize = this.PageSize;
            return provider.ExecuteByLimit<T>(start, limit);
        }

        public PagedDataTable ExecutePage()
        {
            return this.Execute(this.CurrentPage);
        }

        protected PagedEntity<T> ExecutePage<T>() where T : new()
        {
            return this.Execute<T>(this.CurrentPage);
        }

        public QueryStatement SetPageSize(int pageSize)
        {
            this.PageSize = pageSize;
            return this;
        }

        public DbDataReader ExecuteReader()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.ExecuteReader();
        }

        public List<T> Bind<T>(Action<T, IDataRecord> bindAction) where T : new()
        {
            if (bindAction == null)
            {
                throw new ArgumentNullException("bindAction");
            }
            List<T> result;
            using (DbDataReader reader = this.ExecuteReader())
            {
                List<T> entityList;
                if (reader.HasRows)
                {
                    entityList = new List<T>();
                    while (reader.Read())
                    {
                        T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        bindAction(entity, reader);
                        entityList.Add(entity);
                    }
                }
                else
                {
                    entityList = null;
                }
                result = entityList;
            }
            return result;
        }

        public object ExecuteScalar()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.ExecuteScalar();
        }

        public object[] ExecuteUnique()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.ExecuteUnique();
        }

        public override void ProcessParameters()
        {
            foreach (BaseExpression exp in this.Conditions)
            {
                exp.ProcessParameter(base.Statement);
            }
            foreach (BaseExpression exp2 in this.Columns)
            {
                exp2.ProcessParameter(base.Statement);
            }
            foreach (BaseExpression exp3 in this.Havings)
            {
                exp3.ProcessParameter(base.Statement);
            }
            foreach (BaseExpression exp4 in this.Groups)
            {
                exp4.ProcessParameter(base.Statement);
            }
            if (this.NextCriterion != null)
            {
                this.NextCriterion.Statement = base.Statement;
                this.NextCriterion.ProcessParameters();
            }
        }

        public void Clear()
        {
            this.Columns.Clear();
            this.Joins.Clear();
            this.Conditions.Clear();
            this.Orders.Clear();
            this.Groups.Clear();
            this.Havings.Clear();
            this.NextCriterion = null;
            this.ParamCount = 0;
        }

        public override string GenerateSql()
        {
            this.ParamCount = 0;
            StringBuilder builder = new StringBuilder(256);
            if (!string.IsNullOrEmpty(base.InitialeSql))
            {
                builder.Append(base.InitialeSql);
            }
            builder.Append(base.Dialect.GetKeyword("select")).Append(base.Dialect.NewLine).Append(" ").Append(this.All ? (" " + base.Dialect.GetKeyword("all") + " ") : (this.Distinct ? (" " + base.Dialect.GetKeyword("DISTINCT") + " ") : "")).Append(this.GenerateColumns()).Append(base.Dialect.NewLine).Append(" ").Append(base.Dialect.GetKeyword("from")).Append(" ").Append(base.Dialect.NewLine).Append(base.Dialect.GetSystemName(base.Table)).Append(this.GenerateJoins()).Append(this.GenerateGroups()).Append(this.GenerateHavings()).Append(this.GenerateCondition()).Append(this.GenerateOrders());
            if (this.NextCriterion != null)
            {
                builder.Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("UNION")).Append(" ");
                if (this.UnionAllRows)
                {
                    builder.Append(base.Dialect.GetKeyword("ALL")).Append(" ");
                }
                builder.Append(base.Dialect.NewLine);
                builder.Append(this.NextCriterion.GenerateSql());
            }
            if (!string.IsNullOrEmpty(base.FinalizeSql))
            {
                builder.Append(base.FinalizeSql);
            }
            return builder.ToString();
        }

        internal string GenerateCountSql()
        {
            this.ParamCount = 0;
            StringBuilder builder = new StringBuilder(256);
            if (!string.IsNullOrEmpty(base.InitialeSql))
            {
                builder.Append(base.InitialeSql);
            }
            builder.Append(base.Dialect.GetKeyword("select")).Append(base.Dialect.NewLine).Append(" ").Append(this.All ? (" " + base.Dialect.GetKeyword("all") + " ") : (this.Distinct ? (" " + base.Dialect.GetKeyword("DISTINCT") + " ") : "")).Append(" ").Append(base.Dialect.GetKeyword("count")).Append(" (*)  ").Append(base.Dialect.GetKeyword("as ")).Append(base.Dialect.GetKeyword("Total")).Append(base.Dialect.NewLine).Append(" ").Append(base.Dialect.GetKeyword("from")).Append(" ").Append(base.Dialect.NewLine).Append(base.Dialect.GetSystemName(base.Table)).Append(this.GenerateJoins()).Append(this.GenerateGroups()).Append(this.GenerateHavings()).Append(this.GenerateCondition());
            if (this.NextCriterion != null)
            {
                builder.Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("UNION")).Append(" ");
                if (this.UnionAllRows)
                {
                    builder.Append(base.Dialect.GetKeyword("ALL")).Append(" ");
                }
                builder.Append(base.Dialect.NewLine);
                builder.Append(this.NextCriterion.GenerateSql());
            }
            if (!string.IsNullOrEmpty(base.FinalizeSql))
            {
                builder.Append(base.FinalizeSql);
            }
            return builder.ToString();
        }

        public new QueryStatement SetFinalizeSql(string sql)
        {
            base.SetFinalizeSql(sql);
            return this;
        }

        public new QueryStatement SetInitialSql(string sql)
        {
            base.SetInitialSql(sql);
            return this;
        }

        private string GenerateHavings()
        {
            if (this.Havings.Count == 0 || this.Groups.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(32);
            builder.Append(base.Dialect.NewLine);
            builder.Append(" HAVEING ");
            foreach (BaseExpression exp in this.Havings)
            {
                this.ProcessExpression(exp);
                builder.Append(exp.Sql);
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        internal string GenerateOrders()
        {
            if (this.Orders.Count == 0 || this.NextCriterion != null)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(32);
            builder.Append(" ").Append(base.Dialect.GetKeyword("order by")).Append(" ");
            foreach (OrderExpression order in this.Orders)
            {
                this.ProcessExpression(order);
                builder.Append(order.Sql);
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        private string GenerateGroups()
        {
            if (this.Groups.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(32);
            builder.Append(base.Dialect.NewLine).Append(" ").Append(base.Dialect.GetKeyword("group by")).Append(" ");
            foreach (BaseExpression exp in this.Groups)
            {
                this.ProcessExpression(exp);
                builder.Append(exp.Sql);
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        private string GenerateColumns()
        {
            if (this.Columns.Count == 0)
            {
                return " * ";
            }
            StringBuilder builder = new StringBuilder(32);
            builder.Append(base.Dialect.NewLine);
            foreach (BaseExpression col in this.Columns)
            {
                this.ProcessExpression(col);
                builder.Append(col.Sql);
                if (!string.IsNullOrEmpty(col.Alias))
                {
                    builder.Append(" '" + col.Alias.Replace("'", "''") + "'");
                }
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        private string GenerateJoins()
        {
            if (this.Joins.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(32);
            builder.Append(base.Dialect.NewLine);
            foreach (JoinExpression join in this.Joins)
            {
                this.ProcessExpression(join);
                builder.Append(join.Sql);
            }
            return builder.ToString();
        }

        private string GenerateCondition()
        {
            if (this.Conditions.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(64);
            builder.Append(base.Dialect.NewLine).Append(" ").Append(base.Dialect.GetKeyword("where")).Append(" ");
            LinkedListNode<BaseExpression> exp = this.Conditions.First;
            while (exp != null)
            {
                this.ProcessExpression(exp.Value);
                builder.Append(exp.Value.Sql);
                exp = exp.Next;
                if (exp != null)
                {
                    switch (exp.Value.PreRelation)
                    {
                        case PreviousOperator.And:
                            {
                                builder.Append(base.Dialect.GetKeyword(" AND "));
                                break;
                            }
                        case PreviousOperator.Or:
                            {
                                builder.Append(base.Dialect.GetKeyword(" OR "));
                                break;
                            }
                        default:
                            {
                                builder.Append(base.Dialect.GetKeyword(" AND "));
                                break;
                            }
                    }
                }
            }
            return builder.ToString();
        }

        public List<TEntity> List<TEntity>() where TEntity : new()
        {
            return BinderManager<TEntity>.Binder.List(this);
        }

        public TEntity First<TEntity>() where TEntity : new()
        {
            return BinderManager<TEntity>.Binder.First(this);
        }

        public PagedEntity<TEntity> List<TEntity>(int page) where TEntity : new()
        {
            return this.Execute<TEntity>(page);
        }

        public PagedEntity<TEntity> List<TEntity>(int start, int limit) where TEntity : new()
        {
            return this.Execute<TEntity>(start, limit);
        }

        public PagedEntity<TEntity> FirstPage<TEntity>() where TEntity : new()
        {
            return this.Execute<TEntity>(1);
        }

        public override int GetHashCode()
        {
            return this.GenerateSql().GetHashCode();
        }
    }

    public class QueryStatement<TEntity> : QueryStatement where TEntity : new()
    {
        public QueryStatement()
        {
            base.Table = BinderManager<TEntity>.Binder.EntifyInfo.Schema;
        }

        public QueryStatement(SqlStatement sql)
            : base(null, sql)
        {
            base.Table = BinderManager<TEntity>.Binder.EntifyInfo.Schema;
        }

        public new List<TEntity> All()
        {
            return base.List<TEntity>();
        }

        public TEntity First()
        {
            return base.First<TEntity>();
        }

        public PagedEntity<TEntity> FirstPage()
        {
            return base.FirstPage<TEntity>();
        }

        public PagedEntity<TEntity> List(int pageNumber)
        {
            return base.List<TEntity>(pageNumber);
        }

        public PagedEntity<TEntity> List(int start, int limit)
        {
            return base.List<TEntity>(start, limit);
        }

        public new QueryStatement<TEntity> Where(BaseExpression expression)
        {
            base.Where(expression);
            return this;
        }

        public new QueryStatement<TEntity> Where(BaseCompareExpression expression)
        {
            base.Where(expression);
            return this;
        }

        public new QueryStatement<TEntity> Where(PreviousOperator pre, BaseCompareExpression expression)
        {
            base.Where(pre, expression);
            return this;
        }

        public new QueryStatement<TEntity> OrderBy(ColumnSchema column, OrderMethod orderMethod)
        {
            base.OrderBy(column, orderMethod);
            return this;
        }

        public new QueryStatement<TEntity> OrderBy(ColumnSchema column)
        {
            base.OrderBy(column);
            return this;
        }

        public new QueryStatement<TEntity> OrderBy(string column)
        {
            base.OrderBy(column);
            return this;
        }

        public List<TEntity> ToList()
        {
            return base.List<TEntity>();
        }

        public new QueryStatement<TEntity> SetPageSize(int pageSize)
        {
            base.SetPageSize(pageSize);
            return this;
        }
    }
}