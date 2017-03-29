using System.Collections.Generic;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public class ServiceStatus
    {
        public string ServiceName { get; set; }

        public List<HeartbeatStatus> ListHeartbeatStatus = new List<HeartbeatStatus>();
    }

    public class HeartbeatStatus
    {
        public string Address { get; set; }

        public string ServiceInterface { get; set; }

        public bool Pulsate { get; set; }

        public double Delay { get; set; }

        public double Weight { get; set; }

        public double CurrentWeight { get; set; }

        public long ConnectionsNumber { get; set; }
    }
}