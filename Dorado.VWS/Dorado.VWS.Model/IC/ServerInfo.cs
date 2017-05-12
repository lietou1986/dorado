#region using

using System;

#endregion using

namespace Dorado.VWS.Model.IC
{
    [Serializable]
    public class ServerInfo
    {
        public string IP { get; set; }

        public string ClientVersion { get; set; }

        public int IISStatus { get; set; }

        public string HostName { get; set; }
    }
}