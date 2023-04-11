using System.Security.Claims;

namespace BlazorECommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(DataContext dataContext, 
            ICartService cartService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        public async Task<ServiceResponse<bool>> PlaceOrderAsync()
        {
            var products = (await _cartService.GetDbCartProductsAsync()).Data;

            decimal totalPrice = 0;

            products.ForEach(product => totalPrice += product.Price * product.Quantity);

            var orderItems = new List<OrderItem>();
            products.ForEach(product => orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));

            var order = new Order
            {
                UserId = GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            _dataContext.Orders.Add(order);

            _dataContext.CartItems.RemoveRange(_dataContext.CartItems
                .Where(ci => ci.UserId == GetUserId()));

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
