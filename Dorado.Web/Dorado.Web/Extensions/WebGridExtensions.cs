using System.Collections.Generic;
using System.Web.Mvc;

namespace Dorado.Web.Extensions
{
    public static class WebGridExtensions
    {
        /// <summary>
        /// Light-weight wrapper around the constructor for WebGrid so that we get take advantage of compiler type inference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
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
        /// <returns></returns>
        public static WebGrid<T> Grid<T>(this HtmlHelper htmlHelper,
                                                 IEnumerable<T> source,
                                                 IEnumerable<string> columnNames = null,
                                                 string defaultSort = null,
                                                 int rowsPerPage = 10,
                                                 bool canPage = true,
                                                 bool canSort = true,
                                                 string ajaxUpdateContainerId = null,
                                                 string ajaxUpdateCallback = null,
                                                 string fieldNamePrefix = null,
                                                 string pageFieldName = null,
                                                 string selectionFieldName = null,
                                                 string sortFieldName = null,
                                                 string sortDirectionFieldName = null)
        {
            return new WebGrid<T>(source, columnNames,
                                      defaultSort,
                                      rowsPerPage,
                                      canPage,
                                      canSort,
                                      ajaxUpdateContainerId,
                                      ajaxUpdateCallback,
                                      fieldNamePrefix,
                                      pageFieldName,
                                      selectionFieldName,
                                      sortFieldName,
                                      sortDirectionFieldName);
        }

        /// <summary>
        /// Light-weight wrapper around the constructor for WebGrid so that we get take advantage of compiler type inference and to automatically call Bind to disable the automatic paging and sorting (use this for server-side paging)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="source"></param>
        /// <param name="totalRows"></param>
        /// <param name="columnNames"></param>
        /// <param name="defaultSort"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="canPage"></param>
        /// <param name="canSort"></param>
        /// <param name="ajaxUpdateContainerId"></param>
        /// <param name="ajaxUpdateCallback"></param>
        /// <param name="fieldNamePrefix"></param>
        /// <param name="pageFieldName"></param>
        /// <param name="selectionFieldName"></param>
        /// <param name="sortFieldName"></param>
        /// <param name="sortDirectionFieldName"></param>
        /// <returns></returns>
        public static WebGrid<T> ServerPagedGrid<T>(this HtmlHelper htmlHelper,
                                                    IEnumerable<T> source,
                                                    int totalRows,
                                                    IEnumerable<string> columnNames = null,
                                                    string defaultSort = null,
                                                    int rowsPerPage = 10,
                                                    bool canPage = true,
                                                    bool canSort = true,
                                                    string ajaxUpdateContainerId = null,
                                                    string ajaxUpdateCallback = null,
                                                    string fieldNamePrefix = null,
                                                    string pageFieldName = null,
                                                    string selectionFieldName = null,
                                                    string sortFieldName = null,
                                                    string sortDirectionFieldName = null)
        {
            var webGrid = new WebGrid<T>(null,
                columnNames,
                                             defaultSort,
                                             rowsPerPage,
                                             canPage,
                                             canSort,
                                             ajaxUpdateContainerId,
                                             ajaxUpdateCallback,
                                             fieldNamePrefix,
                                             pageFieldName,
                                             selectionFieldName,
                                             sortFieldName,
                                             sortDirectionFieldName);
            return webGrid.Bind(source, rowCount: totalRows, autoSortAndPage: false); ;
        }
    }
}