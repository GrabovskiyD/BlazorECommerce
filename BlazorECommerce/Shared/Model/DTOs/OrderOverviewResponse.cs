namespace BlazorECommerce.Shared.Model.DTOs
{
    public class OrderOverviewResponse
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Product { get; set; }
        public string ProductImageUrl { get; set; }
    }
}
