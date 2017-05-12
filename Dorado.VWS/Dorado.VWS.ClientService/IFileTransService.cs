/*-------------------------------------------------------------------------
 * 版权所有：凡客诚品（北京）科技有限公司
 * 版本：v1.0
 * 时间： 2011/7/29 10:59:54
 * 作者：蔡昌艳（Bruce Tscai）
 * 联系方式：caichangyan@vancl.cn
 * 本类主要用途描述：文件传输类
 *  -------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Vancl.IC.VWS.ClientService.Model;


namespace Vancl.IC.VWS.ClientService
{
    /// <summary>
    /// 文件传输类
    /// </summary>
    [ServiceContract]
    public interface IFileTransService
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="md5">md5文件验证</param>
        /// <returns>文件byte数组</returns>
        [OperationContract]
        byte[] Download(string filePath, out string md5);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileBytes">文件byte数组</param>
        /// <param name="md5">md5文件验证</param>
        /// <returns>成功返回True;失败返回false</returns>
        [OperationContract]
        bool Upload(byte[] fileBytes, string md5);

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="folderPath">文件夹地址</param>
        /// <returns></returns>
        [OperationContract]
        VwsDirectoryInfoList GetFileList(string folderPath);
    }
}
