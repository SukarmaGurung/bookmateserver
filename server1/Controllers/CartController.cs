using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // Requires authentication
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/cart
    [HttpGet]
    public async Task<ActionResult<CartResponseDTO>> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.UserId == int.Parse( userId));

        if (cart == null) return Ok(new CartResponseDTO());

        return Ok(MapToCartResponse(cart));
    }

    // POST: api/cart
    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartDTO cartDto)
    {
        var userId = int.Parse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart { UserId = userId };

        var book = await _context.Books.FindAsync(cartDto.BookId);
        if (book == null) return NotFound("Book not found");

        var existingItem = cart.Items.FirstOrDefault(i => i.BookId == cartDto.BookId);
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
                Format = cartDto.Format
            });
        }

        _context.Carts.Update(cart);
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

        // 5% discount for 5+ books
        if (totalItems >= 5) discount += cart.Items.Sum(i => i.Book.Price * i.Quantity) * 0.05m;

        // TODO: Add 10% loyalty discount after checking user's order history
        return discount;
    }
}