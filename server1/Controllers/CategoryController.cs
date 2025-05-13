using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    // Admin: Create a custom category
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory(CategoryDTO categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            Type = "Custom"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    // Admin: Update a category
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryDTO categoryDto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Category not found.");

        // Prevent modifying system categories
        if (category.Type == "System")
            return BadRequest("System categories cannot be modified.");

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;

        await _context.SaveChangesAsync();
        return Ok(category);
    }

    // Admin: Delete a category
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Category not found.");

        // Prevent deleting system categories
        if (category.Type == "System")
            return BadRequest("System categories cannot be deleted.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok("Category deleted.");
    }

    // Get all categories with optional filtering
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryParameters query)
    {
        var categories = _context.Categories.AsQueryable();

        // Filter by type if specified
        if (!string.IsNullOrEmpty(query.Type))
            categories = categories.Where(c => c.Type.ToLower() == query.Type.ToLower());

        // Pagination
        var skip = (query.Page - 1) * query.PageSize;
        var result = await categories.Skip(skip).Take(query.PageSize).ToListAsync();

        return Ok(result);
    }

    // Get single category by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Category not found.");
        return Ok(category);
    }
}