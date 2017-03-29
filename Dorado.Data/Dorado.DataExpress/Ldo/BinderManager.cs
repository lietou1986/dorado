using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.Ldo
{
    public static class BinderManager<TEntity> where TEntity : new()
    {
        private static IEntityBinder<TEntity> _binder;

        public static IEntityBinder<TEntity> Binder
        {
            get
            {
                return BinderManager<TEntity>._binder;
            }
            set
            {
                BinderManager<TEntity>._binder = value;
            }
        }

        public static LdoEntityInfo EntityInfo
        {
            get;
            private set;
        }

        static BinderManager()
        {
            BinderManager<TEntity>._binder = new ReflectionBinder<TEntity>();
            BinderManager<TEntity>.EntityInfo = BinderManager<TEntity>._binder.EntifyInfo;
        }

        public static ColumnSchema GetColumnSchema(string propertyName)
        {
            DataProperty pro = null;
            if (BinderManager<TEntity>.EntityInfo.Properties.TryGetValue(propertyName, out pro))
            {
                return pro.Schema;
            }
            return null;
        }
    }
}