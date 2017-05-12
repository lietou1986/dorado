using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;
using Vancl.IC.VWS.Contract;
using Vancl.IC.VWS.MessageClient;

namespace Vancl.IC.VWS.BLL
{
    public class MessageManager
    {
        private ILog _logger = LogManager.GetLogger(typeof(MessageManager));
        private Operator _operator;
        private readonly DispatchSingleton _singleton = DispatchSingleton.GetInstance();
        private readonly Dictionary<string, Operator> _dicOpts = new Dictionary<string, Operator>();

        public delegate void RecievedEventHandler(Operator opt, VwsMessage msg);
        public RecievedEventHandler RecievedEvent;

        public delegate void FileListCallbackHandler(int taskid, VwsDirectoryInfoList list);
        public FileListCallbackHandler FileListEvent;

        private static MessageManager _my = new MessageManager();
        private MessageManager() { }

        public static MessageManager GetInstance()
        {
            return _my ?? new MessageManager();
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public void Start()
        {
            //清空Opts列表
            _dicOpts.Clear();
            //创建自己的Operator
            CreateOperator();

            //连接服务
            _singleton.Connect(_operator);
            //添加Event处理
            _singleton.DispatchEvent += AfterConnect;
            _singleton.DispatchCallBackEvent += DispatchCallBack;
            //开始心跳
            _singleton.ClientTest();
        }

        /// <summary>
        /// 退出连接
        /// </summary>
        public void Stop()
        {
            //退出服务
            _singleton.ExitChatSession();
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="msg"></param>
        public void SendMessageOne(Operator opt, VwsMessage msg)
        {
            _singleton.SayAndClear(opt.IP, msg, true);
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="msg"></param>
        public void SendMessageOne(string ip, VwsMessage msg)
        {
            Operator opt = GetOperator(ip);
            if (opt != null)
            {
                _singleton.SayAndClear(opt.IP, msg, true); 
            }
        }

        #region 连接结束后，重构当前Opt集合
        /// <summary>
        /// 连接结束后，重构当前Opt集合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AfterConnect(object sender, DispatchEventArgs e)
        {
            _dicOpts.Clear();
            foreach (Operator opt in e.List)
            {
                _dicOpts.Add(opt.IP, opt);
                _logger.Debug(opt.IP);
            }

            _logger.InfoFormat("Connect at {0} with operator[{1}]", DateTime.Now, _operator.IP);
        }
        #endregion

        /// <summary>
        /// 创建当前客户端的Operator
        /// </summary>
        private void CreateOperator()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostname);

            string strIp = string.Empty;
            foreach (IPAddress item in ipEntry.AddressList)
            {
                if (!item.IsIPv6LinkLocal)
                {
                    strIp = item.ToString() + "web";
                }
            }

            string strPcName = ipEntry.HostName;

            string fileversion = AssemblyHelper.GetProductVersion();

            _operator = new Operator()
            {
                IP = strIp,
                ComputerName = strPcName,
                ClientVer = fileversion
            };

            _logger.InfoFormat("Create operator1: IP[{0}],ComputerName[{1}]", _operator.IP, _operator.ComputerName);
        }

        #region 从服务端收到消息并处理
        /// <summary>
        /// 从服务端收到消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DispatchCallBack(object sender, DispatchCallBackEventArgs e)
        {
            _logger.Debug("test call");
            switch (e.CallbackType)
            {
                case CallBackType.Receive:
                case CallBackType.ReceiveWhisper:
                    _logger.Info("received from " + e.Opt.IP +  "msg:" + e.Message.Name.ToString());
                    if (VwsMessageType.Res.Equals(e.Message.MsgType))
                    {
                        if ("getfilelist".Equals(e.Message.Name) && FileListEvent != null)
                        {
                            FileListEvent(e.Message.TaskId, e.Message.FileList);
                        }
                        else if("syncfiles".Equals(e.Message.Name))
                        {
                            //处理同步文件的返回
                            SyncTaskProcessor syncTaskProc = new SyncTaskProcessor();
                            syncTaskProc.CallbackSyncTask(e.Opt, e.Message);
                        }
                    }
                    break;
                case CallBackType.UserEnter:
                    UserEnter(e.Opt);
                    break;
                case CallBackType.UserLeave:
                    UserLeave(e.Opt);
                    break;
            }
        }

        /// <summary>
        /// 客户端离开
        /// </summary>
        /// <param name="p"></param>
        private void UserLeave(Operator p)
        {
            _logger.DebugFormat("UserLeave {0}", p.IP);
            if (_dicOpts.ContainsKey(p.IP))
            {
                _dicOpts.Remove(p.IP);
            }
        }

        /// <summary>
        /// 客户端进入
        /// </summary>
        /// <param name="p"></param>
        private void UserEnter(Operator p)
        {
            _logger.DebugFormat("UserEnter {0}", p.IP);
            if (_dicOpts.ContainsKey(p.IP))
            {
                _dicOpts.Remove(p.IP);
            }
            _dicOpts.Add(p.IP, p);
        }
        #endregion

        public Operator[] GetAllOpts()
        {
            Operator[] list = new Operator[_dicOpts.Count];
            _dicOpts.Values.CopyTo(list, 0);

            return list;
        }

        /// <summary>
        ///     获取Ip指定的operator
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Operator GetOperator(string ip)
        {
            foreach (Operator opt in GetAllOpts())
            {
                if (ip.Equals(opt.IP))
                {
                    return opt;
                }
            }
            return null;
        }
    }
}
