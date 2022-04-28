namespace eCommerceApi.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool isDeleted { get; set; }
    }
}
