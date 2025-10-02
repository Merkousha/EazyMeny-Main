using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Infrastructure.Data;
using EazyMenu.Infrastructure.Repositories;
using EazyMenu.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EazyMenu.Infrastructure;

/// <summary>
/// تنظیمات Dependency Injection برای Infrastructure Layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddHttpClient<ISmsService, KavenegarSmsService>();
        services.AddHttpClient<IPaymentService, ZarinpalPaymentService>();
        services.AddScoped<IQRCodeService, QRCodeService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IOtpService, OtpService>();
        
        // Memory Cache برای OTP
        services.AddMemoryCache();

        return services;
    }
}
