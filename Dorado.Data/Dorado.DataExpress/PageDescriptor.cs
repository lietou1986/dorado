using System.Runtime.Serialization;

namespace Dorado.DataExpress
{
    [DataContract(Name = "pageInfo")]
    public class PageDescriptor
    {
        [DataMember(Name = "total", Order = 0)]
        public long Total
        {
            get;
            set;
        }

        [DataMember(Name = "pageSize", Order = 1)]
        public int PageSize
        {
            get;
            set;
        }

        [DataMember(Name = "start", Order = 2)]
        public long Start
        {
            get;
            set;
        }

        [DataMember(Name = "limit", Order = 3)]
        public long Limit
        {
            get;
            set;
        }

        [DataMember(Name = "totalPage", Order = 4)]
        public int TotalPage
        {
            get;
            set;
        }

        [DataMember(Name = "currentPage", Order = 5)]
        public int CurrentPage
        {
            get;
            set;
        }
    }
}