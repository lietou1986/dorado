using Dorado.DataExpress.Ldo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Dorado.DataExpress
{
    public static class DataAccessBaseExtension
    {
        public static Database GetDataContext(this IDataAccess dao)
        {
            string name = dao.GetType().FullName;
            Database db = DatabaseManager.GetDatabase(name);
            if (db.Pool.EnableFilters && !DataAccessBaseExtension.CanAccess(db, name))
            {
                throw new OperationCanceledException(string.Format("类:{0}未被授权访问数据库", name));
            }
            return db;
        }

        public static Database GetDataContext(this IDataAccess dao, string poolName)
        {
            string name = dao.GetType().FullName;
            Database db = DatabaseManager.GetDatabase(poolName, name);
            if (db.Pool.EnableFilters && !DataAccessBaseExtension.CanAccess(db, name))
            {
                throw new OperationCanceledException(string.Format("类:{0}未被授权访问数据库", name));
            }
            return db;
        }

        internal static bool CanAccess(Database db, string name)
        {
            return db.Pool.Filters.Count == 0 || db.Pool.Filters.Any((KeyValuePair<string, Regex> filter) => filter.Value.IsMatch(name));
        }

        internal static string GetField<T>(this object obj, string propName) where T : new()
        {
            return BinderManager<T>.Binder.EntifyInfo[propName];
        }

        internal static FieldAttribute GetPrimaryKey<T>(this object obj) where T : new()
        {
            return BinderManager<T>.Binder.EntifyInfo.PrimaryKey.Field;
        }

        public static QueryStatement<T> From<T>(this DataAccessBase dataAccessBase) where T : new()
        {
            return dataAccessBase.DataContext.From<T>();
        }

        public static SqlStatement Procedure(this DataAccessBase dataAccessBase, string procedureName)
        {
            return dataAccessBase.DataContext.Procedure(procedureName);
        }

        public static SqlStatement Sql(this DataAccessBase dataAccessBase, string sql)
        {
            return dataAccessBase.DataContext.Sql(sql);
        }

        public static InsertStatement<T> InsertInto<T>(this DataAccessBase dataAccessBase) where T : new()
        {
            return dataAccessBase.DataContext.InsertInto<T>();
        }

        public static InsertStatement<T> InsertInto<T>(this DataAccessBase dataAccessBase, Expression<Func<T>> newObjectExpression) where T : new()
        {
            return dataAccessBase.DataContext.InsertInto<T>(newObjectExpression);
        }

        public static int Delete<T>(this DataAccessBase dataAccessBase, object primaryKey) where T : new()
        {
            return dataAccessBase.DataContext.Delete<T>(primaryKey);
        }

        public static Transaction BeginTransaction(this DataAccessBase dataAccessBase)
        {
            return dataAccessBase.DataContext.BeginTransaction();
        }
    }
}