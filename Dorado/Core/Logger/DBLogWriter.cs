using Dorado.Core.Cache;
using Dorado.Core.Data;
using Dorado.Core.GlobalTimer;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dorado.Core.Logger
{
    /// <summary>
    /// 基于数据库的日志记录策略
    /// </summary>
    /// <typeparam name="TLogItem">日志类型</typeparam>
    public class DBLogWriter<TLogItem> : ILogWriter<TLogItem>, IDisposable
        where TLogItem : ILogItem
    {
        protected readonly Object ThisLock = new Object();
        private static readonly Cache<string, object> _memory = new Cache<string, object>();
        private readonly IGlobalTimerTaskHandle _taskHandle;
        protected bool IsOnlyError;
        protected string ConnectionString;
        protected string TableName;
        protected readonly List<TLogItem> LogItems = new List<TLogItem>();
        protected int BufferSize = 100;//内存中缓存的日志记录数
        private const string SqlTemplate = "CREATE TABLE [{0}]([ID] [bigint] IDENTITY(1,1) NOT NULL,[Type] [tinyint] NOT NULL,[Flag] [nvarchar](200) NOT NULL,[Message] [nvarchar](MAX) NOT NULL,[Time] [datetime] NOT NULL,CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED([ID] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="isOnlyError"></param>
        public DBLogWriter(string connectionString, string tableName = "Log", bool isOnlyError = false)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));

            IsOnlyError = isOnlyError;
            ConnectionString = connectionString;
            TableName = tableName;
            _taskHandle = Global.GlobalTimer.Add(TimeSpan.FromSeconds(5), new TaskFuncAdapter(WriteLogItems), false);
        }

        /// <summary>
        /// 每天创建一个表
        /// </summary>
        /// <returns></returns>
        private bool CreateTable(string tableName)
        {
            object cacheValue = _memory.Get(tableName);
            if (cacheValue != null && cacheValue.ToString() == "exist")
                return true;

            try
            {
                lock (ThisLock)
                {
                    using (Conn conn = new Conn(ConnectionString))
                    {
                        DataArray data = conn.Select(string.Format("select top 1 * from sysobjects where name = '{0}' and type='U'", tableName));
                        if (data.IsEmpty)
                        {
                            conn.ExecuteNonQuery(string.Format(SqlTemplate, tableName));
                        }
                    }
                }
                _memory.AddOfRelative(tableName, "exist", TimeSpan.FromDays(2));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 批量记录日志
        /// </summary>
        /// <param name="items">日志</param>
        public void Write(IEnumerable<TLogItem> items)
        {
            if (items == null)
                return;

            lock (LogItems)
            {
                foreach (TLogItem item in items)
                {
                    LogItems.Add(item);
                }
            }
            if (LogItems.Count > BufferSize)
                WriteLogItems();
        }

        protected virtual void WriteLogItems()
        {
            if (LogItems.Count == 0)
                return;

            try
            {
                //按时间命名数据表表名
                string tableName = string.Format("{0}_{1}", TableName, DateTime.Now.ToString("yyyyMMdd"));
                if (!CreateTable(tableName)) return;

                lock (LogItems)
                {
                    SqlConnection conn = new SqlConnection(ConnectionString);
                    conn.Open();
                    var dataTable = IsOnlyError ? LogItems.Where(n => n.Type == LogType.Error).ToList().ToDataTable() : LogItems.ToDataTable();
                    dataTable.Columns.Add("ID");
                    dataTable.Columns[dataTable.Columns.Count - 1].SetOrdinal(0);
                    SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn)
                    {
                        DestinationTableName = tableName,
                        BatchSize = dataTable.Rows.Count
                    };
                    try
                    {
                        if (dataTable.Rows.Count != 0)
                        {
                            sqlBulkCopy.WriteToServer(dataTable);
                        }
                    }
                    finally
                    {
                        conn.Close();
                        sqlBulkCopy.Close();
                    }
                    LogItems.Clear();
                }
            }
            catch
            {
                // ignored
            }
        }

        #region IDisposable Members

        ~DBLogWriter()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _taskHandle.Dispose();
            WriteLogItems();
        }

        #endregion IDisposable Members
    }
}