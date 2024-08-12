namespace Onyx.Models
{
    /// <summary>
    /// A set of query parameters for GetMany method: field selection, filtering, sorting, pagination.
    /// </summary>
    public class QueryParams
    {
        /// <summary>
        /// List of fields to filter the data by.
        /// </summary>
        public List<string> FilterField { get; set; }

        /// <summary>
        /// List of values for filter fields.
        /// </summary>
        public List<string> FilterValue { get; set; }

        /// <summary>
        /// String name of the field that will be used for sorting.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Ascending/descending order.
        /// </summary>
        public bool IsAscending { get; set; }

        /// <summary>
        /// Page number, always starts with 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Amount of records per page.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
