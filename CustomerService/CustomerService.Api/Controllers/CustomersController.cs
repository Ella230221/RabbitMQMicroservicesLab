using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CustomerService.Api.Data;
using CustomerService.Api.Models;

namespace CustomerService.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public CustomersController(CustomerDbContext context)
    {
        _context = context;
    }

    // ✅ GET ALL
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var customers = await _context.Customers.ToListAsync();

        var result = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email
        });

        return Ok(result);
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        var dto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };

        return Ok(dto);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }
}
