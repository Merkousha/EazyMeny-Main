using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EazyMenu.Application;

/// <summary>
/// تنظیمات Dependency Injection برای Application Layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // MediatR (CQRS Pattern)
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
