using Dorado.ESB.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Dorado.ESB.Core
{
    #region DomainLoader

    public class DomainLoader : IDomainLoader
    {
        /// <summary>
        /// 卸载Domain
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="appDomainHostName"></param>
        public void UnloadDomain(string hostName, string appDomainHostName)
        {
            // validation
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("hostName");
            if (string.IsNullOrEmpty(appDomainHostName) || appDomainHostName == "*" || appDomainHostName == AppDomain.CurrentDomain.FriendlyName)
                throw new ArithmeticException("This operation is not valid for default domain");

            appDomainHostName = appDomainHostName.Trim();
            hostName = hostName.Trim();

            if (!string.IsNullOrEmpty(HostServices.Current.HostName) && HostServices.Current.HostName == hostName)
            {
                HostServices.Current.Abort(appDomainHostName, true);
            }
            else
            {
                throw new ArgumentException(string.Format("The '{0}' is invalid hostName", hostName));
            }
        }

        /// <summary>
        /// 调入Domain
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="appDomainHostName"></param>
        /// <param name="metadata"></param>
        public void LoadDomain(string hostName, string appDomainHostName, HostMetadata metadata)
        {
            // validation
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("hostName");
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            if (string.IsNullOrEmpty(appDomainHostName) || appDomainHostName == "*" || appDomainHostName == AppDomain.CurrentDomain.FriendlyName)
                throw new ArithmeticException("This operation is not valid for default domain");

            appDomainHostName = appDomainHostName.Trim();
            hostName = hostName.Trim();

            if (!string.IsNullOrEmpty(HostServices.Current.HostName) && HostServices.Current.HostName == hostName)
            {
                HostServices.Current.Load(appDomainHostName, metadata);
            }
            else
            {
                throw new ArgumentException(string.Format("The '{0}' is invalid hostName", hostName));
            }
        }

        /// <summary>
        /// 重新调入 Domain
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="appDomainHostName"></param>
        /// <param name="metadata"></param>
        public void ReloadDomain(string hostName, string appDomainHostName, HostMetadata metadata)
        {
            // validation
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("hostName");
            if (metadata == null)
                throw new ArgumentNullException("metadata");
            if (string.IsNullOrEmpty(appDomainHostName) || appDomainHostName == "*" || appDomainHostName == AppDomain.CurrentDomain.FriendlyName)
                throw new ArithmeticException("This operation is not valid for default domain");

            appDomainHostName = appDomainHostName.Trim();
            hostName = hostName.Trim();

            if (!string.IsNullOrEmpty(HostServices.Current.HostName) && HostServices.Current.HostName == hostName)
            {
                HostServices.Current.Reload(appDomainHostName, metadata);
            }
            else
            {
                throw new ArgumentException(string.Format("The '{0}' is invalid hostName", hostName));
            }
        }

        /// <summary>
        /// 得到当前host的service
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public List<ServiceMetadataBase> GetHostedServices(string hostName)
        {
            // validation
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentNullException("hostName");

            hostName = hostName.Trim();

            if (!string.IsNullOrEmpty(HostServices.Current.HostName) && HostServices.Current.HostName == hostName)
            {
                return HostServices.Current.GetHostedServices;
            }
            else
            {
                throw new ArgumentException(string.Format("The '{0}' is invalid hostName", hostName));
            }
        }
    }

    #endregion DomainLoader

    #region VirtualService Extensions (caching, etc.)

    /// <summary>
    /// Storage (cache) of the configData
    /// </summary>
    public sealed class ServiceConfigDataService : IExtension<ServiceHostBase>
    {
        private ServiceConfigData _data = null;

        public ServiceConfigData ServiceConfigData { get { return _data; } }

        public ServiceConfigDataService(ServiceConfigData data)
        {
            _data = data;
        }

        public void Attach(ServiceHostBase owner)
        {
        }

        public void Detach(ServiceHostBase owner)
        {
        }
    }

    #endregion VirtualService Extensions (caching, etc.)
}