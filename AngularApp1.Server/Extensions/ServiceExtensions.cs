using MoniteringSystem.Data;
using MoniteringSystem.Service;
using Microsoft.EntityFrameworkCore;
using MoniteringSystem.Configuration;

namespace MoniteringSystem.Extensions
{
    public static class ServiceExtensions
    {
        // Add database context registration
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 21)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure()));
        }

        // Add services like AuthService and EmailService
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAuthService, AuthService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
