namespace server1.DTOs
{
    public class BookQueryParameters
    {
        public string? Search { get; set; }
        public string? Genre { get; set; }
        public string? Author { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
