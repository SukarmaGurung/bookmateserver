namespace server1.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Links to Member
        public List<CartItem> Items { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }  // Navigation property
        public int Quantity { get; set; } = 1;
        public string Format { get; set; }  // Paperback/Hardcover etc.
    }
}