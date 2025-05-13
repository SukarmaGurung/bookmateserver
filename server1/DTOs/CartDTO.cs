namespace server1.DTOs
{
    public class CartDTO
    {
        public int BookId { get; set; }
        public int Quantity { get; set; } = 1;
        public string Format { get; set; }
    }

    public class CartResponseDTO
    {
        public List<CartItemDTO> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItemDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Format { get; set; }
    }
}