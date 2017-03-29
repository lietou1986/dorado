using System.Collections.Generic;
using System.Data.Common;

namespace Dorado.DataExpress.Ldo
{
    public interface IEntityBinder<TEntity> where TEntity : new()
    {
        LdoEntityInfo EntifyInfo
        {
            get;
            set;
        }

        TEntity CreateInstance();

        List<TEntity> List(QueryStatement query);

        TEntity First(QueryStatement query);

        int Update(UpdateStatement update, TEntity entity);

        int Update(UpdateStatement update, object obj);

        int Delete(DeleteStatement delete, TEntity entity);

        void ReadEntity(DbDataReader reader, List<DataReaderField> fields, TEntity entity);
    }
}