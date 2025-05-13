using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;
using server1.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;

    public OrderController(AppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // POST: api/order
    [HttpPost]
    public async Task<ActionResult<OrderResponseDTO>> PlaceOrder()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FindAsync(userId);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
            return BadRequest("Cart is empty");

        // Create order
        var order = new Order
        {
            UserId = userId,
            Items = cart.Items.Select(i => new OrderItem
            {
                BookId = i.BookId,
                Quantity = i.Quantity,
                PriceAtPurchase = i.Book.Price
            }).ToList(),
            Subtotal = cart.Items.Sum(i => i.Book.Price * i.Quantity),
            Discount = CalculateDiscounts(cart, user)
        };

        // Update book stock
        foreach (var item in cart.Items)
        {
            var book = await _context.Books.FindAsync(item.BookId);
            book.Stock -= item.Quantity;
            book.SoldCount += item.Quantity;
        }

        // Clear cart
        _context.Carts.Remove(cart);

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // Send confirmation email
        await _emailService.SendOrderConfirmation(user.Email, order);

        return Ok(MapToOrderResponse(order));
    }

    // GET: api/order/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDTO>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return Ok(MapToOrderResponse(order));
    }

    // DELETE: api/order/{id} (Cancel order)
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private OrderResponseDTO MapToOrderResponse(Order order)
    {
        return new OrderResponseDTO
        {
            Id = order.Id,
            ClaimCode = order.ClaimCode,
            OrderDate = order.OrderDate,
            Total = order.Total,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemDTO
            {
                BookId = i.BookId,
                Title = i.Book.Title,
                Quantity = i.Quantity,
                Price = i.PriceAtPurchase
            }).ToList()
        };
    }

    private decimal CalculateDiscounts(Cart cart, User user)
    {
        decimal discount = 0;
        if (cart?.Items == null || !cart.Items.Any()) return discount;

        int totalItems = cart.Items.Sum(i => i.Quantity);
        decimal subtotal = cart.Items.Sum(i => i.Book?.Price ?? 0 * i.Quantity);

        // 5% discount for 5+ books
        if (totalItems >= 5)
        {
            discount += subtotal * 0.05m;
        }

        // 10% loyalty discount (check user's past orders)
        if (user?.Id != null)
        {
            var completedOrders = _context.Orders
                .Count(o => int.Parse (o.UserId) == user.Id && o.Status == OrderStatus.Fulfilled);

            if (completedOrders >= 10)
            {
                discount += subtotal * 0.10m;
            }
        }

        return discount;
    }
}