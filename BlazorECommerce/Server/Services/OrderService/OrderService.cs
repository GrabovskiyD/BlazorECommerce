namespace BlazorECommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;

        public OrderService(DataContext dataContext,
            ICartService cartService,
            IAuthService authService)
        {
            _dataContext = dataContext;
            _cartService = cartService;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrdersAsync()
        {
            var orders = await _dataContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == _authService.GetUserId())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            var orderResponse = new List<OrderOverviewResponse>();
            orders.ForEach(o => orderResponse.Add(new OrderOverviewResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate.ToString("dd.MM.yyyy HH:mm"),
                TotalPrice = o.TotalPrice,
                Product = o.OrderItems.Count > 1 ?
                    $"{o.OrderItems.First().Product.Title} and " +
                    $"{o.OrderItems.Count - 1} more..." :
                    o.OrderItems.First().Product.Title,
                    ProductImageUrl = o.OrderItems.First().Product.ImageUrl
            }));

            var response = new ServiceResponse<List<OrderOverviewResponse>> 
            {
                Data = orderResponse,
                Success = true
            };
            return response;
        }

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
                UserId = _authService.GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            _dataContext.Orders.Add(order);

            _dataContext.CartItems.RemoveRange(_dataContext.CartItems
                .Where(ci => ci.UserId == _authService.GetUserId()));

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
