namespace server1.Models
{
    public enum OrderStatus { Pending, Fulfilled, Cancelled }

    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => Subtotal - Discount;
        public string ClaimCode { get; set; } = Guid.NewGuid().ToString()[..8].ToUpper();
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}