
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server1.DTOs;
using server1.Models;
using server1.Services;


namespace server1.Services
{

    public class AnnouncementService : IAnnouncementService
    {
        private readonly AppDbContext _context;

        public AnnouncementService(AppDbContext context)
        {
            _context = context;
        }

        // Update the method to match the interface's non-nullable return type
        public async Task<AnnouncementDTO> GetAnnouncementById(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                throw new KeyNotFoundException($"Announcement with ID {id} not found.");
            }

            return new AnnouncementDTO
            {
                AnnouncementID = announcement.AnnouncementID,
                Message = announcement.Message,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate
            };
        }

        //Create a new announcements
        public async Task<bool> CreateAnnouncement(AnnouncementDTO announcementDto)
        {
            var announcement = new Announcement
            {
                AnnouncementID = announcementDto.AnnouncementID,
                Message = announcementDto.Message,
                StartDate = announcementDto.StartDate,
                EndDate = announcementDto.EndDate
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            return true; // Return true to match the interface's expected return type
        }

        //Update an existing announcement
        public async Task<bool> UpdateAnnouncement(int id, AnnouncementDTO updatedAnnouncement)
        {
            var existingAnnouncement = await _context.Announcements.FindAsync(id);
            if (existingAnnouncement == null) return false;

            existingAnnouncement.Message = updatedAnnouncement.Message;
            existingAnnouncement.StartDate = updatedAnnouncement.StartDate;
            existingAnnouncement.EndDate = updatedAnnouncement.EndDate;

            _context.Announcements.Update(existingAnnouncement);
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete an announcement by ID
        public async Task<bool> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return false;

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}