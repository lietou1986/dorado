using System;

namespace Dorado.Spider.Workload.Sql
{
    /// <summary>
    /// SqlHolder: Holds the Sql commands for most databases.
    /// Some databases will require modifications of some of
    /// these commands.
    /// </summary>
    public class SqlHolder
    {
        /// <summary>
        /// Sql to clear the workload.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlClear()
        {
            return "DELETE FROM spider_workload";
        }

        /// <summary>
        /// Sql to clear the host table.  Leave any hosts marked to ignore.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlClear2()
        {
            return "DELETE FROM spider_host WHERE status <> 'I'";
        }

        /// <summary>
        /// Add to the workload.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlAdd()
        {
            return "INSERT INTO spider_workload(host,url,status,depth,url_hash,source_id) VALUES(?,?,?,?,?,?)";
        }

        /// <summary>
        /// Add to the host table.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlAdd2()
        {
            return "INSERT INTO spider_host(host,status,urls_done,urls_error) VALUES(?,?,?,?)";
        }

        /// <summary>
        /// Get the next workload item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetWork()
        {
            return "SELECT workload_id,URL FROM spider_workload WHERE status =  ? AND host = ?";
        }

        /// <summary>
        /// Update a workload item just obtained.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetWork2()
        {
            return "UPDATE spider_workload SET status =  ? WHERE workload_id = ?";
        }

        /// <summary>
        /// Determine if the workload is empty.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlWorkloadEmpty()
        {
            return "SELECT COUNT(*) FROM spider_workload WHERE status in ('P','W') AND host =  ?";
        }

        /// <summary>
        /// Set the workload status of an item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlSetWorkloadStatus()
        {
            return "UPDATE spider_workload SET status =  ? WHERE workload_id =  ?";
        }

        /// <summary>
        /// Set the host status of an item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlSetWorkloadStatus2()
        {
            return "UPDATE spider_host SET urls_done =  urls_done + ?, urls_error =  urls_error + ? WHERE host =  ?";
        }

        /// <summary>
        /// Get the depth of a workload item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetDepth()
        {
            return "SELECT url,depth FROM spider_workload WHERE url_hash =  ?";
        }

        /// <summary>
        /// Get the source of a workload item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetSource()
        {
            return "SELECT w.url,s.url FROM spider_workload w,spider_workload s WHERE w.source_id =  s.workload_id AND w.url_hash =  ?";
        }

        /// <summary>
        /// Setup the workload for a resume.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlResume()
        {
            return "SELECT distinct host FROM spider_workload WHERE status =  'P'";
        }

        /// <summary>
        /// Setup the host table for a resume.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlResume2()
        {
            return "UPDATE spider_workload SET status =  'W' WHERE status =  'P'";
        }

        /// <summary>
        /// Get the ID of a workload item.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetWorkloadId()
        {
            return "SELECT workload_id,url FROM spider_workload WHERE url_hash =  ?";
        }

        /// <summary>
        /// Get the ID of a host.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetHostId()
        {
            return "SELECT host_id,host FROM spider_host WHERE host =  ?";
        }

        /// <summary>
        /// Get the next host to process.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetNextHost()
        {
            return "SELECT host_id,host FROM spider_host WHERE status =  ?";
        }

        /// <summary>
        /// Set the status of a host.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlSetHostStatus()
        {
            return "UPDATE spider_host SET status =  ? WHERE host_id =  ?";
        }

        /// <summary>
        /// Get the host that matches the specified ID.
        /// </summary>
        /// <returns>An Sql statement.</returns>
        public virtual String GetSqlGetHost()
        {
            return "SELECT host FROM spider_host WHERE host_id =  ?";
        }
    }
}