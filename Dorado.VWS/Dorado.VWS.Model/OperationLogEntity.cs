/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 11:46:20
 * 版本号：v1.0
 * 本类主要用途描述：操作日志实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     操作日志实体
    /// </summary>
    [Serializable]
    public class OperationLogEntity : EntityBase<OperationLogEntity>, IConvertToEntity<OperationLogEntity>
    {
        /// <summary>
        ///     操作日志Id
        /// </summary>
        public int OperationLogId { get; set; }

        /// <summary>
        ///     操作者用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     域名
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     操作类型
        /// </summary>
        public EnumOperateType OperateType { get; set; }

        /// <summary>
        ///     操作类型名称
        /// </summary>
        public string OperateTypeName
        {
            get { return OperateType.GetDescription(); }
        }

        /// <summary>
        ///     详细日志信息
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        ///     结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        #region IConvertToEntity<OperationLogEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public OperationLogEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var operationLogEntity = new OperationLogEntity
                                             {
                                                 OperationLogId = Convert.ToInt32(row["Id"]),
                                                 UserName = row["UserName"].ToString(),
                                                 OperateType =
                                                     (EnumOperateType)
                                                     Convert.ToInt32(row["OperateType"]),
                                                 DomainName = row["DomainName"].ToString(),
                                                 Result = Convert.ToBoolean(row["Result"]),
                                                 Log = row["Log"].ToString(),
                                                 CreateTime = Convert.ToDateTime(row["CreateTime"])
                                             };

                return operationLogEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<OperationLogEntity> Members
    }
}