using System;
using System.Data;
using Dorado.VWS.Model.Enum;

namespace Dorado.VWS.Model
{
    /// <summary>
    /// 文件监听实体
    /// </summary>
    [Serializable]
    public class FilesWatcherEntity : EntityBase<FilesWatcherEntity>, IConvertToEntity<FilesWatcherEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     操作类型
        /// </summary>
        public EnumFWOperateType OperateType { get; set; }

        /// <summary>
        ///     域名Id
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        ///     同步需要增加或修改文件集合
        /// </summary>
        public string AddFiles { get; set; }

        /// <summary>
        ///     同步需要增加或修改文件集合
        /// </summary>
        public string UpdateFiles { get; set; }

        /// <summary>
        ///     同步需要删除文件集合
        /// </summary>
        public string DelFiles { get; set; }

        /// <summary>
        ///     同步操作者用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     操作者Ip
        /// </summary>
        public string OperatorIp { get; set; }

        /// <summary>
        ///     记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     日志信息
        /// </summary>
        public string LogInfo { get; set; }

        /// <summary>
        ///  文件监听描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public FilesWatcherEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var fsEntity = new FilesWatcherEntity
                {
                    Id = Convert.ToInt32(row["Id"]),
                    OperateType =
                        (EnumFWOperateType)Convert.ToInt32(row["OperateType"]),
                    ServerId = Convert.ToInt32(row["ServerId"]),
                    Root = row["Root"].ToString(),
                    AddFiles = row["AddFiles"].ToString(),
                    UpdateFiles = row["UpdateFiles"].ToString(),
                    DelFiles = row["DelFiles"].ToString(),
                    UserName = row["UserName"].ToString(),
                    OperatorIp = row["OperatorIp"].ToString(),
                    CreateTime = Convert.ToDateTime(row["CreateTime"]),
                    UpdateTime = Convert.ToDateTime(row["UpdateTime"]),
                    LogInfo = row["LogInfo"].ToString(),
                    Description = row["Description"].ToString(),
                    DeleteFlag = Convert.ToBoolean(row["DeleteFlag"]),
                    Remark = row["Remark"].ToString(),
                };

                if (row["AddFiles"] != DBNull.Value)
                {
                    fsEntity.AddFiles = row["AddFiles"].ToString();
                }
                if (row["UpdateFiles"] != DBNull.Value)
                {
                    fsEntity.UpdateFiles = row["UpdateFiles"].ToString();
                }
                if (row["DelFiles"] != DBNull.Value)
                {
                    fsEntity.DelFiles = row["DelFiles"].ToString();
                }
                if (row["OperatorIp"] != DBNull.Value)
                {
                    fsEntity.OperatorIp = row["OperatorIp"].ToString();
                }
                if (row["CreateTime"] != DBNull.Value)
                {
                    fsEntity.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                }
                if (row["UpdateTime"] != DBNull.Value)
                {
                    fsEntity.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                }
                if (row["LogInfo"] != DBNull.Value)
                {
                    fsEntity.LogInfo = row["LogInfo"].ToString();
                }
                if (row["Description"] != DBNull.Value)
                {
                    fsEntity.Description = row["Description"].ToString();
                }
                if (row["DeleteFlag"] != DBNull.Value)
                {
                    fsEntity.DeleteFlag = Convert.ToBoolean(row["DeleteFlag"]);
                }
                if (row["Remark"] != DBNull.Value)
                {
                    fsEntity.Remark = row["Remark"].ToString();
                }
                return fsEntity;
            }
            return null;
        }
    }
}