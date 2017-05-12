using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vancl.IC.VWS.Contract;

namespace Vancl.IC.VWS.BLL
{
    public static class MessageProcessor
    {
        private static readonly MessageManager MsgMgr = MessageManager.GetInstance();

        public delegate void FileListCallbackHandler(int taskid, VwsDirectoryInfoList list);
        public static event FileListCallbackHandler FileListEvent;
        static MessageProcessor()
        {
            MsgMgr.Start();
            MsgMgr.RecievedEvent += RecievedMsg;
        }

        private static void RecievedMsg(Operator opt, VwsMessage msg)
        {
            if (VwsMessageType.Res.Equals(msg))
            {
                if ("getfilelist".Equals(msg.Name) && FileListEvent != null)
                {
                    FileListEvent(msg.TaskId, msg.FileList);
                } 
            }
        }

        public static bool SendMessage(string ip, VwsMessage msg)
        {
            Operator opt = GetOperator(ip);

            if (opt != null)
            {
                MsgMgr.SendMessageOne(opt, msg);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     获取Ip指定的operator
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Operator GetOperator(string ip)
        {
            Operator[] opts = MsgMgr.GetAllOpts();
            foreach (Operator opt in opts)
            {
                if (ip.Equals(opt.IP))
                {
                    return opt;
                }
            }
            return null;
        }

        public static IList<Operator> GetAllOperators()
        {
            return MsgMgr.GetAllOpts();
        }

        public static void Stop()
        {
            MsgMgr.Stop();
        }
    }
}
