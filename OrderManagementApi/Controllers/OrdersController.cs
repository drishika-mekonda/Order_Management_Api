using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Place a new order who are Authenticated
        [Authorize]
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            if (order == null) return BadRequest("Order is null.");

            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            order.UserName = userName;
            order.TotalAmount = order.Quantity * order.UnitPrice;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order placed successfully", orderId = order.Id });
        }

        // Get all orders for current user
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.UserName == userName)
                .ToListAsync();

            return Ok(orders);
        }

        // Get a single order for current user
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);

            if (order == null) return NotFound();

            return Ok(order);
        }

        // Update an existing order (only if it belongs to current user)
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updated)
        {
            if (updated == null) return BadRequest("Invalid order data.");

            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);
            if (order == null) return NotFound();

            order.ProductName = updated.ProductName;
            order.Quantity = updated.Quantity;
            order.UnitPrice = updated.UnitPrice;
            order.TotalAmount = updated.Quantity * updated.UnitPrice;

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // Delete an order (only if it belongs to current user)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return Unauthorized();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserName == userName);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order deleted successfully",
                orderId = id
            });
        }
    }
}


