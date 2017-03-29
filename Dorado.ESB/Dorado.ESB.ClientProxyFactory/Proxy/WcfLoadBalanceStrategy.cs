using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public abstract class StrategySuper
    {
        public abstract EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress);
    }

    public class StrategyContext
    {
        private StrategySuper strategySuper = null;

        public StrategyContext(string strategy)
        {
            switch (strategy.ToLower())
            {
                case "random":
                    RandomStrategy randomStrategy = new RandomStrategy();
                    strategySuper = randomStrategy;
                    break;

                case "mindelay":
                    MinDelayStrategy minDelayStrategy = new MinDelayStrategy();
                    strategySuper = minDelayStrategy;
                    break;

                case "leastconnections":
                    LeastConnectionsStrategy leastConnectionsStrategy = new LeastConnectionsStrategy();
                    strategySuper = leastConnectionsStrategy;
                    break;

                case "weightedrandom":
                    WeightedRandomStrategy weightedRandomStrategy = new WeightedRandomStrategy();
                    strategySuper = weightedRandomStrategy;
                    break;

                case "weighted":
                    WeightedStrategy weighted = new WeightedStrategy();
                    strategySuper = weighted;
                    break;
            }
        }

        public EndpointAddress GetResult(ServiceStatus serviceStatus, string notContainAddress)
        {
            return strategySuper.SelectAddress(serviceStatus, notContainAddress);
        }
    }

    public class RandomStrategy : StrategySuper
    {
        private EndpointAddress endpointAddress = null;

        public RandomStrategy()
        {
        }

        public override EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress)
        {
            try
            {
                List<HeartbeatStatus> listHeartbeatStatus = serviceStatus.ListHeartbeatStatus;
                Random random = new Random(DateTime.Now.Millisecond);
                HeartbeatStatus heartbeatStatus;
                if (string.IsNullOrEmpty(notContainAddress))
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => random.Next()).FirstOrDefault(c => c.Pulsate == true);
                else
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => random.Next()).Except(listHeartbeatStatus.Where(c => c.Address == notContainAddress)).FirstOrDefault(c => c.Pulsate == true);

                if (heartbeatStatus != null)
                {
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                }
                else
                {
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => random.Next()).FirstOrDefault();
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                    LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory",
                        new Exception("A serious warning, all the wcf server error， re-create the connection auto")
                         );
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
            return endpointAddress;
        }
    }

    public class MinDelayStrategy : StrategySuper
    {
        private EndpointAddress endpointAddress = null;

        public MinDelayStrategy()
        {
        }

        public override EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress)
        {
            try
            {
                List<HeartbeatStatus> listHeartbeatStatus = serviceStatus.ListHeartbeatStatus;
                HeartbeatStatus heartbeatStatus;
                if (string.IsNullOrEmpty(notContainAddress))
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.Delay).FirstOrDefault(c => c.Pulsate == true);
                else
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.Delay).Except(listHeartbeatStatus.Where(c => c.Address == notContainAddress)).FirstOrDefault(c => c.Pulsate == true);

                if (heartbeatStatus != null)
                {
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                }
                else
                {
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.Delay).FirstOrDefault();
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                    LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory",
                   new Exception("A serious warning, all the wcf server error， re-create the connection auto")
                   );
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
            return endpointAddress;
        }
    }

    public class LeastConnectionsStrategy : StrategySuper
    {
        private EndpointAddress endpointAddress = null;

        public LeastConnectionsStrategy()
        {
        }

        public override EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress)
        {
            try
            {
                List<HeartbeatStatus> listHeartbeatStatus = serviceStatus.ListHeartbeatStatus;
                HeartbeatStatus heartbeatStatus;
                if (string.IsNullOrEmpty(notContainAddress))
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.ConnectionsNumber).FirstOrDefault(c => c.Pulsate == true);
                else
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.ConnectionsNumber).Except(listHeartbeatStatus.Where(c => c.Address == notContainAddress)).FirstOrDefault(c => c.Pulsate == true);

                if (heartbeatStatus != null)
                {
                    heartbeatStatus.ConnectionsNumber++;
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                }
                else
                {
                    heartbeatStatus = listHeartbeatStatus.OrderBy(c => c.ConnectionsNumber).FirstOrDefault();
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                    LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory",
                   new Exception("A serious warning, all the wcf server error， re-create the connection auto")
                   );
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
            return endpointAddress;
        }
    }

    public class WeightedRandomStrategy : StrategySuper
    {
        private EndpointAddress endpointAddress = null;

        public WeightedRandomStrategy()
        {
        }

        private Random random = new Random(DateTime.Now.Millisecond);

        private string GetServer(List<HeartbeatStatus> listHeartbeatStatus, double totalWeight)
        {
            int randomNumber = random.Next(0, (int)totalWeight);
            string address = string.Empty;
            foreach (HeartbeatStatus heartbeatStatus in listHeartbeatStatus)
            {
                if (randomNumber < heartbeatStatus.Weight)
                {
                    address = heartbeatStatus.Address;
                    break;
                }
                randomNumber = randomNumber - (int)heartbeatStatus.Weight;
            }
            return address;
        }

        public override EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress)
        {
            try
            {
                List<HeartbeatStatus> listHeartbeatStatus = serviceStatus.ListHeartbeatStatus;
                string address = string.Empty;
                if (string.IsNullOrEmpty(notContainAddress))
                {
                    address = GetServer(listHeartbeatStatus.Where(c => c.Pulsate == true).ToList(), listHeartbeatStatus.Sum(c => c.Weight));
                }
                else
                {
                    IEnumerable<HeartbeatStatus> ienum = listHeartbeatStatus.Where(c => c.Address == notContainAddress);
                    address = GetServer(listHeartbeatStatus.Where(c => c.Pulsate == true).Except(ienum).ToList(), listHeartbeatStatus.Except(ienum).Sum(c => c.Weight));
                }

                if (!string.IsNullOrEmpty(address))
                {
                    endpointAddress = new EndpointAddress(address);
                }
                else
                {
                    address = GetServer(listHeartbeatStatus, listHeartbeatStatus.Sum(c => c.Weight));
                    endpointAddress = new EndpointAddress(address);
                    LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory",
                   new Exception("A serious warning, all the wcf server error， re-create the connection auto")
                   );
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
            return endpointAddress;
        }
    }

    public class WeightedStrategy : StrategySuper
    {
        private EndpointAddress endpointAddress = null;

        public WeightedStrategy()
        {
        }

        private Random random = new Random(DateTime.Now.Millisecond);

        private HeartbeatStatus GetServer(List<HeartbeatStatus> listHeartbeatStatus)
        {
            HeartbeatStatus heartbeatStatus;
            listHeartbeatStatus.ForEach(c => c.CurrentWeight = Math.Round((double)c.ConnectionsNumber / (double)listHeartbeatStatus.Sum(x => x.ConnectionsNumber), 5) * 100);
            heartbeatStatus = listHeartbeatStatus.Where(c => c.CurrentWeight < c.Weight).OrderBy(d => random.Next()).FirstOrDefault();
            if (heartbeatStatus == null)
            {
                heartbeatStatus = listHeartbeatStatus.Where(c => c.CurrentWeight == c.Weight).OrderBy(d => random.Next()).FirstOrDefault();
                if (heartbeatStatus == null)
                {
                    heartbeatStatus = listHeartbeatStatus.Where(c => c.CurrentWeight > c.Weight).OrderBy(d => random.Next()).FirstOrDefault();
                }
            }
            heartbeatStatus.ConnectionsNumber += 1;
            return heartbeatStatus;
        }

        public override EndpointAddress SelectAddress(ServiceStatus serviceStatus, string notContainAddress)
        {
            try
            {
                List<HeartbeatStatus> listHeartbeatStatus = serviceStatus.ListHeartbeatStatus;
                HeartbeatStatus heartbeatStatus;
                if (string.IsNullOrEmpty(notContainAddress))
                {
                    heartbeatStatus = GetServer(listHeartbeatStatus.Where(c => c.Pulsate == true).ToList());
                }
                else
                {
                    IEnumerable<HeartbeatStatus> ienum = listHeartbeatStatus.Where(c => c.Address == notContainAddress);
                    heartbeatStatus = GetServer(listHeartbeatStatus.Where(c => c.Pulsate == true).Except(ienum).ToList());
                }

                if (heartbeatStatus != null)
                {
                    endpointAddress = new EndpointAddress(heartbeatStatus.Address);
                }
                else
                {
                    endpointAddress = new EndpointAddress(GetServer(listHeartbeatStatus).Address);
                    LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory",
                   new Exception("A serious warning, all the wcf server error， re-create the connection auto")
                  );
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ClientProxyFactory", ex);
            }
            return endpointAddress;
        }
    }
}