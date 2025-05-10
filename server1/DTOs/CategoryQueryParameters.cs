namespace server1.DTOs
{
    public class CategoryQueryParameters
    {
        public string? Type { get; set; } // Filter by "System" or "Custom"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}