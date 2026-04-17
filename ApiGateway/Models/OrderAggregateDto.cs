namespace ApiGateway.Models;

public class OrderAggregateDto
{
    public OrderDto Order { get; set; } = new();
    public CustomerDto? Customer { get; set; }
    public ProductDto? Product { get; set; }
    public PaymentDto? Payment { get; set; }
}
