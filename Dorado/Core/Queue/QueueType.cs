namespace Dorado.Core.Queue
{
    /// <summary>
    /// 队列类型
    /// </summary>
    public enum QueueType
    {
        /// <summary>
        /// http://msdn.microsoft.com/zh-cn/library/ms711472%28v=VS.85%29.aspx
        /// </summary>
        Msmq,

        /// <summary>
        /// http://www.oschina.net/p/rabbitmq
        /// </summary>
        RabbitMq,

        /// <summary>
        ///网络通讯库，内存式，据说速度比上面的队列快100倍
        ///http://www.oschina.net/p/0mq
        /// </summary>
        ZeroMq,

        /// <summary>
        ///SQL Server Service Broker 队列
        ///http://msdn.microsoft.com/zh-cn/library/ms345108%28v=sql.90%29.aspx
        /// </summary>
        Sssb,

        /// <summary>
        ///高性能内存数据库
        /// </summary>
        Redis,

        /// <summary>
        ///嵌入式数据库
        /// </summary>
        Sqlite
    }
}