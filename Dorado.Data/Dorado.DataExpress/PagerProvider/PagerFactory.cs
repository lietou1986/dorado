using Dorado.DataExpress.Driver;
using Dorado.DataExpress.Utility;
using System;
using System.Collections;
using System.Threading;

namespace Dorado.DataExpress.PagerProvider
{
    public class PagerFactory
    {
        private static readonly Hashtable RegisterdProvider;
        private static readonly Type BaseDirverType;
        private static readonly Type BasePagerProviderType;

        static PagerFactory()
        {
            PagerFactory.RegisterdProvider = new Hashtable();
            PagerFactory.BaseDirverType = typeof(BaseDriver);
            PagerFactory.BasePagerProviderType = typeof(BasePager);
            PagerFactory.Register(typeof(MySql), typeof(MySqlPager));
            PagerFactory.Register(typeof(MsSql2000), typeof(MsSql2000Pager));
            PagerFactory.Register(typeof(MsSql2005), typeof(MsSql2005Pager));
            PagerFactory.Register(typeof(Oracle), typeof(OraclePager));
            PagerFactory.Register(typeof(NativeOracle), typeof(OraclePager));
        }

        private PagerFactory()
        {
        }

        public static void Register(Type driverType, Type providerType)
        {
            Hashtable registerdProvider;
            Monitor.Enter(registerdProvider = PagerFactory.RegisterdProvider);
            try
            {
                if (!PagerFactory.BaseDirverType.IsAssignableFrom(driverType))
                {
                    throw new Exception("驱动程序必须继承自BaseDriver[" + driverType + "]");
                }
                if (!PagerFactory.BasePagerProviderType.IsAssignableFrom(providerType))
                {
                    throw new Exception("分页提供程序必须继承自BasePagerProvider[" + providerType + "]");
                }
                PagerFactory.RegisterdProvider.Add(driverType, providerType);
            }
            finally
            {
                Monitor.Exit(registerdProvider);
            }
        }

        public static BasePager GetProvider(BaseDriver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException("driver");
            }
            if (PagerFactory.RegisterdProvider.ContainsKey(driver.GetType()))
            {
                Type t = (Type)PagerFactory.RegisterdProvider[driver.GetType()];
                return (BasePager)t.New();
            }
            throw new Exception(string.Format("针对驱动[{0}]的数据分页程序未提供!", driver.GetType().FullName));
        }
    }
}