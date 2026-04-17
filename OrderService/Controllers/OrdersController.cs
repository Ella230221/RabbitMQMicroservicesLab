using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RabbitMQPublisher _publisher;

        public OrdersController(
            OrderDbContext dbContext,
            IHttpClientFactory httpClientFactory,
            RabbitMQPublisher publisher)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _dbContext.Orders
                .OrderByDescending(order => order.CreatedAtUtc)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var customerClient = _httpClientFactory.CreateClient("Customers");
            var productClient = _httpClientFactory.CreateClient("Products");

            var customerResponse = await customerClient.GetAsync($"/api/customers/{request.CustomerId}");
            if (customerResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return BadRequest("Customer does not exist.");
            }

            customerResponse.EnsureSuccessStatusCode();
            var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
            if (customer is null)
            {
                return BadRequest("Unable to read customer details.");
            }

            var productResponse = await productClient.GetAsync($"/api/products/{request.ProductId}");
            if (productResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return BadRequest("Product does not exist.");
            }

            productResponse.EnsureSuccessStatusCode();
            var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();
            if (product is null)
            {
                return BadRequest("Unable to read product details.");
            }

            if (product.Stock < request.Quantity)
            {
                return BadRequest($"Only {product.Stock} item(s) left in stock.");
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * request.Quantity,
                Status = "Created",
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            var message = JsonSerializer.Serialize(new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                UnitPrice = order.UnitPrice,
                TotalPrice = order.TotalPrice,
                CreatedAtUtc = order.CreatedAtUtc
            });

            _publisher.Publish(message);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}
