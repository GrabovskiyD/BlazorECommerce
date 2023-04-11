namespace BlazorECommerce.Shared.Model.DTOs
{
    public class OrderDetailsResponse
    {
        public string OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailsProductResponse> Products { get; set; }
    }
}
