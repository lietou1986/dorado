using Dorado.DataExpress.Ldo;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dorado.DataExpress
{
    public static class DataAccessObjectExtension
    {
        public static InsertStatement<T> Insert<T>(this DataAccessObject<T> dataAccessObject, Expression<Func<T>> newObjectExpression) where T : class, new()
        {
            return dataAccessObject.InsertInto(newObjectExpression);
        }

        public static int Delete<T>(this DataAccessObject<T> dataAccessObject, object primaryKeyValue) where T : class, new()
        {
            return dataAccessObject.DataContext.Delete<T>(primaryKeyValue);
        }

        public static PagedEntity<T> GetPageIf<T>(this DataAccessObject<T> dataAccessObject, int pageNumber, Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] orderBy) where T : class, new()
        {
            return dataAccessObject.GetPageIf(pageNumber, 20, condition, orderBy);
        }

        public static PagedEntity<T> GetPageIf<T>(this DataAccessObject<T> dataAccessObject, int pageNumber, int pageSize, Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] orderBy) where T : class, new()
        {
            QueryStatement<T> query = dataAccessObject.DataContext.From<T>().Where(condition).SetPageSize(pageSize);
            if (orderBy != null && orderBy.Length > 0)
            {
                query.OrderBy(orderBy);
            }
            else
            {
                query.OrderBy(BinderManager<T>.EntityInfo.PrimaryKey.Schema);
            }
            return query.List(pageNumber);
        }

        public static List<T> ListIf<T>(this DataAccessObject<T> dataAccessObject, Expression<Func<T, bool>> condition) where T : class, new()
        {
            return dataAccessObject.DataContext.From<T>().Where(condition).All();
        }

        public static QueryStatement<T> From<T>(this DataAccessObject<T> dataAccessObject) where T : class, new()
        {
            return dataAccessObject.DataContext.From<T>();
        }

        public static QueryStatement<T> Select<T>(this DataAccessObject<T> dataAccessObject, params Expression<Func<T, object>>[] expressions) where T : class, new()
        {
            return dataAccessObject.DataContext.From<T>().Select(expressions);
        }
    }
}