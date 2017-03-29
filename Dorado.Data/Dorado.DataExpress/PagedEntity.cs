using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado.DataExpress
{
    [DataContract(Name = "entities")]
    public class PagedEntity<T> where T : new()
    {
        [DataMember(Name = "descriptor", Order = 1)]
        public PageDescriptor Descriptor
        {
            get;
            set;
        }

        [DataMember(Order = 2, Name = "rows")]
        public List<T> Rows
        {
            get;
            set;
        }

        public PagedEntity()
        {
            this.Descriptor = new PageDescriptor();
        }
    }
}