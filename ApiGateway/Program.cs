using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ApiGateway;
using ApiGateway.Models;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
var ocelotEnvironment = builder.Configuration["Ocelot:Environment"] ?? "Local";
var serviceHosts = ocelotEnvironment.Equals("Docker", StringComparison.OrdinalIgnoreCase)
    ? new Dictionary<string, string>
    {
        ["Products"] = "http://productservice:8080",
        ["Customers"] = "http://customerservice:8080",
        ["Orders"] = "http://orderservice:8080",
        ["Payments"] = "http://paymentservice:8080"
    }
    : new Dictionary<string, string>
    {
        ["Products"] = "http://localhost:5003",
        ["Customers"] = "http://localhost:5001",
        ["Orders"] = "http://localhost:5002",
        ["Payments"] = "http://localhost:5004"
    };

// 🔥 ADD CORS (REQUIRED for Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Configuration.AddJsonFile($"ocelot.{ocelotEnvironment}.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient("Products", client => client.BaseAddress = new Uri(serviceHosts["Products"]));
builder.Services.AddHttpClient("Customers", client => client.BaseAddress = new Uri(serviceHosts["Customers"]));
builder.Services.AddHttpClient("Orders", client => client.BaseAddress = new Uri(serviceHosts["Orders"]));
builder.Services.AddHttpClient("Payments", client => client.BaseAddress = new Uri(serviceHosts["Payments"]));
var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAll");

app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiGateway v1");
    options.DocumentTitle = "ApiGateway Swagger";
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/swagger/v1/swagger.json", (HttpContext httpContext) =>
    {
        var json = GatewayOpenApiDocumentFactory.CreateJson(httpContext.Request);
        return Results.Text(json, "application/json");
    });

    endpoints.MapGet("/aggregations/orders/{id:int}", async (int id, IHttpClientFactory httpClientFactory) =>
    {
        var ordersClient = httpClientFactory.CreateClient("Orders");
        var orderResponse = await ordersClient.GetAsync($"/api/orders/{id}");
        if (!orderResponse.IsSuccessStatusCode)
        {
            return orderResponse.StatusCode == System.Net.HttpStatusCode.NotFound
                ? Results.NotFound()
                : Results.StatusCode((int)orderResponse.StatusCode);
        }

        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        if (order is null)
        {
            return Results.BadRequest("Unable to read order details.");
        }

        var customersClient = httpClientFactory.CreateClient("Customers");
        var productsClient = httpClientFactory.CreateClient("Products");
        var paymentsClient = httpClientFactory.CreateClient("Payments");

        var customerTask = GetOptionalAsync<CustomerDto>(customersClient, $"/api/customers/{order.CustomerId}");
        var productTask = GetOptionalAsync<ProductDto>(productsClient, $"/api/products/{order.ProductId}");
        var paymentTask = GetOptionalAsync<PaymentDto>(paymentsClient, $"/api/payments/by-order/{order.Id}");

        await Task.WhenAll(customerTask, productTask, paymentTask);

        var aggregate = new OrderAggregateDto
        {
            Order = order,
            Customer = customerTask.Result,
            Product = productTask.Result,
            Payment = paymentTask.Result
        };

        return Results.Ok(aggregate);
    });

    endpoints.MapGet("/dashboard/summary", async (IHttpClientFactory httpClientFactory) =>
    {
        var productsClient = httpClientFactory.CreateClient("Products");
        var customersClient = httpClientFactory.CreateClient("Customers");
        var ordersClient = httpClientFactory.CreateClient("Orders");
        var paymentsClient = httpClientFactory.CreateClient("Payments");

        var productsTask = productsClient.GetFromJsonAsync<List<ProductDto>>("/api/products");
        var customersTask = customersClient.GetFromJsonAsync<List<CustomerDto>>("/api/customers");
        var ordersTask = ordersClient.GetFromJsonAsync<List<OrderDto>>("/api/orders");
        var paymentsTask = paymentsClient.GetFromJsonAsync<List<PaymentDto>>("/api/payments");

        await Task.WhenAll(productsTask!, customersTask!, ordersTask!, paymentsTask!);

        var products = productsTask.Result ?? new List<ProductDto>();
        var customers = customersTask.Result ?? new List<CustomerDto>();
        var orders = ordersTask.Result ?? new List<OrderDto>();
        var payments = paymentsTask.Result ?? new List<PaymentDto>();

        var summary = new DashboardSummaryDto
        {
            ProductCount = products.Count,
            CustomerCount = customers.Count,
            OrderCount = orders.Count,
            PaymentCount = payments.Count,
            TotalRevenue = orders.Sum(order => order.TotalPrice),
            TotalProcessedPayments = payments.Sum(payment => payment.Amount),
            LatestOrders = orders
                .OrderByDescending(order => order.CreatedAtUtc)
                .Take(5)
                .ToList(),
            LatestPayments = payments
                .OrderByDescending(payment => payment.ProcessedAtUtc)
                .Take(5)
                .ToList()
        };

        return Results.Ok(summary);
    });
});

await app.UseOcelot(); // ONLY routing layer

app.Run();

static async Task<T?> GetOptionalAsync<T>(HttpClient client, string requestUri)
{
    var response = await client.GetAsync(requestUri);
    if (!response.IsSuccessStatusCode)
    {
        return default;
    }

    return await response.Content.ReadFromJsonAsync<T>();
}
