namespace BlazorECommerce.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;

        public AddressService(DataContext dataContext, IAuthService authService)
        {
            _dataContext = dataContext;
            _authService = authService;
        }
        public async Task<ServiceResponse<Address>> AddOrUpdateAddressAsync(Address address)
        {
            var response = new ServiceResponse<Address>();
            var dbAddress = (await GetAddressAsync()).Data;
            if(dbAddress is null) 
            {
                address.UserId = _authService.GetUserId();
                _dataContext.Addresses.Add(address);
                response.Data = address;
                response.Success = true;
                response.Message = "The new address added.";
            }
            else
            {
                dbAddress.FirstName = address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.State = address.State;
                dbAddress.Country = address.Country;
                dbAddress.City = address.City;
                dbAddress.Zip = address.Zip;
                dbAddress.Street = address.Street;
                response.Data = dbAddress;
                response.Success = true;
                response.Message = "The address updated.";
            }
            await _dataContext.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<Address>> GetAddressAsync()
        {
            int userId = _authService.GetUserId();
            var address = await _dataContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
            return new ServiceResponse<Address>
            {
                Data = address,
                Success = true
            };
        }
    }
}
