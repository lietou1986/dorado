using System.Collections.Generic;

namespace Dorado.Core.Collection
{
    /// <summary>
    /// 忽略字符串大小写的HashSet
    /// </summary>
    public class IgnoreCaseHashSet : HashSet<string>
    {
        public IgnoreCaseHashSet()
            : base(IgnoreCaseEqualityComparer.Instance)
        {
        }
    }
}