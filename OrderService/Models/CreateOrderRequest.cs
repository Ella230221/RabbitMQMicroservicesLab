using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class CreateOrderRequest
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
