using Dorado.DataExpress.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Dorado.DataExpress.Ldo
{
    public class ReflectionBinder<TEntity> : IEntityBinder<TEntity> where TEntity : new()
    {
        private static readonly LdoEntityInfo EntityInfo;
        private static readonly Type Type;

        public LdoEntityInfo EntifyInfo
        {
            get
            {
                return ReflectionBinder<TEntity>.EntityInfo;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public TEntity CreateInstance()
        {
            return (TEntity)ReflectionBinder<TEntity>.Type.New();
        }

        public List<TEntity> List(QueryStatement query)
        {
            using (DbDataReader reader = query.ExecuteReader())
            {
                List<DataReaderField> fields = reader.GetFieldsInfo();
                if (reader.HasRows)
                {
                    List<TEntity> entities = new List<TEntity>();
                    while (reader.Read())
                    {
                        TEntity entity = this.CreateInstance();
                        this.ReadEntity(reader, fields, entity);
                        entities.Add(entity);
                    }
                    return entities;
                }
            }
            return new List<TEntity>();
        }

        public void ReadEntity(DbDataReader reader, List<DataReaderField> fields, TEntity entity)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (ReflectionBinder<TEntity>.EntityInfo.Fields.ContainsKey(fields[i].FieldName))
                {
                    DataProperty pi = ReflectionBinder<TEntity>.EntityInfo.Fields[fields[i].FieldName];
                    if (reader.IsDBNull(i))
                    {
                        pi.Property.SetValue(entity, null);
                    }
                    else
                    {
                        pi.Property.FastSetValue(entity, reader.GetTypedValue(i, pi.Property.PropertyType));
                    }
                }
            }
        }

        public TEntity First(QueryStatement query)
        {
            using (DbDataReader reader = query.ExecuteReader())
            {
                List<DataReaderField> fields = reader.GetFieldsInfo();
                if (reader.HasRows)
                {
                    reader.Read();
                    TEntity entity = this.CreateInstance();
                    this.ReadEntity(reader, fields, entity);
                    return entity;
                }
            }
            return default(TEntity);
        }

        public int Update(UpdateStatement update, TEntity entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(DeleteStatement delete, TEntity entity)
        {
            throw new NotImplementedException();
        }

        public int Update(UpdateStatement update, object obj)
        {
            throw new NotImplementedException();
        }

        static ReflectionBinder()
        {
            ReflectionBinder<TEntity>.Type = typeof(TEntity);
            ReflectionBinder<TEntity>.EntityInfo = new LdoEntityInfo(typeof(TEntity));
        }
    }
}