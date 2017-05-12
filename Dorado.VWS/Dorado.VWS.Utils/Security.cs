using System;
using System.Collections.Generic;
using System.Linq;
using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.VWS.Utils
{
    public static class Security
    {
        private static DateTime _lastLoadConfigTime = DateTime.MinValue;

        private static readonly List<string> _allowIPList = new List<string>();

        /// <summary>
        /// 验证请求端IP
        /// </summary>
        /// <param name="requestIP"></param>
        /// <returns></returns>
        public static bool CheckRequestIp(string requestIP)
        {
            return true;
        }

        /// <summary>
        /// 验证要启动/停止的服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool CheckWindowsService(string serviceName)
        {
            LoadConfig();
            return true;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private static void LoadConfig()
        {
            if (DateTime.Now < _lastLoadConfigTime.AddMinutes(1))
            {
                return;
            }

            try
            {
                //TODO:临时加上的允许IP
                //_allowIPList.Add("192.168.7.162");
                //_allowIPList.Add("192.168.7.223");
                //_allowIPList.Add("192.168.7.211");
                _allowIPList.Add("172.17.1.220");
                _allowIPList.Add("172.17.1.221");
                //_allowIPList.Add("172.17.4.25");
                //_allowIPList.Add("172.17.4.26");

                #region 发布时取消注释

                string allowIp = AppSettingProvider.Get("_allowIP") ?? "空";

                if (string.IsNullOrEmpty(allowIp)) { LoggerWrapper.Logger.Error("VWS", "_allowIP配置节点不存在或value为空"); }
                else
                {
                    if (allowIp.Contains('|'))
                    {
                        string[] allowIpArr = allowIp.Split('|');
                        if (allowIpArr.Any())
                        {
                            foreach (string item in allowIpArr)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    _allowIPList.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        _allowIPList.Add(allowIp);
                    }
                }

                #endregion 发布时取消注释

                _lastLoadConfigTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex);
            }
        }
    }
}