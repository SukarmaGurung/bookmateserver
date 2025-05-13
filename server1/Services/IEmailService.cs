using server1.Models;

namespace server1.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmation(string email, Order order);
    }
}