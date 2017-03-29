using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Extensions.Behaviors
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class PoolingAttribute : Attribute, IServiceBehavior
    {
        private const int defaultMaxPoolSize = 32;
        private const int defaultMinPoolSize = 0;
        private int maxPoolSize = defaultMaxPoolSize;
        private int minPoolSize = defaultMinPoolSize;

        private ServiceThrottlingBehavior throttlingBehavior = null;

        public int MaxPoolSize
        {
            get { return maxPoolSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ResourceHelper.GetString("ExNegativePoolSize"));
                }

                this.maxPoolSize = value;
            }
        }

        public int MinPoolSize
        {
            get { return minPoolSize; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(ResourceHelper.GetString("ExNegativePoolSize"));
                }

                this.minPoolSize = value;
            }
        }

        #region IServiceBehavior Members

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
            if (this.throttlingBehavior != null)
            {
                ((IServiceBehavior)this.throttlingBehavior).AddBindingParameters(description, serviceHostBase, endpoints, parameters);
            }
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            PoolingInstanceProvider instanceProvider = new PoolingInstanceProvider(description.ServiceType,
                                                                                               minPoolSize);

            if (this.throttlingBehavior != null)
            {
                ((IServiceBehavior)this.throttlingBehavior).ApplyDispatchBehavior(description, serviceHostBase);
            }

            ServiceThrottle throttle = null;

            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    if ((this.throttlingBehavior == null) && (this.maxPoolSize != Int32.MaxValue))
                    {
                        if (throttle == null)
                        {
                            throttle = cd.ServiceThrottle;
                        }
                        if (cd.ServiceThrottle == null)
                        {
                            throw new InvalidOperationException(ResourceHelper.GetString("ExNullThrottle"));
                        }
                        if (throttle != cd.ServiceThrottle)
                        {
                            throw new InvalidOperationException(ResourceHelper.GetString("ExDifferentThrottle"));
                        }
                    }

                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = instanceProvider;
                    }
                }
            }

            if ((throttle != null) && (throttle.MaxConcurrentInstances > this.maxPoolSize))
            {
                throttle.MaxConcurrentInstances = this.maxPoolSize;
            }
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            if (this.maxPoolSize < this.minPoolSize)
            {
                throw new InvalidOperationException(ResourceHelper.GetString("ExMinLargerThanMax"));
            }

            ServiceBehaviorAttribute serviceBehavior = description.Behaviors.Find<ServiceBehaviorAttribute>();

            if (serviceBehavior != null &&
                serviceBehavior.InstanceContextMode == InstanceContextMode.Single)
            {
                throw new InvalidOperationException(ResourceHelper.GetString("ExInvalidContext"));
            }

            int throttlingIndex = this.GetBehaviorIndex(description, typeof(ServiceThrottlingBehavior));
            if (throttlingIndex == -1)
            {
                this.throttlingBehavior = new ServiceThrottlingBehavior();
                this.throttlingBehavior.MaxConcurrentInstances = this.MaxPoolSize;

                ((IServiceBehavior)this.throttlingBehavior).Validate(description, serviceHostBase);
            }
            else
            {
                int poolingIndex = this.GetBehaviorIndex(description, typeof(PoolingAttribute));
                if (poolingIndex < throttlingIndex)
                {
                    throw new InvalidOperationException(ResourceHelper.GetString("ExThrottleBeforePool"));
                }
            }
        }

        #endregion IServiceBehavior Members

        private int GetBehaviorIndex(ServiceDescription description, Type behaviorType)
        {
            for (int i = 0; i < description.Behaviors.Count; i++)
            {
                if (behaviorType.IsInstanceOfType(description.Behaviors[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }

    #region PoolingBehaviorSection

    /// <summary>
    /// Configuration class
    /// </summary>
    public class PoolingBehaviorSection : BehaviorExtensionElement
    {
        #region ConfigurationProperties

        [ConfigurationProperty("minPoolSize", DefaultValue = 0)]
        public int MinPoolSize
        {
            get { return (int)this["minPoolSize"]; }
            set { this["minPoolSize"] = value; }
        }

        [ConfigurationProperty("maxPoolSize", DefaultValue = 32)]
        public int MaxPoolSize
        {
            get { return (int)this["maxPoolSize"]; }
            set { this["maxPoolSize"] = value; }
        }

        #endregion ConfigurationProperties

        #region BehaviorExtensionSection

        protected override object CreateBehavior()
        {
            PoolingAttribute pooling = new PoolingAttribute();
            pooling.MinPoolSize = this.MinPoolSize;
            pooling.MaxPoolSize = this.MaxPoolSize;
            return pooling;
        }

        #endregion BehaviorExtensionSection

        public override Type BehaviorType
        {
            get { return typeof(PoolingAttribute); }
        }
    }

    #endregion PoolingBehaviorSection
}