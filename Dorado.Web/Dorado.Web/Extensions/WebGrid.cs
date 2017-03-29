using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Web.Helpers;

namespace Dorado.Web.Extensions
{
    /// <summary>
    /// Wrapper for System.Web.Helpers.WebGrid that preserves the item type from the data source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebGrid<T> : WebGrid
    {
        /// <param name="source">Data source</param>
        /// <param name="columnNames">Data source column names. Auto-populated by default.</param>
        /// <param name="defaultSort">Default sort column.</param>
        /// <param name="rowsPerPage">Number of rows per page.</param>
        /// <param name="canPage">true to enable paging</param>
        /// <param name="canSort">true to enable sorting</param>
        /// <param name="ajaxUpdateContainerId">ID for the grid's container element. This enables AJAX support.</param>
        /// <param name="ajaxUpdateCallback">Callback function for the AJAX functionality once the update is complete</param>
        /// <param name="fieldNamePrefix">Prefix for query string fields to support multiple grids.</param>
        /// <param name="pageFieldName">Query string field name for page number.</param>
        /// <param name="selectionFieldName">Query string field name for selected row number.</param>
        /// <param name="sortFieldName">Query string field name for sort column.</param>
        /// <param name="sortDirectionFieldName">Query string field name for sort direction.</param>
        public WebGrid(IEnumerable<T> source = null, IEnumerable<string> columnNames = null, string defaultSort = null, int rowsPerPage = 10, bool canPage = true, bool canSort = true, string ajaxUpdateContainerId = null, string ajaxUpdateCallback = null, string fieldNamePrefix = null, string pageFieldName = null, string selectionFieldName = null, string sortFieldName = null, string sortDirectionFieldName = null)
            : base(source.SafeCast<object>(), columnNames, defaultSort, rowsPerPage, canPage, canSort, ajaxUpdateContainerId, ajaxUpdateCallback, fieldNamePrefix, pageFieldName, selectionFieldName, sortFieldName, sortDirectionFieldName)
        {
        }

        public WebGridColumn Column(string columnName = null, string header = null, Func<T, object> format = null, string style = null, bool canSort = true)
        {
            Func<dynamic, object> wrappedFormat = null;
            if (format != null)
            {
                wrappedFormat = o => format((T)o.Value);
            }
            WebGridColumn column = base.Column(columnName, header, wrappedFormat, style, canSort);
            return column;
        }

        public WebGrid<T> Bind(IEnumerable<T> source, IEnumerable<string> columnNames = null, bool autoSortAndPage = true, int rowCount = -1)
        {
            base.Bind(source.SafeCast<object>(), columnNames, autoSortAndPage, rowCount);
            return this;
        }
    }
}