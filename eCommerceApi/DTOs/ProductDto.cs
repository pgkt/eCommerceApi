namespace eCommerceApi.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public decimal Price { get; set; }
        public bool IncludeGST { get; set; }
        public decimal GSTPercentage { get; set; }
    }
}
