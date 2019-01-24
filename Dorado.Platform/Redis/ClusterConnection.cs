using Dorado.Configuration;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Dorado.Platform.Redis
{
    public static class ClusterConnection
    {
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> Muxers = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public static ConnectionMultiplexer GetMuxer(string conn)
        {
            return Muxers.GetOrAdd(conn, c => ConnectionMultiplexer.Connect(ConnectionStringProvider.Get(c)));
        }
    }
}