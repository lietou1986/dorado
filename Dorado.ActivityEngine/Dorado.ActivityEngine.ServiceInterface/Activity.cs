using Dorado.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado.ActivityEngine.ServiceInterface
{
    [DataContract]
    public class Activity
    {
        [DataMember(IsRequired = true)]
        public int ActivityType
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public int TenantId
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public DateTime Timestamp
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, string> Resources
        {
            get;
            private set;
        }

        public Activity()
        {
            this.Resources = new Dictionary<string, string>();
        }

        public Activity(int activityType, int tenantId)
        {
            Guard.ArgumentPositive(activityType);
            Guard.ArgumentPositive(tenantId);
            this.ActivityType = activityType;
            this.TenantId = tenantId;
            this.Timestamp = DateTime.Now;
            this.Resources = new Dictionary<string, string>();
        }
    }
}