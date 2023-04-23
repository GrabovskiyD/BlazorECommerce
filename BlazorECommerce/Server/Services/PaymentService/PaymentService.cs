using Stripe;
using Stripe.Checkout;

namespace BlazorECommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        const string secret = "whsec_3cbe3bc308e0ef02d0666d5c9ca4fa520fc8985a79c1baa5b7387ceb4ef80dfa";

        public PaymentService(ICartService cartService,
            IAuthService authService,
            IOrderService orderService)
        {
            StripeConfiguration.ApiKey = "sk_test_51MwVBqG8aePJCaOR1OQ8SbJCGN8lf49jBiHFOmrI7Zw9CHj4D9xr3aDaFXEOnDvtGuYBGAh0La2SQEgYznKzNrW100wQcrnen6";

            _cartService = cartService;
            _authService = authService;
            _orderService = orderService;
        }
        public async Task<Session> CreateCheckoutSessionAsync()
        {
            var products = (await _cartService.GetDbCartProductsAsync()).Data;
            var lineItems = new List<SessionLineItemOptions>();
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Title,
                        Images = new List<string>
                        {
                            product.ImageUrl
                        }
                    }
                },
                Quantity = product.Quantity
            }));
            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string> { "US"}
                },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7190/order-success",
                CancelUrl = "https://localhost:7190/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public async Task<ServiceResponse<bool>> FulfillOrderAsync(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    secret);

                if(stripeEvent.Type is Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authService.GetUserByEmailAsync(session.CustomerEmail);
                    await _orderService.PlaceOrderAsync(user.Id);
                }

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true
                };
            }
            catch (StripeException ex) 
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
