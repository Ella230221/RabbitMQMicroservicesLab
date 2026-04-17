using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            return await ReadListAsync<ProductDto>("/products");
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            return await ReadListAsync<CustomerDto>("/customers");
        }

        public async Task<bool> CreateProductAsync(ProductDto product)
        {
            var response = await _http.PostAsJsonAsync("/products", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderDto>> GetOrdersAsync()
        {
            return await ReadListAsync<OrderDto>("/orders");
        }

        public async Task<List<PaymentDto>> GetPaymentsAsync()
        {
            return await ReadListAsync<PaymentDto>("/payments");
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            return await _http.GetFromJsonAsync<DashboardSummaryDto>("/dashboard/summary")
                ?? new DashboardSummaryDto();
        }

        public async Task<bool> CreateCustomerAsync(CustomerDto customer)
        {
            var response = await _http.PostAsJsonAsync("/customers", customer);
            return response.IsSuccessStatusCode;
        }

        public async Task<(bool Success, string Message)> CreateOrderAsync(CreateOrderRequest order)
        {
            var response = await _http.PostAsJsonAsync("/orders", order);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Order created successfully.");
            }

            var message = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(message) ? "Unable to create order." : message);
        }

        private async Task<List<T>> ReadListAsync<T>(string url)
        {
            var data = await _http.GetFromJsonAsync<List<T>>(url);
            return data ?? new List<T>();
        }
    }
}
