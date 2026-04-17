namespace PaymentService.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
}
