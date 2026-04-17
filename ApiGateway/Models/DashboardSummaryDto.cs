namespace ApiGateway.Models;

public class DashboardSummaryDto
{
    public int ProductCount { get; set; }
    public int CustomerCount { get; set; }
    public int OrderCount { get; set; }
    public int PaymentCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProcessedPayments { get; set; }
    public List<OrderDto> LatestOrders { get; set; } = new();
    public List<PaymentDto> LatestPayments { get; set; } = new();
}
