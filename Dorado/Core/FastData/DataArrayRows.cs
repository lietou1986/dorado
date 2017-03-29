using System.Collections;

namespace Dorado.Core.FastData
{
    public class DataArrayRow
    {
        private readonly DataArrayRows _rows;

        public DataArrayRow(DataArrayRows rows)
        {
            _rows = rows;
        }

        public DataArray DataArray
        {
            get
            {
                return _rows.DataArray;
            }
        }

        public DataArrayColumn this[string name]
        {
            get
            {
                DataArrayColumn col = _rows.DataArray[name, _rows.Cursor];
                return col;
            }
        }
    }

    /// <summary>
    /// DataArrayRow 的摘要说明。
    /// </summary>
    public class DataArrayRows : IEnumerable, IEnumerator
    {
        internal int Cursor = -1;
        private readonly DataArray _array;

        public DataArrayRows(DataArray arr)
        {
            _array = arr;
        }

        public int Count
        {
            get
            {
                return _array.Count;
            }
        }

        public DataArray DataArray
        {
            get
            {
                return _array;
            }
        }

        #region IEnumerator 成员

        public void Reset()
        {
            Cursor = -1;
        }

        public object Current
        {
            get
            {
                return new DataArrayRow(this);
            }
        }

        public DataArrayColumn this[string name]
        {
            get
            {
                return _array[name];
            }
        }

        public DataArrayColumn this[string name, int row]
        {
            get
            {
                return _array[name, row];
            }
        }

        public bool MoveNext()
        {
            if (Cursor < _array.Count - 1)
            {
                Cursor++;
                return true;
            }
            return false;
        }

        #endregion IEnumerator 成员

        #region IEnumerable 成员

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        #endregion IEnumerable 成员
    }
}