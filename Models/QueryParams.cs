namespace Onyx.Models
{
    public class QueryParams
    {
        public List<string> FilterField { get; set; }
        public List<string> FilterValue { get; set; }
        public string SortBy { get; set; }
        public bool IsAscending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
