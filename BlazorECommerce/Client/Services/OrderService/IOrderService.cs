﻿using BlazorECommerce.Shared.Model.DTOs;

namespace BlazorECommerce.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task PlaceOrderAsync();
        Task<List<OrderOverviewResponse>> GetOrdersAsync();
        Task<OrderDetailsResponse> GetOrderDetailsAsync(int orderId);
    }
}
