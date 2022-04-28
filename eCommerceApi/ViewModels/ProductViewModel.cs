namespace eCommerceApi.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public decimal Price { get; set; }
        public bool IncludeGST { get; set; }
        public decimal GSTPercentage { get; set; }
    }
}
