using System.Threading.Tasks;

namespace MoniteringSystem.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
