using Stripe.Checkout;

namespace BlazorECommerce.Server.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Session> CreateCheckoutSessionAsync();
        Task<ServiceResponse<bool>> FulfillOrderAsync(HttpRequest request);
    }
}
