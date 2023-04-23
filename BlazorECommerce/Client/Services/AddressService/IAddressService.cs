namespace BlazorECommerce.Client.Services.AddressService
{
    public interface IAddressService
    {
        Task<Address> GetAddressAsync();
        Task<Address> AddOrUpdateAddressAsync(Address address);
    }
}
