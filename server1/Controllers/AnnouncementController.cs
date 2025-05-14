using Microsoft.AspNetCore.Mvc;
using server1.DTOs;
using server1.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server1.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService) //Use Interface, not Concrete Class
        {
            _announcementService = announcementService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAnnouncement([FromBody] AnnouncementDTO dto) // ✅ Mark as async
        {
            var createdAnnouncement = await _announcementService.CreateAnnouncement(dto); // ✅ Await the task

            return Ok(new
            {
                message = "Announcement added successfully!",
                createdAnnouncement
            });
        }
        
        

        //Get Announcement by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnnouncementById(int id) // ✅ Mark as async
        {
            var announcement = await _announcementService.GetAnnouncementById(id); // ✅ Await

            if (announcement == null)
                return NotFound(new { message = "Announcement not found!" });

            return Ok(announcement);
        }

        //Update Announcement
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementDTO dto) // ✅ Mark as async
        {
            var updatedAnnouncement = await _announcementService.UpdateAnnouncement(id, dto); // ✅ Await

            if (updatedAnnouncement == null)
                return NotFound(new { message = "Announcement not found!" });

            return Ok(new
            {
                message = "Announcement updated successfully!",
                updatedAnnouncement
            });
        }


        //Delete Announcement
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id) // ✅ Mark method as async
        {
            var success = await _announcementService.DeleteAnnouncement(id); // ✅ Await the Task<bool>

            if (!success)
                return NotFound(new { message = "Announcement not found!" });

            return Ok(new { message = "Announcement deleted successfully!" });
        }

    }
}
