using System;

namespace Dorado.Spider.Workload.Sql.Oracle
{
    /// <summary>
    /// OracleHolder: Contains Sql statements customized to Oracle.
    /// </summary>
    internal class OracleHolder : SqlHolder
    {
        public override String GetSqlAdd()
        {
            return "INSERT INTO spider_workload(workload_id,host,url,status,depth,url_hash,source_id) VALUES(spider_workload_seq.NEXTVAL,?,?,?,?,?,?)";
        }

        public override String GetSqlAdd2()
        {
            return "INSERT INTO spider_host(host_id,host,status,urls_done,urls_error) VALUES(spider_host_seq.NEXTVAL,?,?,?,?)";
        }
    }
}