using System;
using System.Collections;

namespace Dorado.Core.FastData.DataType
{
    internal class NodeChain
    {
        internal TreeNameNode Node;
        internal NodeChain Next;

        public NodeChain(TreeNameNode node)
        {
            Node = node;
            Next = null;
        }
    }

    [Serializable]
    public class TreeName : ICloneable, IEnumerator
    {
        private readonly TreeNameNode[] _items = new TreeNameNode[10];
        private int _count;
        private TreeNameNode[] _list;
        private int _cursor = -1;

        public int Count
        {
            get { return _count; }
        }

        public TreeNameNode Add(TreeNameNode node)
        {
            if (node == null) return null;
            int index = Math.Abs(node.Hash) % 10;

            TreeNameNode tmp = _items[index];
            if (tmp == null)
            {
                _items[index] = node;
            }
            else
            {
            Start_Compare:
                if (node.Hash < tmp.Hash)
                {
                    if (tmp.Left != null)
                    {
                        tmp = tmp.Left;
                        goto Start_Compare;
                    }
                    tmp.Left = node;
                }
                else if (node.Hash > tmp.Hash)
                {
                    if (tmp.Right != null)
                    {
                        tmp = tmp.Right;
                        goto Start_Compare;
                    }
                    tmp.Right = node;
                }
                else return null;
            }
            _count++;
            return node;
        }

        public bool Contains(string name)
        {
            return this[name] != null;
        }

        public TreeNameNode this[string name]
        {
            get
            {
                int hash = name.GetHashCode();
                int index = Math.Abs(hash) % 10;

                TreeNameNode node = _items[index];
                while (node != null)
                {
                    if (hash == node.Hash)
                    {
                        return node;
                    }
                    node = hash < node.Hash ? node.Left : node.Right;
                }
                return null;
            }
        }

        public TreeNameNode Delete(string name)
        {
            if (name == null) return null;
            int hash = name.GetHashCode();
            int index = Math.Abs(hash) % 10;

            TreeNameNode node = _items[index];
            TreeNameNode father = null;
            int stat = 0;
            while (node != null)
            {
                if (hash < node.Hash)
                {
                    father = node;
                    node = node.Left;
                    stat = 1;
                }
                else if (hash > node.Hash)
                {
                    father = node;
                    node = node.Right;
                    stat = 2;
                }
                else
                {
                    switch (stat)
                    {
                        case 0:
                            _items[index] = node.Left;
                            Add(node.Right);
                            break;

                        case 1:
                            father.Left = node.Left;
                            Add(node.Right);
                            break;

                        case 2:
                            father.Right = node.Right;
                            Add(node.Left);
                            break;
                    }
                    _count--;
                    node.Left = null;
                    node.Right = null;
                    return node;
                }
            }
            return node;
        }

        public TreeNameNode Rename(string old, string name)
        {
            TreeNameNode node = Delete(old);
            if (node != null)
            {
                node.Hash = name.GetHashCode();
                Add(node);
            }
            return node;
        }

        #region ICloneable 成员

        public object Clone()
        {
            TreeName tree = new TreeName();
            Array.Copy(_items, tree._items, _items.Length);
            tree._count = _count;
            return tree;
        }

        #endregion ICloneable 成员

        #region IEnumerator 成员

        public void Reset()
        {
            if (_count == 0) return;
            if (_list == null || _list.Length != _count)
            {
                _list = new TreeNameNode[_count];
                Stack stack = new Stack(_count);

                int index = 0;
                for (int i = 0; i < _items.Length; i++)
                {
                    TreeNameNode node = _items[i];
                    while (node != null || stack.Count > 0)
                    {
                        while (node != null)
                        {
                            stack.Push(node);
                            node = node.Left;
                        }
                        if (stack.Count > 0)
                        {
                            node = (TreeNameNode)stack.Pop();
                            _list[index++] = node;
                            node = node.Right;
                        }
                    }
                }
            }
            _cursor = -1;
        }

        public object Current
        {
            get
            {
                return _list[_cursor];
            }
        }

        public bool MoveNext()
        {
            _cursor++;
            if (_cursor > _count - 1) return false;
            return true;
        }

        #endregion IEnumerator 成员
    }
}