using System.Data;

namespace Dorado.DataExpress
{
    public class PagedDataTable
    {
        public PageDescriptor Descriptor
        {
            get;
            set;
        }

        public DataTable Data
        {
            get;
            set;
        }

        public PagedDataTable()
        {
            this.Descriptor = new PageDescriptor();
        }
    }
}