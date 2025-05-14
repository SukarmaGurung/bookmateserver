using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDTO>> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId));

        if (cart == null) return Ok(new CartResponseDTO());

        return Ok(MapToCartResponse(cart));
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartDTO cartDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var parsedUserId = int.Parse(userId);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == parsedUserId);

        if (cart == null)
        {
            cart = new Cart { UserId = parsedUserId };
            _context.Carts.Add(cart);
        }

        var book = await _context.Books.FindAsync(cartDto.BookId);
        if (book == null) return NotFound("Book not found");

        var existingItem = cart.Items.FirstOrDefault(i => i.BookId == cartDto.BookId && i.Format == cartDto.Format);
        if (existingItem != null)
        {
            existingItem.Quantity += cartDto.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                BookId = cartDto.BookId,
                Quantity = cartDto.Quantity,
                Format = cartDto.Format ?? "Hardcopy"
            });
        }

        cart.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCart([FromBody] CartDTO cartDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var parsedUserId = int.Parse(userId);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == parsedUserId);

        if (cart == null) return NotFound("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.BookId == cartDto.BookId);
        if (item == null) return NotFound("Item not found in cart");

        item.Quantity = cartDto.Quantity;
        cart.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveFromCart([FromQuery] int bookId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var parsedUserId = int.Parse(userId);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == parsedUserId);

        if (cart == null) return NotFound("Cart not found");

        var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);
        if (item == null) return NotFound("Item not found in cart");

        cart.Items.Remove(item);
        cart.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

    private CartResponseDTO MapToCartResponse(Cart cart)
    {
        var response = new CartResponseDTO();
        response.Items = cart.Items.Select(i => new CartItemDTO
        {
            Id = i.Id,
            BookId = i.BookId,
            Title = i.Book.Title,
            ImageUrl = i.Book.ImageUrl,
            Price = i.Book.Price,
            Quantity = i.Quantity,
            Format = i.Format
        }).ToList();

        response.Subtotal = response.Items.Sum(i => i.Price * i.Quantity);
        response.Discount = CalculateDiscounts(cart);
        response.Total = response.Subtotal - response.Discount;

        return response;
    }

    private decimal CalculateDiscounts(Cart cart)
    {
        decimal discount = 0;
        int totalItems = cart.Items.Sum(i => i.Quantity);

        if (totalItems >= 5) discount += cart.Items.Sum(i => i.Book.Price * i.Quantity) * 0.05m;

        return discount;
    }
}