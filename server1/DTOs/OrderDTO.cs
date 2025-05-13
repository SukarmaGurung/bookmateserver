namespace server1.DTOs
{
    public class OrderCreateDTO
    {
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string ClaimCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}