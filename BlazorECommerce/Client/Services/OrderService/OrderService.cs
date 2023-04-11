﻿using BlazorECommerce.Shared.Model.DTOs;
using Microsoft.AspNetCore.Components;

namespace BlazorECommerce.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        public OrderService(HttpClient httpClient,
            NavigationManager navigationManager,
            IAuthService authService)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _authService = authService;
        }

        public async Task PlaceOrderAsync()
        {
            if(await _authService.IsUserAuthenticatedAsync())
            {
                await _httpClient.PostAsync("api/order", null);
            }
            else
            {
                _navigationManager.NavigateTo("login");
            }
        }

        public async Task<List<OrderOverviewResponse>> GetOrdersAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderOverviewResponse>>>("api/order");
            return result.Data;
        }

        public async Task<OrderDetailsResponse> GetOrderDetailsAsync(int orderId)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>($"api/order/{orderId}");
            return result.Data;
        }
    }
}
