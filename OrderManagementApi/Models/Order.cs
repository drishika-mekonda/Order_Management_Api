namespace OrderManagementApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }
        public string? UserName { get; set; }
    }
}