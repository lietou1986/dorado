using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

namespace Dorado.VWS.Services.Persistence
{
    public class FileMD5Dao : DBbase<FileMD5Entity>
    {
        public IList<FileMD5Entity> GetByDomainId(int domainId)
        {
            const string sql =
                @"SELECT * FROM zsync_filemd5(NOLOCK) WHERE DomainId = @DomainId and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public bool Save(int domainId, string filePath, string md5)
        {
            var tmp = GetByDomainIdAndPath(domainId, filePath);
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@FilePath", filePath),
                                            new SqlParameter("@MD5", md5)
                                        };
            if (tmp == null)
            {
                const string sql =
               @"INSERT INTO zsync_filemd5(DomainId, FilePath, MD5) VALUES
                          (@DomainId, @FilePath, @MD5)";

                ExecuteScalar(CommandType.Text, sql, parameters);
                return true;
            }

            const string sqlupdate =
               @"UPDATE zsync_filemd5 SET MD5=@MD5 where DomainId = @DomainId and FilePath=@FilePath";

            var result = ExecuteNonQuery(CommandType.Text, sqlupdate, parameters);
            return result == 1;
        }

        public FileMD5Entity GetByDomainIdAndPath(int domainId, string path)
        {
            const string sql =
               @"SELECT * FROM zsync_filemd5(NOLOCK) WHERE DomainId = @DomainId and FilePath=@FilePath and DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@FilePath", path)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }
    }
}