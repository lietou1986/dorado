using Dorado.Spider.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;

namespace Dorado.Spider.Workload.Sql
{
    /// <summary>
    /// RepeatableStatement: Holds a Sql command that can
    /// be repeated if the Sql connection fails.
    /// </summary>
    internal class RepeatableStatement
    {
        /// <summary>
        /// The results of a query.
        /// </summary>
        public class Results
        {
            /// <summary>
            /// The DataReader associated with these results.
            /// </summary>
            public DbDataReader DataReader
            {
                get
                {
                    return _resultSet;
                }
            }

            /// <summary>
            /// The PreparedStatement that generated these results.
            /// </summary>
            private readonly DbCommand _statement;

            /// <summary>
            /// The ResultSet that was generated.
            /// </summary>
            private readonly DbDataReader _resultSet;

            /// <summary>
            /// The RepeatableStatement object that this belongs to.
            /// </summary>
            private readonly RepeatableStatement _parent;

            /// <summary>
            /// Construct a Results object.
            /// </summary>
            /// <param name="parent">The parent object, the RepeatableStatement.</param>
            /// <param name="statement">The PreparedStatement for these results.</param>
            /// <param name="resultSet">The ResultSet.</param>
            public Results(RepeatableStatement parent, DbCommand statement, DbDataReader resultSet)
            {
                this._statement = statement;
                this._resultSet = resultSet;
                this._parent = parent;
            }

            /// <summary>
            /// Close the ResultSet.
            /// </summary>
            public void Close()
            {
                this._resultSet.Close();
                _parent.ReleaseStatement(this._statement);
            }
        }

        /// <summary>
        /// The SqlWorkloadManager that created this object.
        /// </summary>
        private SqlWorkloadManager _manager;

        /// <summary>
        /// The Sql for this statement.
        /// </summary>
        private readonly String _sql;

        /// <summary>
        /// The PreparedStatements that are assigned to each thread.
        /// </summary>
        private readonly List<DbCommand> _statementCache = new List<DbCommand>();

        /// <summary>
        /// Construct a repeatable statement based on the specified Sql.
        /// </summary>
        /// <param name="sql">The Sql to base this statement on.</param>
        public RepeatableStatement(String sql)
        {
            this._sql = sql;
        }

        /// <summary>
        /// Close the statement.
        /// </summary>
        public void Close()
        {
            try
            {
                Monitor.Enter(this);

                foreach (DbCommand statement in this._statementCache)
                {
                    statement.Dispose();
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        /// <summary>
        /// Create the statement, so that it is ready to assign
        /// PreparedStatements.
        /// </summary>
        /// <param name="manager">The manager that created this statement.</param>
        public void Create(SqlWorkloadManager manager)
        {
            Close();
            this._manager = manager;
        }

        /// <summary>
        /// Execute Sql that does not return a result set. If an
        /// error occurs, the statement will be retried until it is
        /// successful. This handles broken connections.
        /// </summary>
        /// <param name="parameters">The parameters for this Sql.</param>
        public void Execute(params Object[] parameters)
        {
            DbCommand statement = null;

            try
            {
                statement = ObtainStatement();

                for (; ; )
                {
                    try
                    {
                        statement.Parameters.Clear();
                        foreach (Object parameter in parameters)
                        {
                            DbParameter dbParam = statement.CreateParameter();

                            dbParam.Value = parameter ?? DBNull.Value;
                            statement.Parameters.Add(dbParam);
                        }

                        statement.ExecuteNonQuery();
                        return;
                    }
                    catch (Exception e)
                    {
                        this._manager.Spider.Logging.Log(Logger.Level.Error,
                       "Sql Exception", e);

                        this._manager.TryOpen();
                    }
                }
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
            finally
            {
                if (statement != null)
                {
                    ReleaseStatement(statement);
                }
            }
        }

        /// <summary>
        /// Execute an Sql query that returns a result set. If an
        /// error occurs, the statement will be retried until it is
        /// successful. This handles broken connections.
        /// </summary>
        /// <param name="parameters">The parameters for this Sql.</param>
        /// <returns>The results of the query.</returns>
        public Results ExecuteQuery(params Object[] parameters)
        {
            try
            {
                var statement = ObtainStatement();

                for (; ; )
                {
                    try
                    {
                        statement.Parameters.Clear();
                        foreach (Object parameter in parameters)
                        {
                            DbParameter dbParam = statement.CreateParameter();

                            dbParam.Value = parameter ?? DBNull.Value;
                            statement.Parameters.Add(dbParam);
                        }

                        DbDataReader reader = statement.ExecuteReader();
                        Results results = new Results(this, statement, reader);
                        return results;
                    }
                    catch (Exception e)
                    {
                        this._manager.Spider.Logging.Log(Logger.Level.Error,
                         "Sql Exception", e);

                        this._manager.TryOpen();
                    }
                }
            }
            catch (Exception e)
            {
                throw (new WorkloadException(e));
            }
        }

        /// <summary>
        /// Obtain a statement. Each thread should use their own
        /// statement, and then call the releaseStatement method
        /// when they are done.
        /// </summary>
        /// <returns>A PreparedStatement object.</returns>
        private DbCommand ObtainStatement()
        {
            DbCommand result = null;

            try
            {
                Monitor.Enter(this);
                if (this._statementCache.Count == 0)
                {
                    result = this._manager.Connection.CreateCommand();
                    result.CommandText = this._sql;
                    result.Prepare();
                }
                else
                {
                    result = this._statementCache[0];
                    this._statementCache.Remove(result);
                    result.Parameters.Clear();
                }
            }
            finally
            {
                Monitor.Exit(this);
            }

            return result;
        }

        /// <summary>
        /// This method releases statements after the thread is
        /// done with them. These statements are not closed, but
        /// rather cached until another thread has need of them.
        /// </summary>
        /// <param name="stmt">The statement that is to be released.</param>
        private void ReleaseStatement(DbCommand stmt)
        {
            try
            {
                Monitor.Enter(this);
                if (!this._statementCache.Contains(stmt))
                    this._statementCache.Add(stmt);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}