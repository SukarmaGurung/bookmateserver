namespace server1.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public string Publisher { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SoldCount { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAwardWinner { get; set; } = false; 
        public bool IsOnSale { get; set; } = false; 
        public decimal? DiscountPrice { get; set; } 
        public DateTime? DiscountStartDate { get; set; } 
        public DateTime? DiscountEndDate { get; set; }
        public decimal? DiscountPercentage { get; internal set; }
        public DateTime? DiscountStartTime { get; internal set; }
        public DateTime? DiscountEndTime { get; internal set; }
        public decimal GetCurrentPrice()
        {
            var now = DateTime.UtcNow;
            if (IsOnSale && DiscountPercentage.HasValue && DiscountStartTime <= now && DiscountEndTime >= now)
            {
                return Price - (Price * DiscountPercentage.Value / 100);
            }
            return Price;
        }
    }
}
