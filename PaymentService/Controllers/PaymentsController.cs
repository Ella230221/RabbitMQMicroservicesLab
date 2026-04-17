using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentDbContext _context;

    public PaymentsController(PaymentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _context.Payments
            .Where(payment => payment.OrderId > 0)
            .OrderByDescending(payment => payment.ProcessedAtUtc)
            .ToListAsync();

        var result = payments.Select(MapToDto);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment is null || payment.OrderId <= 0)
        {
            return NotFound();
        }

        return Ok(MapToDto(payment));
    }

    [HttpGet("by-order/{orderId:int}")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        var payment = await _context.Payments
            .OrderByDescending(item => item.ProcessedAtUtc)
            .FirstOrDefaultAsync(item => item.OrderId == orderId);

        return payment is null ? NotFound() : Ok(MapToDto(payment));
    }

    private static PaymentDto MapToDto(Payment payment) => new()
    {
        Id = payment.Id,
        OrderId = payment.OrderId,
        CustomerId = payment.CustomerId,
        Amount = payment.Amount,
        Status = payment.Status,
        ProcessedAtUtc = payment.ProcessedAtUtc
    };
}
