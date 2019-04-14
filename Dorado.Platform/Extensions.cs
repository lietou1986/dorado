
using Dorado.Configuration;
using Dorado.Core.Data;
using Dorado.Platform.Infrastructure;

namespace Dorado.Platform
{
    public static class Extensions
    {
        public static T GetService<T>(this object obj)
        {
            return DoradoContext.Instance.ContainerManager.GetService<T>();
        }

        public static Conn GetConn(this object obj, string connName = "Default")
        {
            return new Conn(ConnectionStringProvider.Get(connName));
        }
    }
}