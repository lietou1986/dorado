using Dorado.Core.Data;
using System.Collections.Generic;

namespace Dorado.Web.Filters
{
    internal class ConnContainer : Dictionary<string, Conn>
    {
        public const string ContainerName = "ConnContainer";
    }
}