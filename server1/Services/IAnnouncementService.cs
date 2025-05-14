using server1.DTOs;
using server1.Models;

namespace server1.Services
{
    public interface IAnnouncementService
    {
       
        Task<AnnouncementDTO> GetAnnouncementById(int id);
        Task<bool> CreateAnnouncement(AnnouncementDTO announcementDto);
        Task<bool> UpdateAnnouncement(int id, AnnouncementDTO updatedAnnouncement);
        Task<bool> DeleteAnnouncement(int id);
    }
}
