namespace server1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "System" for predefined, "Custom" for admin-created
    }
}