using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;
using server1.Services;
using System.Linq;



    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public BookController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // Admin: Create a book
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBook([FromForm] BookDTO bookDto)
        {
            if (bookDto.Image == null)
            {
                return BadRequest("No image uploaded.");
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(bookDto.Image);

            var book = new Book
            {
                Title = bookDto.Title,
                ISBN = bookDto.ISBN,
                Description = bookDto.Description,
                Author = bookDto.Author,
                Genre = bookDto.Genre,
                Language = bookDto.Language,
                Format = bookDto.Format,
                Publisher = bookDto.Publisher,
                PublicationDate = bookDto.PublicationDate,
                Price = bookDto.Price,
                Stock = bookDto.Stock,
                SoldCount = 0,
                ImageUrl = imageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok(book);
        }

        // Admin: Update a book
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookDTO bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");

            if (bookDto.Image != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(bookDto.Image);
                book.ImageUrl = imageUrl;
            }

            book.Title = bookDto.Title;
            book.ISBN = bookDto.ISBN;
            book.Description = bookDto.Description;
            book.Author = bookDto.Author;
            book.Genre = bookDto.Genre;
            book.Language = bookDto.Language;
            book.Format = bookDto.Format;
            book.Publisher = bookDto.Publisher;
            book.PublicationDate = bookDto.PublicationDate;
            book.Price = bookDto.Price;
            book.Stock = bookDto.Stock;

            await _context.SaveChangesAsync();
            return Ok(book);
        }

        // Admin: Delete a book
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book deleted.");
        }

        // User: Get all books with filters, search, sort, pagination
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] BookQueryParameters query)
        {
            var books = _context.Books.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(query.Search))
            {
                books = books.Where(b =>
                    b.Title.ToLower().Contains(query.Search.ToLower()) ||
                    b.ISBN.ToLower().Contains(query.Search.ToLower()) ||
                    b.Description.ToLower().Contains(query.Search.ToLower()));
            }

            // Filter
            if (!string.IsNullOrEmpty(query.Genre))
                books = books.Where(b => b.Genre.ToLower() == query.Genre.ToLower());

            if (!string.IsNullOrEmpty(query.Author))
                books = books.Where(b => b.Author.ToLower() == query.Author.ToLower());

            if (query.MinPrice.HasValue)
                books = books.Where(b => b.Price >= query.MinPrice);

            if (query.MaxPrice.HasValue)
                books = books.Where(b => b.Price <= query.MaxPrice);

            // Sort
            books = query.SortBy?.ToLower() switch
            {
                "price" => books.OrderBy(b => b.Price),
                "popularity" => books.OrderByDescending(b => b.SoldCount),
                "title" => books.OrderBy(b => b.Title),
                "date" => books.OrderByDescending(b => b.PublicationDate),
                _ => books.OrderBy(b => b.Title)
            };

            // Pagination
            var skip = (query.Page - 1) * query.PageSize;
            var result = await books.Skip(skip).Take(query.PageSize).ToListAsync();

            return Ok(result);
        }

        // User: Get single book by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound("Book not found.");
            return Ok(book);
        }
    }

