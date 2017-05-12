using System;

namespace Dorado.VWS.Model
{
    public class FileMD5Entity : EntityBase<FileMD5Entity>, IConvertToEntity<FileMD5Entity>
    {
        public int DomainId { get; set; }

        public string FilePath { get; set; }

        public string MD5 { get; set; }

        public DateTime CreateTime { get; set; }

        public FileMD5Entity ConvertToEntity(System.Data.DataRow row)
        {
            if (row == null)
            {
                return null;
            }

            var fileMd5Entity = new FileMD5Entity
                                    {
                                        DomainId = Convert.ToInt32(row["DomainId"]),
                                        FilePath = row["FilePath"].ToString(),
                                        MD5 = row["MD5"].ToString(),
                                        CreateTime = Convert.ToDateTime(row["CreateTime"])
                                    };
            return fileMd5Entity;
        }
    }
}