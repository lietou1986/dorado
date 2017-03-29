using System;

namespace Dorado.Core.FastData.DataType
{
    [Serializable]
    public class TreeNameNode
    {
        public object Data;
        public int Hash;
        public TreeNameNode Left;
        public TreeNameNode Right;

        public TreeNameNode(string name, object d)
        {
            Data = d;
            Hash = name.GetHashCode();
            Left = null;
            Right = null;
        }
    }
}