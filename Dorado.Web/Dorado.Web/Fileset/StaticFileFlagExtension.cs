using Dorado.Web.Fileset;
using System.Linq;

namespace Dorado.Web.Fileset
{
    public static class StaticFileFlagExtension
    {
        public static bool Contains(this StaticFileFlag thisFlag, params StaticFileFlag[] flags)
        {
            return flags.All((StaticFileFlag flag) => (thisFlag & flag) == flag);
        }
    }
}