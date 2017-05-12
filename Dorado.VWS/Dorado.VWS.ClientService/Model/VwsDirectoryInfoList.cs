/*-------------------------------------------------------------------------
 * 版权所有：凡客诚品（北京）科技有限公司
 * 版本：v1.0
 * 时间： 2011/7/29 11:28:02
 * 作者：蔡昌艳（Bruce Tscai）
 * 联系方式：caichangyan@vancl.cn
 * 本类主要用途描述：文件实体列表类
 *  -------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vancl.IC.VWS.ClientService.Model
{
    /// <summary>
    /// 文件实体列表类
    /// </summary>
    [CollectionDataContract]
    public class VwsDirectoryInfoList : List<VwsDirectoryInfo>
    {
        public override bool Equals(object obj)
        {
            if (obj is VwsDirectoryInfoList)
            {
                VwsDirectoryInfoList temp = (VwsDirectoryInfoList)obj;

                IList<string> orgin = new List<string>();
                foreach (VwsDirectoryInfo item in this)
                {
                    orgin.Add(item.ToString());
                }

                foreach (VwsDirectoryInfo item in temp)
                {
                    if (!orgin.Contains(item.ToString()))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
