using System;

namespace Dorado.Web.Fileset
{
    internal class LocalJsCacheItem
    {
        internal DateTime LastModifyDate
        {
            get;
            set;
        }

        internal string FileContent
        {
            get;
            set;
        }
    }
}