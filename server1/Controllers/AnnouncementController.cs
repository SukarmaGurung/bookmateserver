using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server1.Models;

namespace server1.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnnouncementController(AppDbContext context)
        {
            _context = context;
        }

        // Admin: Create new announcement
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] Announcement announcement)
        {
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            return Ok(announcement);
        }

        // Public: Get active announcements
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAnnouncements()
        {
            var now = DateTime.UtcNow;
            var active = await _context.Announcements
                .Where(a => a.StartTime <= now && a.EndTime >= now)
                .ToListAsync();

            return Ok(active);
        }

        // Admin: Get all announcements
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAnnouncements()
        {
            var all = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(all);
        }

        // Admin: Update an announcement
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] Announcement updated)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            announcement.Title = updated.Title;
            announcement.Message = updated.Message;
            announcement.Type = updated.Type;
            announcement.StartTime = updated.StartTime;
            announcement.EndTime = updated.EndTime;

            await _context.SaveChangesAsync();
            return Ok(announcement);
        }

        // Admin: Delete announcement
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
