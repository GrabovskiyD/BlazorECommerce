namespace BlazorECommerce.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> RegisterAsync(User user, string password);
        Task<bool> UserExistsAsync(string email);
        Task<ServiceResponse<string>> LoginAsync(string email, string password);
        Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, string newPassword);
        int GetUserId();
    }
}
