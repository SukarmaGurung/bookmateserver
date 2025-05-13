using server1.Models;

namespace server1.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderConfirmation(string email, Order order)
        {
            // In production, integrate with SendGrid/Mailgun/etc.
            _logger.LogInformation($"Sending order confirmation to {email} for order #{order.Id}");
            await Task.Delay(100); // Simulate email send
        }
    }
}