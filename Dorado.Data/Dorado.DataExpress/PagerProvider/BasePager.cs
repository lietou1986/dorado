using System;

namespace Dorado.DataExpress.PagerProvider
{
    public abstract class BasePager
    {
        private int _currentPage = 1;
        private int _pageSize = 20;

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;
            }
        }

        public int CurrentPage
        {
            get
            {
                return this._currentPage;
            }
            set
            {
                this._currentPage = value;
            }
        }

        public int Start
        {
            get;
            set;
        }

        public QueryStatement Query
        {
            get;
            set;
        }

        public PagedDataTable ExecuteByPageSize(int pageSize, int page)
        {
            this.PageSize = pageSize;
            this.CurrentPage = page;
            this.Start = pageSize * (page - 1);
            return this.ExecuteAndWrapTable();
        }

        public PagedEntity<T> ExecuteByPageSize<T>(int pageSize, int page) where T : new()
        {
            this.PageSize = pageSize;
            this.CurrentPage = page;
            this.Start = this.PageSize * (page - 1);
            return this.ExecuteAndWrapEntity<T>();
        }

        public PagedDataTable Execute(int page)
        {
            this.CurrentPage = page;
            this.Start = this.PageSize * (page - 1);
            return this.ExecuteAndWrapTable();
        }

        public PagedEntity<T> Execute<T>(int page) where T : new()
        {
            this.CurrentPage = page;
            this.Start = this.PageSize * (page - 1);
            return this.ExecuteAndWrapEntity<T>();
        }

        public PagedDataTable ExecuteByLimit(int start, int limit)
        {
            this.PageSize = limit;
            this.CurrentPage = (int)Math.Ceiling((double)start / (double)limit) + 1;
            this.Start = start;
            return this.ExecuteAndWrapTable();
        }

        public PagedEntity<T> ExecuteByLimit<T>(int start, int limit) where T : new()
        {
            this.PageSize = limit;
            this.CurrentPage = (int)Math.Ceiling((double)start / (double)limit) + 1;
            this.Start = start;
            return this.ExecuteAndWrapEntity<T>();
        }

        internal PagedEntity<T> ExecuteAndWrapEntity<T>() where T : new()
        {
            PagedEntity<T> pagedEntity = this.Execute<T>();
            pagedEntity.Descriptor.Start = (long)this.Start;
            pagedEntity.Descriptor.Limit = ((pagedEntity.Rows == null) ? 0L : ((long)pagedEntity.Rows.Count));
            pagedEntity.Descriptor.PageSize = this.PageSize;
            pagedEntity.Descriptor.CurrentPage = this.CurrentPage;
            pagedEntity.Descriptor.TotalPage = (int)Math.Ceiling((double)pagedEntity.Descriptor.Total / (double)this.PageSize);
            return pagedEntity;
        }

        internal PagedDataTable ExecuteAndWrapTable()
        {
            PagedDataTable pagedDataTable = this.Execute();
            pagedDataTable.Descriptor.Start = (long)this.Start;
            pagedDataTable.Descriptor.Limit = ((pagedDataTable.Data.Rows == null) ? 0L : ((long)pagedDataTable.Data.Rows.Count));
            pagedDataTable.Descriptor.PageSize = this.PageSize;
            pagedDataTable.Descriptor.CurrentPage = this.CurrentPage;
            pagedDataTable.Descriptor.TotalPage = (int)Math.Ceiling((double)pagedDataTable.Descriptor.Total / (double)this.PageSize);
            return pagedDataTable;
        }

        public abstract PagedDataTable Execute();

        public abstract PagedEntity<T> Execute<T>() where T : new();
    }
}