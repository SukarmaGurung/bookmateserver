namespace server1.DTOs
{
    public class DiscountDto
    {
        public bool IsOnSale { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? DiscountStartTime { get; set; }
        public DateTime? DiscountEndTime { get; set; }
    }
}
